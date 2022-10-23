using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static AntiUAV.Bussiness.GisTool;

namespace AntiUAV.Bussiness.ServiceImpl
{
    /// <summary>
    /// 设备服务业务
    /// </summary>
    public class DeviceService : MetadataService<DeviceInfo, DeviceUpdate, DeviceDel, DeviceAdd>, IDeviceService
    {
        private readonly ITargetService _target;
        private readonly GisTool _gis;
        public DeviceService(IEntityCrudService orm, ITargetService target, GisTool gis) : base(orm)
        {
            _target = target;
            _gis = gis;
        }

        /// <summary>
        /// 获取设备分类信息
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DeviceCategoryInfo>> GetCategory()
        {
            try
            {
                return await _orm.GetAnyAsync<DeviceCategoryInfo>();
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptGetFail, ex);
            }
        }

        public Task<DeviceServiceInfo> GetHost(int deviceId)
        {
            var key = RedisCacheKeyConst.GetDeviceHostCacheKey(deviceId);
            return Task.FromResult(RedisHelper.Get<DeviceServiceInfo>(key));
        }

        public Task<IEnumerable<DeviceServiceInfo>> GetProbeHost()
        {
            var key = RedisCacheKeyConst.GetDeviceHostCacheKey(0);
            var keys = RedisHelper.Keys(key);
            var infos = RedisHelper.MGet<DeviceServiceInfo>(keys);
            return Task.FromResult(infos?.Where(x => x.Category.IsProbeDevice()));
        }

        public Task<bool> RegistHost(int deviceId, int category, string host)
        {
            var key = RedisCacheKeyConst.GetDeviceHostCacheKey(deviceId);
            var res = RedisHelper.SetNx(key, new DeviceServiceInfo() { DeviceId = deviceId, Category = category, RpcHost = host });
            if (res)
            {
                RedisHelper.Expire(key, 15);
            }
            return Task.FromResult(res);
        }

