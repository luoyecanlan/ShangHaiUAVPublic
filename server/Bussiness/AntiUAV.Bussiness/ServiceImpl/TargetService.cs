using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.DB;
using DbOrm.AntiUAV.Entity;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.ServiceImpl
{
    public class TargetService : ITargetService
    {
        public TargetService(ILogger<TargetService> logger, IUavModelService uav, IHistoryTrackService history, IWhiteListService white)
        {
            _logger = logger;
            _uav = uav;
            _history = history;
            _white = white;
        }

        private readonly ILogger _logger;

        private readonly IUavModelService _uav;

        private readonly IHistoryTrackService _history;

        private readonly IWhiteListService _white;

        /// <summary>
        /// 目标点更新最小间隔时间（秒）
        /// </summary>
        private readonly int TgPointsUpdateMinIntervalTime = 20;//s

        /// <summary>
        /// 目标消失后保留时间（秒）
        /// </summary>
        private readonly int TgDisappearSaveTime = 30;//s

        /// <summary>
        /// 获取最后更新目标信息
        /// </summary>
        /// <param name="tgId">目标ID</param>
        /// <returns></returns>
        public Task<TargetInfo> GetLastUpdateTargetInfo(string tgId, int devId)
        {
            try
            {
                return Task.FromResult(RedisHelper.Get<TargetInfo>(RedisCacheKeyConst.GetDeviceTrackCacheKey(tgId, devId)));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"读取目标{tgId}缓存数据异常.");
            }
            return default;
        }

        /// <summary>
        /// 获取目标最后更新位置信息
        /// </summary>
        /// <param name="timeStamp">更新时间戳(为空则获取全部数据)</param>
        /// <returns></returns>
        public Task<IEnumerable<TargetPosition>> GetLastUpdateTargetsPositionAsync(int devId, DateTime? timeStamp = null)
        {
            try
            {
                var keys = RedisHelper.Keys(RedisCacheKeyConst.GetDeviceTrackCacheKey("*", devId));
                var data = RedisHelper.MGet<TargetPosition>(keys);
                if (timeStamp != null)
                {
                    data = data.Where(tg => tg?.TrackTime > timeStamp).ToArray();
                }
                return Task.FromResult(data as IEnumerable<TargetPosition>);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"读取{timeStamp}时间内更新目标缓存数据异常.");
            }
            return default;
        }

        /// <summary>
        /// 获取消失目标
        /// </summary>
        /// <param name="devId">设备ID</param>
        /// <returns></returns>
        public Task<IEnumerable<TargetDisappear>> GetLastDisappearTargets(int devId)
        {
            try
            {
                var diskey = RedisCacheKeyConst.GetDeviceTrackDisCacheKey(devId);//消失目标存储REDIS KEY
                var res = RedisHelper.HGetAll<DateTime>(diskey)?.Select(s => new TargetDisappear()
                {
                    TargetId = s.Key,
                    DisappearTime = s.Value,
                    CauseOfDisappear = DisappearType.ClearBatch
                }).ToList();
                return Task.FromResult(res as IEnumerable<TargetDisappear>);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"销批目标缓存数据获取异常.");
            }
            return default;
        }

        /// <summary>
        /// 更新设备目标信息
        /// 1.消失超过30秒的目标从消失集合中移除
        /// 2.目标自销毁周期是20s（期间无更新则自动销毁）
        /// </summary>
        /// <param name="devId">设备ID</param>
        /// <param name="targets">更新的目标信息</param>
        /// <param name="distgs">消失的目标ID</param>
        /// <returns></returns>
        public Task UpdateTargetsInfo(int devId, IEnumerable<TargetInfo> targets, IEnumerable<string> distgs)
        {
            if (devId > 0)
            {
                #region 清理已经消失时间太久的目标列表内容
                var diskey = RedisCacheKeyConst.GetDeviceTrackDisCacheKey(devId);//消失目标存储REDIS KEY
                var rd_distgs = RedisHelper.HGetAll<DateTime>(diskey)
                    .Where(x => x.Value.AddSeconds(TgDisappearSaveTime) < DateTime.Now).Select(s => s.Key).ToArray();//可清理的消失目标集合，消失超过30秒的目标
                if (rd_distgs.Length > 0)
                    RedisHelper.HDel(diskey, rd_distgs);//清理消失超过x秒的目标
                #endregion

                RedisHelper.StartPipe(pipe =>
                {
                    if (distgs != null)
                    {
                        foreach (var distgId in distgs)
                        {
                            pipe.Del(RedisCacheKeyConst.GetDeviceTrackCacheKey(distgId, devId));//目标消失，则删除该目标最后位置信息
                            pipe.HSet(diskey, distgId, DateTime.Now);//消失目标集合加入这个目标
                        }
                    }
                    if (targets != null)
                    {
                        foreach (var tg in targets)
                        {
                            pipe.Set(RedisCacheKeyConst.GetDeviceTrackCacheKey(tg.Id, devId), tg, TgPointsUpdateMinIntervalTime);//单目标点存活最长周期20s，最新信息写入
                            pipe.HSetNx(RedisCacheKeyConst.GetDeviceHistoryCacheKey(tg.Id, devId), tg.TrackTime.ToString("yyyyMMddHHmmss.ffff"), tg);//将最新的点写入历史集合中，这里后续要写入mysql,考虑加入超时限制
                        }
                    }
                });
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 更新正在追踪的目标信息
        /// </summary>
        /// <param name="devId">目标来源设备ID</param>
        /// <param name="tg">更新的目标信息</param>
        /// <returns></returns>
        public Task UpdateTargetInfo(int devId, TargetInfo tg)
        {
            if (devId > 0)
            {
                var lastkey = RedisCacheKeyConst.GetDeviceTrackCacheKey(tg.Id, devId);
                RedisHelper.Set(lastkey, tg, TgPointsUpdateMinIntervalTime);//单目标点存活最长周期20s，最新信息写入
                var hitorykey = RedisCacheKeyConst.GetDeviceHistoryCacheKey(tg.Id, devId);
                RedisHelper.HSet(hitorykey, tg.TrackTime.ToString("yyyyMMddHHmmss.ffff"), tg);//将最新的点写入历史集合中，这里后续要写入mysql,考虑加入超时限制
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 消失目标信息保存
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public async Task DisappearTgsSave(int devId)
        {
            if (devId > 0)
            {
                //看看有多少消失了还在redis浪着的数据
                //先获取所有的历史数据目标， 加入目标编号为空过滤
                var historykeys = RedisHelper.Keys(RedisCacheKeyConst.GetDeviceHistoryCacheKey("*", devId)).
                    Where(k => !string.IsNullOrEmpty(k.Replace(RedisCacheKeyConst.GetDeviceHistoryCacheKey("", devId), "")));
                var savekeys = historykeys.Where(x => !RedisHelper.Exists(RedisCacheKeyConst.ConvertHistoryKeyToTrackKey(x))).ToArray();
                var delkeys = new List<string>(savekeys); //需要在redis 删除的Key

                if (savekeys.Length > 0)
                {
                    _logger.LogDebug("历史数据整理中....,本次整理目标数：" + savekeys.Length);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    #region redis 读取目标数据
                    var models = await _uav.GetAnyAsync();
                    var white = await _white.GetAnyAsync(x => x.StarTime.AddDays(-1) < DateTime.Now && x.EndTime.AddDays(1) > DateTime.Now);
                    var descrpList = RedisHelper.StartPipe(pipe =>
                    {
                        foreach (var save in savekeys)
                        {
                            pipe.HVals<TargetInfo>(save);
                        }
                    }).Select(s => GetTargetDescription(s as TargetInfo[], models, white)).ToList();
                    #endregion

                    sw.Stop();

                    if (descrpList.Count > 0)
                    {
                        _logger.LogDebug("Redis读取数据并计算ORM模型耗时：" + sw.Elapsed.TotalMilliseconds);
                        sw.Restart();
                        var commit = false;
                        int i = 0;

                        #region mysql 存储
                        using (var db = new AntiuavDB())
                        {
                            using var t = db.BeginTransaction();
                            var noExistList = from c in descrpList
                                              where (from s in db.Targets where s.Id == c.Target.Id select s).Count() <= 0
                                              && !string.IsNullOrEmpty(c.Target.TgId)
                                              select c;
                            try
                            {
                                foreach (var item in noExistList)
                                {
                                    db.Insert(item.Target);
                                    db.BulkCopy(item.TrackList);
                                    i += 1 + item.TrackList.Count();
                                }
                                t.Commit();
                                commit = true;
                            }
                            catch (Exception ex)
                            {
                                t.Rollback();
                                commit = false;
                                throw ex;
                            }
                        }
                        #endregion

                        sw.Stop();
                        _logger.LogDebug("数据库操作耗时: {0},数据量：{1}", sw.Elapsed.TotalMilliseconds, i);//入库时间

                        if (commit && delkeys.Count() > 0)//提交成功则从redis中删除这些数据
                            RedisHelper.Del(delkeys.ToArray());
                    }
                }
            }
            //return Task.CompletedTask;
        }

        private TargetDescription GetTargetDescription(TargetInfo[] values, IEnumerable<UAVModelInfo> uavs, IEnumerable<WhiteListInfo> whites)
        {
            HistoryTargetAdd targetEntity = new HistoryTargetAdd();
            IList<HistoryTrackAdd> trackEntityList = new List<HistoryTrackAdd>();

            var result = from c in values group c.Category by c.Category into g orderby g.Count() descending select new { tp = g.Key, num = g.Count() };
            var obj = result.FirstOrDefault(); //出现次数最多的设备类型及次数

            var category = obj.tp;
            targetEntity.Category = category;

            var pro = obj.num / (double)values.Length;
            targetEntity.CategoryProportion = Math.Round(pro, 2);

            targetEntity.Count = values.Length;
            targetEntity.DeviceCount = 1;

            var timeList = from c in values orderby c.TrackTime select new { time = c.TrackTime };
            targetEntity.Endtime = timeList.Last().time;
            targetEntity.Starttime = timeList.First().time;


            targetEntity.TgId = values.First()?.Id;
            targetEntity.Sn = values.First()?.UAVSn;

            targetEntity.UAVModel = uavs.FirstOrDefault(x => x.Name.Contains(values.First()?.UAVType ?? ""))?.Id ?? 0;
            targetEntity.FlyerPosition = values.LastOrDefault()?.AppAddr ?? "未知";

            var hit = _history.RemoveHitMark(targetEntity.TgId).Result;
            targetEntity.HitMark = Convert.ToInt32(hit);

            var trackList = from c in values
                            select new HistoryTrackAdd
                            {
                                Alt = c.Alt,
                                Category = c.Category,
                                DeviceId = c.DeviceId,
                                Lat = c.Lat,
                                Lng = c.Lng,
                                Mode = c.Mode,
                                //若在白名单，则威胁度0，若不在白名单按原威胁度赋值
                                Threat = whites.Any(x => x.Sn.ToUpper() == c.UAVSn.ToUpper() && c.TrackTime >= x.StarTime && c.TrackTime <= x.EndTime) ? 0 : c.Threat,
                                TrackTime = c.TrackTime,
                                TargetId = c.Id
                            };

            targetEntity.ThreatMax = trackList.Max(m => m.Threat);
            return new TargetDescription() { Target = targetEntity, TrackList = trackList };
        }

        /// <summary>
        /// 目标是否存在
        /// </summary>
        /// <param name="tgId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public bool TargetExistence(string tgId, int deviceId)
        {
            return RedisHelper.Exists(RedisCacheKeyConst.GetDeviceTrackCacheKey(tgId, deviceId));
        }
    }

    /*
     

        /// <summary>
        /// 获取消失目标
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <returns></returns>
        public async Task<IEnumerable<TargetDisappear>> GetLastDisappearTargets(DateTime? timeStamp = null)
        {
            try
            {
                return (await RedisHelper.HGetAllAsync<TargetDisappear>(_disappearTargets))?.Where(x => x.Value.DisappearTime >= timeStamp).Select(s => s.Value).ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"销批目标缓存数据获取异常.");
            }
            return default;
        }

        /// <summary>
        /// 获取最后更新目标信息
        /// </summary>
        /// <param name="tgId">目标ID</param>
        /// <returns></returns>
        public Task<TargetInfo> GetLastUpdateTargetInfo(string tgId)
        {
            try
            {
                return RedisHelper.HGetAsync<TargetInfo>(_realTimeTargets, tgId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"读取目标{tgId}缓存数据异常.");
            }
            return default;
        }

        /// <summary>
        /// 获取目标最后更新位置信息
        /// </summary>
        /// <param name="timeStamp">更新时间戳(为空则获取全部数据)</param>
        /// <returns></returns>
        public async Task<IEnumerable<TargetPosition>> GetLastUpdateTargetsPositionAsync(DateTime? timeStamp = null)
        {
            try
            {
                return (await RedisHelper.HGetAllAsync<TargetPosition>(_realTimeTargets))?
                                  .Where(tg => tg.Value?.TrackTime > timeStamp)
                                  .Select(s => s.Value);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"读取{timeStamp}时间内更新目标缓存数据异常.");
            }
            return default;
        }


    
        //public IEnumerable<string> GetTimeOutTarget(double cycleCount)
        //{
        //    var time = DateTime.Now.AddSeconds(-1 * cycleCount);
        //    var tgs = RedisHelper.HGetAll<TargetPosition>(_realTimeTargets);
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// 消失目标数据保存
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public Task DisappearTargetsSave()
        {
            var dtgs = RedisHelper.LRange<TargetDisappear>(_waitsaveTargets, 0, -1);
            var track = new Dictionary<TargetDisappear, IEnumerable<TargetInfo>>();
            foreach (var dtg in dtgs)
            {
                var tgi = RedisHelper.LRange<TargetInfo>(GetTargetPointsKey(dtg.TargetId), 0, -1);
                if (tgi.Count() > 5)
                    track.Add(dtg, tgi);
                else
                    _logger.LogInformation($"目标{dtg.TargetId}航迹点小于5个点({tgi.Count()}点),已丢弃.");
            }

            //整理track数据
            //数据保存至数据库

            return Task.CompletedTask;
        }

        /// <summary>
        /// 目标消失
        /// </summary>
        /// <param name="tgs"></param>
        /// <returns></returns>
        public Task TargetDisappear(params TargetDisappear[] tgs)
        {
            try
            {
                //var dis = tgs?.ToDictionary(k => k.TargetId, v => v);
                var dis = new List<object>();
                tgs.ToList().ForEach(f =>
                {
                    dis.Add(f.TargetId);
                    dis.Add(f);
                });
                RedisHelper.StartPipe(pipe =>
                {
                    pipe.HDel(_realTimeTargets, tgs.Select(x => x.TargetId).ToArray());
                    pipe.HMSet(_disappearTargets, dis.ToArray());
                    pipe.LPush(_waitsaveTargets, tgs);
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"销批失败,更新目标缓存数据异常.");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 更新目标信息
        /// </summary>
        /// <param name="devId">设备ID</param>
        /// <param name="probe">探测目标信息</param>
        /// <returns></returns>
        public Task UpdateTargets(int devId, params TargetInfo[] probe)
        {
            try
            {
                //var tgs = probe?.ToDictionary(k => k.Id, v => v);
                var tgs = new List<object>();
                probe.ToList().ForEach(f =>
                {
                    tgs.Add(f.Id);
                    tgs.Add(f);
                });
                if (tgs?.Count() > 0)
                {
                    RedisHelper.StartPipe(pipe =>
                    {
                        pipe.HMSet(_realTimeTargets, tgs.ToArray());
                        foreach (var tg in probe)
                        {
                            pipe.LPush(GetTargetPointsKey(tg.Id), tg);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"更新设备{devId}目标({probe?.Count()}批)缓存数据异常.");
            }
            return Task.CompletedTask;
        }
     
     */
}
