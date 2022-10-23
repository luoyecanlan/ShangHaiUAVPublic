using AntiUAV.DeviceServer.Abstractions;
using AntiUAV.DeviceServer.Abstractions.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using System.ComponentModel.DataAnnotations;

namespace AntiUAV.DeviceServer
{
    /// <summary>
    /// 缓存业务数据扩展
    /// </summary>
    public static class MemoryCacheBusinessExtension
    {
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        public static DeviceInfo GetDevice(this IMemoryCache memory)
        {
            memory.TryGetValue(MemoryCacheKey.DeviceInfoKey, out DeviceInfo info);
            return info;
        }

        /// <summary>
        /// 判定是否可以引导其他设备
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool CanGuidance(this DeviceInfo info) => info?.Category < 30000;//非干扰类都可引导其他人

        /// <summary>
        /// 判定是否可以被其他设备引导
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool CanBeGuidance(this DeviceInfo info) => info?.Category > 20000;//非探测类都可被其他人引导

        /// <summary>
        /// 判定是否为可追踪设备
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool IsMonitor(this DeviceInfo info) => info?.Category > 20000 && info?.Category < 30000;//目前广电类可追踪目标


        /// <summary>
        /// 判断设备类型是否是探测设备
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool IsProbe(this DeviceInfo info)
        {
            return info?.Category < 20000;
        }