        /// <summary>
        /// 获取设备状态
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public Task<IEnumerable<DeviceStatus>> GetStatus(params int[] devId)
        {
            if (devId?.Count() <= 0 || devId.Any(x => x <= 0))
                throw new BussinessException(BussinessExceptionCode.ParamInvalidId);
            try
            {
                var keys = devId.Select(s => RedisCacheKeyConst.GetDeviceStatusCacheKey(s)).ToArray();
                return Task.FromResult(RedisHelper.MGet<DeviceStatus>(keys) as IEnumerable<DeviceStatus>);
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptGetFail, ex);
            }
        }
        /// <summary>
        /// 获取所有设备状态
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<DeviceStatus>> GetStatus()
        {
            try
            {
                var keys = RedisHelper.Keys(RedisCacheKeyConst.GetDeviceStatusCacheKey(0));
                var res = RedisHelper.MGet<DeviceStatus>(keys) ?? new DeviceStatus[0];
                return Task.FromResult(res as IEnumerable<DeviceStatus>);
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptGetFail, ex);
            }
        }

        /// <summary>
        /// 获取单一设备状态
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public Task<DeviceStatus> GetStatusOne(int devId)
        {
            if (devId <= 0)
                throw new BussinessException(BussinessExceptionCode.ParamInvalidId);
            try
            {
                return Task.FromResult(RedisHelper.Get<DeviceStatus>(RedisCacheKeyConst.GetDeviceStatusCacheKey(devId)));
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptGetFail, ex);
            }
        }

        /// <summary>
        /// 更新设备位置信息
        /// </summary>
        /// <param name="device">可更新的设备信息</param>
        /// <returns>设备信息</returns>
        public async Task<DeviceInfo> Update(DeviceUpdatePosition position)
        {
            if (position == null)
                throw new BussinessException(BussinessExceptionCode.ParamNull, "更新位置信息");
            bool res;
            try
            {
                res = await _orm.UpdateAsync(position);
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptUpdateFail, ex, message: $"更新位置信息,Id:{position.Id}");
            }
            return res ? await GetAsync(position.Id) : default;
        }

        /// <summary>
        /// 更新设备状态信息
        /// </summary>
        /// <param name="status"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool UpdateStatus(DeviceStatus status, int time)
        {
            return RedisHelper.Set(RedisCacheKeyConst.GetDeviceStatusCacheKey(status.DeviceId), status, time);
        }

        private static readonly object bestDeviceLock = new object();
        /// <summary>
        /// 根据目标位置获取最近的可用设备
        /// </summary>
        /// <param name="tgid">目标id</param>
        /// <param name="func">设备类型筛选</param>
        /// <returns></returns>
        public async Task<DeviceInfo> GetBastDevice(string tgId, Func<DeviceInfo, bool> predicate, RelationshipsType type)
        {
            //Target ID：P10102.7.SFml-18267
            int devId = tgId.ToDeviceId();
            if (devId > 0)
            {
                //获取目标最后点的坐标
                var tgInfo = await _target.GetLastUpdateTargetInfo(tgId, devId);
                //目标消失
                if (tgInfo == null) throw new BussinessException(BussinessExceptionCode.ParamNull);
                var tgP = new Position()
                {
                    Lat = tgInfo.Lat,
                    Lng = tgInfo.Lng,
                    Altitude = tgInfo.Alt
                };
                //获取设备列表
                var devs = (await GetAnyAsync())?.Where(predicate);
                //获取当前被占用的设备Id集合
                var _ids = (await GetRelationships(f => f.RType == type))?.Select(f => f.ToDeviceId);
                //过滤掉被占用的设备
                if (_ids?.Count() > 0)
                {
                    devs = devs?.Where(f => { return !_ids.Contains(f.Id); });
                }
                //过滤锁定的设备
                var _selectIds = RedisHelper.MGet<int>(RedisHelper.Keys($"{RedisCacheKeyConst.DeviceSelectKey}*"));
                if (_selectIds?.Count() > 0)
                {
                    devs = devs?.Where(f => { return !_selectIds.Contains(f.Id); });
                }
                if (devs?.Count() > 0)
                {
                    var devStatus = await GetStatus(devs.Select(f => f.Id).ToArray());
                    var bestId = 0;
                    lock (bestDeviceLock)
                    {
                        //计算并返回最近且闲置的设备
                        var disInfos = devs.Select(f =>
                        {
                            var devP = new Position()
                            {
                                Lat = f.Lat,
                                Lng = f.Lng,
                                Altitude = f.Alt
                            };
                            var status = devStatus.FirstOrDefault(ds => ds?.DeviceId == f.Id);
                            return new
                            {
                                f.Id,
                                RunStatus = status?.Code ?? 0,
                                Dis = _gis.CalculatingDistance(devP, tgP)
                            };
                        });
                        int did = 0;
                        //121.502914国会-4
                        //121.503772绿地-5
                        if (tgP.Lng <= 121.502914)
                        {
                            did = 4;
                        }
                        if (tgP.Lng >= 121.503772)
                        {
                            did = 5;
                        }
                        //设备工作状态(0:工作中;1:停止;2:待机;3:设备通信故障)
                        var bestDev = disInfos?.OrderBy(f => f.Dis).FirstOrDefault(f => f.Id == did && f.RunStatus == DeviceStatusCode.Free);
                        if (bestDev != null)
                        {
                            bestId = bestDev.Id;
                            //写入redis设备被锁定  定时过期
                            RedisHelper.Set($"{RedisCacheKeyConst.DeviceSelectKey}{bestId}", bestId, 2);
                        }
                    }
                    return devId > 0 ? devs.FirstOrDefault(f => f.Id == bestId) : default;
                }
            }
            return default;
        }

        /// <summary>
        /// 获取被当前目标引导的设备信息集合
        /// </summary>
        /// <param name="tgId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DeviceInfo>> GetBeGuidanceDevice(string tgId)
        {
            //获取到非转发的关联关系
            var devs = await GetRelationships(f => f.RType != RelationshipsType.PositionTurn);
            if (devs?.Count() > 0)
            {
                var ids = devs.Where(f => f?.TargetId == tgId).Select(f => f.ToDeviceId);
                return await GetAnyAsync(f => ids.Contains(f.Id));
            }
            return default;
        }

        /// <summary>
        /// 增加一个关联关系(若此关系已存在则更新此关系)
        /// </summary>
        /// <param name="relationships"></param>
        /// <returns></returns>
        public Task<bool> AddRelationships(Relationships relationships)
        {
            if (relationships == null) return default;

            relationships.UpdateTime = DateTime.Now;
            return Task.FromResult(RedisHelper.HSet(RedisCacheKeyConst.GetRelationshipsCacheKey(),
                                         //RedisCacheKeyConst.GetRelationshipsFieldCacheKey(relationships.TargetId, relationships.ToDeviceId, relationships.RType),
                                         relationships.Id,
                                         relationships));

        }

        /// <summary>
        /// 移除一个关联关系
        /// <param name="tgId">目标ID</param>
        /// <param name="type">关联类型</param>
        /// <returns></returns>
        public async Task<bool> RemoveRelationships(string tgId, RelationshipsType type)
        {
            var all = await GetRelationships(r => r.TargetId == tgId && r.RType == type);
            if (all.Count() > 0)
            {
                return RedisHelper.HDel(RedisCacheKeyConst.GetRelationshipsCacheKey(), all.Select(s => s.Id).ToArray()) > 0;
                //all.Select(s => RedisCacheKeyConst.GetRelationshipsFieldCacheKey(s.TargetId, s.ToDeviceId, s.RType)).ToArray()) > 0;

            }
            return true;
        }

        /// <summary>
        /// 移除一个关联关系
        /// </summary>
        /// <param name="predicate">删除关系的条件</param>
        /// <returns></returns>
        public async Task<bool> RemoveRelationships(Func<Relationships, bool> predicate = null)
        {
            var all = await GetRelationships(predicate);
            if (all.Count() > 0)
            {
                return RedisHelper.HDel(RedisCacheKeyConst.GetRelationshipsCacheKey(), all.Select(s => s.Id).ToArray()) > 0;
                //return RedisHelper.HDel(RedisCacheKeyConst.GetRelationshipsCacheKey(),
                //     all.Select(s => RedisCacheKeyConst.GetRelationshipsFieldCacheKey(s.TargetId, s.ToDeviceId, s.RType)).ToArray()) > 0;
            }
            return true;
        }

        /// <summary>
        /// 获取关联关系
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<IEnumerable<Relationships>> GetRelationships(Func<Relationships, bool> predicate = null)
        {
            var relations = RedisHelper.HGetAll<Relationships>(RedisCacheKeyConst.GetRelationshipsCacheKey()).Select(s => s.Value);
            if (predicate != null)
            {
                relations = relations?.Where(predicate);
            }
            var res = relations?.ToList() ?? new List<Relationships>();
            return Task.FromResult(res as IEnumerable<Relationships>);
        }

        /// <summary>
        /// 移除一个关联关系
        /// </summary>
        /// <param name="rid">关联ID</param>
        /// <returns></returns>
        public Task<bool> RemoveRelationships(string rid)
        {
            var res = RedisHelper.HDel(RedisCacheKeyConst.GetRelationshipsCacheKey(), rid);
            return Task.FromResult(res >= 0);
        }

        /// <summary>
        /// 设备是否在线
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public bool? IsOnline(int devid)
        {
            if (devid <= 0) return default;
            return RedisHelper.Exists(RedisCacheKeyConst.GetDeviceStatusCacheKey(devid));
        }

    }
}
