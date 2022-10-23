using AntiUAV.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.Service
{
    /// <summary>
    /// 服务综合操作
    /// </summary>
    public interface IServiceOpt
    {
        /// <summary>
        /// 初始化缓存
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<bool> InitializationCacheAsync(int deviceId);

        /// <summary>
        /// 重新加载预警区
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<bool> ReloadDeviceZoneAsync(int deviceId);

        /// <summary>
        /// 重新加载设备信息
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<bool> ReloadDeviceInfoAsync(int deviceId);

        /// <summary>
        /// 引导设备
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Task Guidance(GuidancePositionInfo position);

        /// <summary>
        /// 设置纠偏信息
        /// </summary>
        /// <param name="timeout">超时时间（默认3秒）</param>
        /// <returns></returns>
        Task<bool> SetRectify(int timeout = 3000);

        /// <summary>
        /// 设置位置信息
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<bool> SetPosition(int timeout = 3000);
        /// <summary>
        /// 设置设备操作信息
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<bool> SetOperationDevice(string json, int timeout = 3000);

        /// <summary>
        /// 设置运行模式（雷达、光电、干扰）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mode">运行模式</param>
        /// <param name="timeout">超时时间（默认3秒）</param>
        /// <returns></returns>
        Task<bool> SetRunMode(string mode, int timeout = 3000);

        /// <summary>
        /// 跟踪指令（雷达、光电）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">跟踪参数</param>
        /// <param name="sw">跟踪开关</param>
        /// <param name="timeout">超时时间（默认3秒）</param>
        /// <returns></returns>
        Task<bool> SetMointor(string json, bool sw, int timeout = 3000);

        /// <summary>
        /// 攻击指令（反制）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">反制参数</param>
        /// <param name="sw">反制开关</param>
        /// <param name="timeout">超时时间（默认3秒）</param>
        /// <returns></returns>
        Task<bool> SetAttack(string json, bool sw, int timeout = 3000);
    }
}