        /// <summary>
        /// 重新加载设备信息（若无设备信息，则初始化该信息）
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool ReloadDevice(this IMemoryCache memory, DeviceInfo info)
        {
            if (info?.Id > 0)
            {
                info.LocalUpdateTime = DateTime.Now;

                if (memory.TryGetValue(MemoryCacheKey.DeviceInfoKey, out DeviceInfo devInfo))
                {
                    if (devInfo.Id != info.Id)
                        return false;
                    foreach (var pro in info.GetType().GetProperties())
                    {
                        //if (pro.Name == "RpcHost") continue;
                        devInfo.GetType().GetProperty(pro.Name).SetValue(devInfo, pro.GetValue(info));
                    }
                    devInfo.LocalUpdateTime = DateTime.Now;
                }
                else
                {
                    memory.Set(MemoryCacheKey.DeviceInfoKey, info);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取所有预警区
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        public static PerWarningZoneInfoCollection GetZones(this IMemoryCache memory)
        {
            memory.TryGetValue(MemoryCacheKey.DevicePerWarningZoneKey, out PerWarningZoneInfoCollection zones);
            return zones;
        }

        /// <summary>
        /// 重新加载所有预警区
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="zones"></param>
        /// <returns></returns>
        public static bool ReloadZones(this IMemoryCache memory, PerWarningZoneInfoCollection zones)
        {
            if (zones == null) return false;
            zones.LocalUpdateTime = DateTime.Now;
            memory.Set(MemoryCacheKey.DevicePerWarningZoneKey, zones);
            return true;
        }

        #region 威胁判定
        /// <summary>
        /// 威胁判定
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="tps">目标点集</param>
        /// <param name="weight">威胁判定权重</param>
        /// <param name="tool">计算工具</param>
        /// <param name="midCount">中间点</param>
        /// <returns></returns>
        public static Task Assessment(this IMemoryCache memory, IEnumerable<TargetPosition> tps, IThreatWeight weight, GisTool tool, int midCount = 6)
        {
            //foreach (var zone in tzs)
            //{
            //    var dis = GisTool.Instance.CalculatingDistance(new GisCaclBase.Position() { Lat = tg.Last.Lat, Lng = tg.Last.Lng }, new GisCaclBase.Position() { Lat = zone.CircumcircleLat, Lng = zone.CircumcircleLng });
            //    if (dis <= zone.CircumcircleR)
            //    {
            //        tg.Score = 100;
            //        break;
            //    }
            //}

            var tgFirst = tps?.FirstOrDefault();
            var tgLast = tps?.LastOrDefault();
            var tgMid = tps?.ElementAtOrDefault(midCount);
            var zones = memory.GetZones()?.Zones;
            var r_socre = 50d;

            foreach (var zone in zones)
            {
                var dis = tool.CalculatingDistance(new GisTool.Position() { Lat = tgLast.Lat, Lng = tgLast.Lng },
                    new GisTool.Position() { Lat = zone.CircumcircleLat, Lng = zone.CircumcircleLng });
                if (dis <= zone.CircumcircleR)
                {
                    r_socre = 100;
                    break;
                }
            }

            //foreach (var zone in zones)
            //{
            //    if (GisTool.isPointInPolygon(new GisTool.Position() { Lat = tgLast.Lat, Lng = tgLast.Lng }, zone.ZPointsPosition))
            //    {
            //        if (zone.Id == 19)
            //        {
            //            if (r_socre < 50)
            //                r_socre = 50;
            //        }
            //        else if (zone.Id == 20)
            //        {
            //            r_socre = 100;
            //        }
            //    }
            //}
            if (tgFirst != null)
                memory.Set($"{MemoryCacheKey.TargetAssessmentKey}.{tgFirst.Id}", r_socre, TimeSpan.FromSeconds(10), true);
            return Task.CompletedTask;
        }
        ///// <summary>
        ///// 威胁判定
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <param name="tps">目标点集</param>
        ///// <param name="weight">威胁判定权重</param>
        ///// <param name="tool">计算工具</param>
        ///// <param name="midCount">中间点</param>
        ///// <returns></returns>
        //public static Task Assessment(this IMemoryCache memory, IEnumerable<TargetPosition> tps, IThreatWeight weight, GisTool tool, int midCount = 6)
        //{
        //    //foreach (var zone in tzs)
        //    //{
        //    //    var dis = GisTool.Instance.CalculatingDistance(new GisCaclBase.Position() { Lat = tg.Last.Lat, Lng = tg.Last.Lng }, new GisCaclBase.Position() { Lat = zone.CircumcircleLat, Lng = zone.CircumcircleLng });
        //    //    if (dis <= zone.CircumcircleR)
        //    //    {
        //    //        tg.Score = 100;
        //    //        break;
        //    //    }
        //    //}

        //    var tgFirst = tps?.FirstOrDefault();
        //    var tgLast = tps?.LastOrDefault();
        //    var tgMid = tps?.ElementAtOrDefault(midCount);
        //    var zones = memory.GetZones()?.Zones;
        //    var r_socre = 0d;
        //    if (weight != null && tool != null && tgFirst != null && tgLast != null && tgMid != null && zones?.Count() > 0)
        //    {
        //        var tgAllCount = tps.Count();
        //        double tgRealCount = tps.Where(x => x.Mode == 0).Count();
        //        var timeSc = (tgLast.TrackTime - tgMid.TrackTime).TotalMilliseconds;
        //        var tgLastPos = new GisTool.Position() { Lat = tgLast.Lat, Lng = tgLast.Lng };
        //        var tgFirstPos = new GisTool.Position() { Lat = tgFirst.Lat, Lng = tgFirst.Lng };
        //        var tgMidPos = new GisTool.Position() { Lat = tgMid.Lat, Lng = tgMid.Lng };

        //        foreach (var zone in zones)
        //        {
        //            var conTime = 1;
        //            var avgVecolity = 1d;
        //            var distance = double.MaxValue;
        //            var instantVecolity = 1d;
        //            var trackQuality = 0d;


        //            if (tgLast != null && tgFirst != null)
        //            {
        //                var targetBear = tool.Convert3DPositionAzimuthAndPitchInfo(tgLastPos, new GisTool.Position() { Lat = zone.CircumcircleLat, Lng = zone.CircumcircleLng });
        //                if (targetBear == null) continue;
        //                //if (zone.Id == 2)
        //                {
        //                    if (targetBear.Dis > zone.CircumcircleR)
        //                    { //r_socre = 0;
        //                        continue;
        //                    }//未进入外接辐射圆
        //                    else
        //                        r_socre = 100;
        //                }


        //            }

        //        }
        //    }
        //    if (tgFirst != null)
        //        memory.Set($"{MemoryCacheKey.TargetAssessmentKey}.{tgFirst.Id}", r_socre, TimeSpan.FromSeconds(10), true);
        //    return Task.CompletedTask;
        //}

        /// <summary>
        /// 获取威胁度
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="tgid">目标ID</param>
        /// <returns></returns>
        public static double GetThreat(this IMemoryCache memory, string tgid)
        {
            memory.TryGetValue($"{MemoryCacheKey.TargetAssessmentKey}.{tgid}", out double socre);
            return socre;
        }

        #endregion

        #region 实际
        /// <summary>
        /// 更新设备实际纠偏信息
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="rectify"></param>
        public static void UpdateDevRectify(this IMemoryCache memory, DevRectifyInfo rectify)
        {
            if (rectify == null) return;
            var info = memory.GetOrCreate(MemoryCacheKey.DevRectifyInfoKey, entity => new DevRectifyInfo());
            info.Az = rectify.Az;
            info.El = rectify.El;
        }

        /// <summary>
        /// 获取设备实际纠偏信息
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        public static DevRectifyInfo GetDevRectify(this IMemoryCache memory)
        {
            memory.TryGetValue(MemoryCacheKey.DevRectifyInfoKey, out DevRectifyInfo info);
            return info;
        }

        /// <summary>
        /// 更新设备实际位置信息
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="position"></param>
        public static void UpdateDevPosition(this IMemoryCache memory, DevPositionInfo position)
        {
            if (position == null) return;
            var info = memory.GetOrCreate(MemoryCacheKey.DevPositionInfoKey, entity => new DevPositionInfo());
            info.Lat = position.Lat;
            info.Lng = position.Lng;
            info.Alt = position.Alt;
        }

        /// <summary>
        /// 获取设备实际位置信息
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        public static DevPositionInfo GetDevPosition(this IMemoryCache memory)
        {
            memory.TryGetValue(MemoryCacheKey.DevPositionInfoKey, out DevPositionInfo info);
            return info;
        }

        #endregion

        #region 状态

        /// <summary>
        /// 获取设备状态信息
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        public static DeviceStatusInfo GetDeviceStatus(this IMemoryCache memory)
        {
            if (!memory.TryGetValue(MemoryCacheKey.DeviceStatusKey, out DeviceStatusInfo status))
            {
                var dev = memory.GetDevice();
                if (dev != null)
                {
                    status = new DeviceStatusInfo()
                    {
                        DeviceId = dev.Id,
                        DeviceCategory = dev.Category,
                        Code = DeviceStatusCode.Free,
                        UpdateTime = DateTime.Now
                    };
                    memory.Set(MemoryCacheKey.DeviceStatusKey, status);
                }
            }
            return status;
        }

        /// <summary>
        /// 更新设备BIT异常信息
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="bit">BIT码</param>
        /// <param name="message">BIT信息内容</param>
        /// <returns></returns>
        public static void UpdateDeviceBit(this IMemoryCache memory, long bit, string message = null)
        {
            var status = memory.GetDeviceStatus();
            if (status != null)
            {
                var error = status.Errors.FirstOrDefault(x => x.BitCode == bit && x.ErrorCode == ErrorCodeEnum.Bit);
                if (error == null)
                {
                    error = new ErrorInfo()
                    {
                        ErrorCode = ErrorCodeEnum.Bit,
                        BitCode = bit
                    };
                    status.Errors.Add(error);
                }
                error.ErrorTime = DateTime.Now;
                error.ErrorMsg = message;
            }
        }

        /// <summary>
        /// 更新设备服务异常信息
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="code">错误码</param>
        /// <param name="errMsg">异常信息</param>
        /// <returns></returns>
        public static void UpdateServiceError(this IMemoryCache memory, ErrorCodeEnum code, string errMsg)
        {
            var status = memory.GetDeviceStatus();
            if (status != null)
            {
                var error = status.Errors.FirstOrDefault(x => x.ErrorCode == code && x.ErrorMsg == errMsg);
                if (error == null)
                {
                    error = new ErrorInfo()
                    {
                        ErrorCode = code
                    };
                    status.Errors.Add(error);
                }
                error.ErrorTime = DateTime.Now;
                error.ErrorMsg = errMsg;
            }
        }

        /// <summary>
        /// 清理超时的异常信息
        /// 同时将异常信息写入到ErrorMsg中
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="sec">超时时间（秒）</param>
        public static void CleanErrorMsg(this IMemoryCache memory, int sec)
        {
            var status = memory.GetDeviceStatus();
            if (status != null)
            {
                status.Errors.RemoveAll(x => x.ErrorTime < DateTime.Now.AddSeconds(-1 * sec));
                status.ErrorMsg = status.Errors.Select(x =>
                {
                    if (x.BitCode > 0)
                        return $"{x.ErrorMsg}({x.BitCode})";
                    else
                        return x.ErrorMsg;
                });
            }
        }

        /// <summary>
        /// 更新设备运行状态
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static void UpdateDeviceRun(this IMemoryCache memory, DeviceStatusCode code)
        {
            var status = memory.GetDeviceStatus();
            if (status != null)
            {
                status.Code = code;
            }
        }

        /// <summary>
        /// 更新设备实时运行信息
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="az">当前方位指向</param>
        /// <param name="el">当前俯仰指向</param>
        public static void UpdateDeviceRunInfo(this IMemoryCache memory, int code, double az, double el)
        {
            var status = memory.GetDeviceStatus();
            if (status != null)
            {
                status.RunInfo.RunCode = code;
                status.RunInfo.CurrentAz = az;
                status.RunInfo.CurrentEl = el;
            }
        }

        /// <summary>
        /// 更新设备最后被引导信息
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="position">被引导位置信息</param>
        public static void UpdateDeviceGdPosition(this IMemoryCache memory, GuidancePositionInfo position)
        {
            var status = memory.GetDeviceStatus();
            if (status != null)
            {
                status.LastGdPosition = position;
            }
        }

        ///// <summary>
        ///// 更新设备服务的RPC host地址
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <param name="host"></param>
        //public static void UpdateRpcHost(this IMemoryCache memory, string host)
        //{
        //    var status = memory.GetDeviceStatus();
        //    if (status != null)
        //    {
        //        status.RpcHost = host;
        //    }
        //}

        /// <summary>
        /// 更新设备被引导信息
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="info"></param>
        public static void UpdateBeGuidanceInfo(this IMemoryCache memory, BeGuidanceTargetInfo info)
        {
            var status = memory.GetDeviceStatus();
            if (status != null)
            {
                status.BeGuidanceInfo = info;
            }
        }

        #endregion

        #region 关系

        /// <summary>
        /// 获取所有关联关系
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        public static IEnumerable<Relationships> GetRelationships(this IMemoryCache memory)
        {
            if (memory.TryGetValue(MemoryCacheKey.RelationshipsKey, out IEnumerable<Relationships> rs))
            {
                return new List<Relationships>(rs);
            }
            return new List<Relationships>();
        }

        /// <summary>
        /// 获取主动引导关系
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        public static IEnumerable<Relationships> GetRelationshipsGuidance(this IMemoryCache memory)
        {
            if (memory.TryGetValue(MemoryCacheKey.RelationshipsKey, out IEnumerable<Relationships> rs))
            {
                return new List<Relationships>(rs.Where(x => x.RType != RelationshipsType.PositionTurn));
            }
            return new List<Relationships>();
        }

        /// <summary>
        /// 获取转发关系
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        public static IEnumerable<Relationships> GetRelationshipsTurn(this IMemoryCache memory)
        {
            if (memory.TryGetValue(MemoryCacheKey.RelationshipsKey, out IEnumerable<Relationships> rs))
            {
                return new List<Relationships>(rs.Where(x => x.RType == RelationshipsType.PositionTurn));
            }

            return new List<Relationships>();
        }

        /// <summary>
        /// 刷新关系信息（重载）
        /// </summary>
        /// <param name="memory"></param>
        public static void ReloadRelationships(this IMemoryCache memory, IEnumerable<Relationships> relationships)
        {
            var dev = memory.GetDevice();
            memory.Set(MemoryCacheKey.RelationshipsKey, relationships.Where(x => x.FromDeviceId == dev?.Id || x.ToDeviceId == dev?.Id).ToList());
            //var rs = memory.GetOrCreate(MemoryCacheKey.RelationshipsKey, entrty => new List<Relationships>());
            //rs.Clear();
            //rs.AddRange(relationships.Where(x => x.FromDeviceId == dev?.Id).ToList());
            var begd = relationships.Where(x => x.ToDeviceId == dev.Id).Select(s => new BeGuidanceTargetInfo(s.TargetId, s.FromDeviceId)).FirstOrDefault();
            memory.UpdateBeGuidanceInfo(begd);
        }

        #endregion

        #region 目标

        /// <summary>
        /// 更新目标信息（新增）
        /// 目标在内存中存在30秒(滑动过期)
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="tgs">目标信息</param>
        public static async Task UpdateTarget(this IMemoryCache memory, params TargetInfo[] tgs)
        {
            foreach (var tg in tgs ?? new TargetInfo[0])
            {
                var m_tg = await memory.GetOrCreateAsync($"{ MemoryCacheKey.TargetPointsKey}.{tg.Id}", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromSeconds(30));
                    return Task.FromResult(new TargetCacheInfo());
                });
                m_tg.Update(tg);
            }
        }

        /// <summary>
        /// 根据TargetID获取航迹信息
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="tgs">目标信息</param>
        public static IEnumerable<TargetInfo> GetTracks(this IMemoryCache memory, string id)
        {
            var list = memory.Get<TargetCacheInfo>($"{ MemoryCacheKey.TargetPointsKey}.{id}")?.Points ?? new List<TargetInfo>();
            return list.OrderByDescending(o => o.TrackTime).Take(4);
        }


        /// <summary>
        /// 删除目标（目标消失）
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="tgid">消失目标ID</param>
        public static void RemoveTargte(this IMemoryCache memory, params string[] tgids)
        {
            foreach (var tgid in tgids)
            {
                if (memory.TryGetValue($"{ MemoryCacheKey.TargetPointsKey}.{tgid}", out TargetCacheInfo info))
                    info.Disappear = true;
            }
        }

        /// <summary>
        /// 目标是否存在
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public static bool TargetExistence(this IMemoryCache memory, string targetId)
        {
            if (memory.TryGetValue($"{ MemoryCacheKey.TargetPointsKey}.{targetId}", out TargetCacheInfo info))
            {
                return !info.Disappear;
            }
            return false;
        }

        /// <summary>
        /// 清理内存超时目标
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="sec">超时时间</param>
        public static IEnumerable<TargetCacheInfo> CleanTarget(this IMemoryCache memory, int sec = 0)
        {
            var distgs = memory.GetAll<TargetCacheInfo>(memory.GetTargetIds());
            if (sec > 0)
            {
                distgs = distgs.Where(x => x.Last?.TrackTime <= DateTime.Now.AddSeconds(-1 * sec) || x.Disappear);
            }
            else
            {
                distgs = distgs.Where(x => x.Disappear);
            }
            memory.RemoveAll(distgs.Select(x => $"{ MemoryCacheKey.TargetPointsKey}.{ x.Last.Id}").ToArray());
            return distgs;
        }

        /// <summary>
        /// 获取所有目标ID
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetTargetIds(this IMemoryCache memory)
        {
            return memory.SearchCacheRegex($"{ MemoryCacheKey.TargetPointsKey}.*") ?? new string[0];
        }

        /// <summary>
        /// 获取所有目标信息
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        public static IEnumerable<TargetCacheInfo> GetAllTargets(this IMemoryCache memory)
        {
            return memory.GetAll<TargetCacheInfo>(memory.GetTargetIds()).Where(x => !x.Disappear).ToList();
        }

        /// <summary>
        ///  根据目标id获取目标
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static TargetCacheInfo GetTargetById(this IMemoryCache memory, string Id)
        {
            memory.TryGetValue($"{ MemoryCacheKey.TargetPointsKey}.{Id}", out TargetCacheInfo info);
            var tg = info;
            return tg;
        }

        #endregion

        //#region 引导

        ///// <summary>
        ///// 获取设备所有引导信息
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <returns></returns>
        //public static IEnumerable<GuidanceTargetInfo> GetGuidanceTarget(this IMemoryCache memory)
        //{

        //    var status = memory.GetDeviceStatus();
        //    return status == null ? new List<GuidanceTargetInfo>() : new List<GuidanceTargetInfo>(status.GuidanceTgs);
        //    //memory.TryGetValue(MemoryCacheKey.DeviceGuidanceKey, out IEnumerable<GuidanceTargetInfo> info);
        //    //return info == null ? new List<GuidanceTargetInfo>() : new List<GuidanceTargetInfo>(info);
        //}

        ///// <summary>
        ///// 增加设备引导信息
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <param name="guidance"></param>
        ///// <returns></returns>
        //public static bool AddGuidanceTarget(this IMemoryCache memory, GuidanceTargetInfo guidance)
        //{
        //    var info = memory.GetDeviceStatus()?.GuidanceTgs;//memory.GetOrCreate(MemoryCacheKey.DeviceGuidanceKey, entity => new List<GuidanceTargetInfo>());
        //    if (info != null && !string.IsNullOrEmpty(guidance?.TargetId)/* && guidance?.ToDeviceClient != null*/)
        //    {
        //        var gd = info.FirstOrDefault(x => x.TargetId == guidance.TargetId && x.ToDeviceId == guidance.ToDeviceId);
        //        if (gd == null)
        //        {
        //            info.Add(guidance);//若之前没有该目标引导至该设备的信息，则新增此信息
        //        }
        //        //else
        //        //{
        //        //    gd.ToDeviceClient = guidance.ToDeviceClient;//若之前已经存在该目标引导至该设备的信息，则更新引导链接信息
        //        //}
        //        return true;
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 删除设备引导信息
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <param name="tgId">目标ID</param>
        ///// <param name="devId">引导设备ID</param>
        ///// <returns>存在该引导信息删除-true;不存在该引导信息-false</returns>
        //public static bool RemoveGuidanceTarget(this IMemoryCache memory, string tgId, int devId)
        //{
        //    var info = memory.GetDeviceStatus()?.GuidanceTgs;
        //    if (info != null)
        //    {
        //        return info.RemoveAll(x => x.TargetId == tgId && x.ToDeviceId == devId) > 0;
        //    }
        //    //if (memory.TryGetValue(MemoryCacheKey.DeviceGuidanceKey, out List<GuidanceTargetInfo> info))
        //    //{
        //    //    return info.RemoveAll(x => x.TargetId == tgId && x.ToDeviceId == devId) > 0;
        //    //}
        //    return false;
        //}

        ///// <summary>
        ///// 删除设备引导信息(按目标批量删除)
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <param name="tgId">目标ID</param>
        ///// <returns>存在该引导信息删除-true;不存在该引导信息-false</returns>
        //public static bool RemoveGuidanceTarget(this IMemoryCache memory, string tgId)
        //{
        //    var info = memory.GetDeviceStatus()?.GuidanceTgs;
        //    if (info != null)
        //    {
        //        return info.RemoveAll(x => x.TargetId == tgId) > 0;
        //    }
        //    //if (memory.TryGetValue(MemoryCacheKey.DeviceGuidanceKey, out List<GuidanceTargetInfo> info))
        //    //{
        //    //    return info.RemoveAll(x => x.TargetId == tgId) > 0;
        //    //}
        //    return false;
        //}

        ///// <summary>
        ///// 删除设备引导信息(按设备批量删除)
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <param name="devId">引导设备ID</param>
        ///// <returns>存在该引导信息删除-true;不存在该引导信息-false</returns>
        //public static bool RemoveGuidanceTarget(this IMemoryCache memory, int devId)
        //{
        //    var info = memory.GetDeviceStatus()?.GuidanceTgs;
        //    if (info != null)
        //    {
        //        return info.RemoveAll(x => x.ToDeviceId == devId) > 0;
        //    }
        //    //if (memory.TryGetValue(MemoryCacheKey.DeviceGuidanceKey, out List<GuidanceTargetInfo> info))
        //    //{
        //    //    return info.RemoveAll(x => x.ToDeviceId == devId) > 0;
        //    //}
        //    return false;
        //}

        //#endregion
        //#region 转发

        ///// <summary>
        ///// 获取所有转发目标信息
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <returns></returns>
        //public static IEnumerable<TurnTargetInfo> GetTurnTargets(this IMemoryCache memory)
        //{
        //    var info = memory.GetDeviceStatus()?.TurnTgs;
        //    //memory.TryGetValue(MemoryCacheKey.DeviceTurnKey, out IEnumerable<TurnTargetInfo> info);
        //    return info == null ? new List<TurnTargetInfo>() : new List<TurnTargetInfo>(info);
        //}

        ///// <summary>
        ///// 添加设备转发信息
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <param name="turn"></param>
        ///// <returns></returns>
        //public static bool AddTurnTarget(this IMemoryCache memory, TurnTargetInfo turn)
        //{
        //    //var info = memory.GetOrCreate(MemoryCacheKey.DeviceTurnKey, entity => new List<TurnTargetInfo>());
        //    var info = memory.GetDeviceStatus()?.TurnTgs;
        //    if (info != null && !string.IsNullOrEmpty(turn?.TargetId))
        //    {
        //        var gd = info.FirstOrDefault(x => x.TargetId == turn.TargetId/* && x.Ip == turn.Ip && x.Port == turn.Port*/);
        //        if (gd == null)
        //        {
        //            info.Add(turn);//若之前没有该目标引导至该设备的信息，则新增此信息
        //        }
        //        else
        //        {
        //            gd.Ip = turn.Ip;
        //            gd.Port = turn.Port;
        //        }
        //        return true;
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 删除设备转发信息
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <param name="turn"></param>
        ///// <returns></returns>
        //public static bool RemoveTurnTarget(this IMemoryCache memory, TurnTargetInfo turn)
        //{
        //    var info = memory.GetDeviceStatus()?.TurnTgs;
        //    if (info != null)
        //    {
        //        return info.RemoveAll(x => x.TargetId == turn.TargetId /*&& x.Ip == turn.Ip && x.Port == turn.Port*/) > 0;
        //    }
        //    //if (memory.TryGetValue(MemoryCacheKey.DeviceTurnKey, out List<TurnTargetInfo> info))
        //    //{
        //    //    return info.RemoveAll(x => x.TargetId == turn.TargetId && x.Ip == turn.Ip && x.Port == turn.Port) > 0;
        //    //}
        //    return false;
        //}

        //#endregion

        //#region 被引导

        ///// <summary>
        ///// 获取设备被引导信息
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <returns></returns>
        //public static BeGuidanceTargetInfo GetBeGuidanceTargetInfo(this IMemoryCache memory)
        //{
        //    return memory.GetDeviceStatus()?.BeGuidanceTg;
        //    //memory.TryGetValue(MemoryCacheKey.DeviceBeGuidanceKey, out BeGuidanceTargetInfo info);
        //    //return info;
        //}

        ///// <summary>
        ///// 移除设备被引导信息
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <returns></returns>
        //public static bool RemoveBeGuidanceTarget(this IMemoryCache memory)
        //{
        //    var begd = memory.GetBeGuidanceTargetInfo();
        //    if (begd != null)
        //    {
        //        begd.TargetId = "";
        //        begd.FromDeviceId = 0;
        //        return true;
        //    }
        //    //if (memory.TryGetValue(MemoryCacheKey.DeviceBeGuidanceKey, out BeGuidanceTargetInfo begd))
        //    //{
        //    //    begd.TargetId = "";
        //    //    begd.FromDeviceId = 0;
        //    //}
        //    return false;
        //}

        ///// <summary>
        ///// 更新设备被引导信息
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <param name="tgInfo"></param>
        ///// <returns>该被引导信息更新成功-true;该引导信息未更新至内存-false</returns>
        //public static bool UpdateBeGuidanceTarget(this IMemoryCache memory, BeGuidanceTargetInfo tgInfo)
        //{
        //    if (!string.IsNullOrEmpty(tgInfo?.TargetId) && tgInfo?.FromDeviceId > 0)
        //    {
        //        var begd = memory.GetBeGuidanceTargetInfo();
        //        if (begd != null)
        //        {
        //            begd.TargetId = tgInfo.TargetId;
        //            begd.FromDeviceId = tgInfo.FromDeviceId;
        //        }
        //        //var info = memory.GetOrCreate(MemoryCacheKey.DeviceBeGuidanceKey, entity => new BeGuidanceTargetInfo(tgInfo.TargetId, tgInfo.FromDeviceId));
        //        //info.TargetId = tgInfo.TargetId;
        //        //info.FromDeviceId = tgInfo.FromDeviceId;
        //        ////info.Invalid = tgInfo.Invalid;
        //        ////return memory.UpdateBeGuidancePosition(tgInfo.GuidanceInfo);
        //        //memory.UpdateDeviceMonitor(tgInfo.TargetId);
        //        return true;
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 更新设备被引导最后位置信息
        ///// </summary>
        ///// <param name="memory"></param>
        ///// <param name="position"></param>
        ///// <returns></returns>
        //public static bool UpdateBeGuidancePosition(this IMemoryCache memory, GuidancePositionInfo position)
        //{
        //    var info = memory.GetBeGuidanceTargetInfo();
        //    //if (memory.TryGetValue(MemoryCacheKey.DeviceBeGuidanceKey, out BeGuidanceTargetInfo info))
        //    if (info != null)
        //    {
        //        if (info.GuidanceInfo.TargetId == position?.TargetId && info.FromDeviceId == position.ProbeDevId)
        //        {
        //            info.GuidanceInfo.Alt = position.Alt;
        //            info.GuidanceInfo.Az = position.Az;
        //            info.GuidanceInfo.Dis = position.Dis;
        //            info.GuidanceInfo.El = position.El;
        //            info.GuidanceInfo.Lat = position.Lat;
        //            info.GuidanceInfo.Lng = position.Lng;
        //            info.GuidanceInfo.ProbeDevAlt = position.ProbeDevAlt;
        //            info.GuidanceInfo.ProbeDevLat = position.ProbeDevLat;
        //            info.GuidanceInfo.ProbeDevLng = position.ProbeDevLng;
        //            info.GuidanceInfo.UpdateTime = DateTime.Now;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //#endregion
    }
}
