using AntiUAV.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.Service
{
    /// <summary>
    /// 目标业务服务
    /// </summary>
    public interface ITargetService
    {
        /// <summary>
        /// 获取目标最后更新位置信息
        /// </summary>
        /// <param name="devId">设备ID</param>
        /// <param name="timeStamp">更新时间戳(为空则获取全部数据)</param>
        /// <returns></returns>
        Task<IEnumerable<TargetPosition>> GetLastUpdateTargetsPositionAsync(int devId, DateTime? timeStamp = null);

        /// <summary>
        /// 获取近期消失目标
        /// </summary>
        /// <param name="devId">设备ID</param>
        /// <returns></returns>
        Task<IEnumerable<TargetDisappear>> GetLastDisappearTargets(int devId);

        /// <summary>
        /// 获取最后更新目标信息
        /// </summary>
        /// <param name="tgId">目标ID</param>
        /// <param name="tgId">设备ID</param>
        /// <returns></returns>
        Task<TargetInfo> GetLastUpdateTargetInfo(string tgId, int devId);

        /// <summary>
        /// 获取超时目标
        /// </summary>
        /// <param name="cycleCount">超时周期</param>
        /// <returns></returns>
        //IEnumerable<string> GetTimeOutTarget(double cycleCount);

        ///// <summary>
        ///// 更新目标信息
        ///// </summary>
        ///// <param name="devId">设备ID</param>
        ///// <param name="probe">探测目标信息</param>
        ///// <returns></returns>
        //Task UpdateTargets(int devId, params TargetInfo[] probe);

        ///// <summary>
        ///// 目标消失
        ///// </summary>
        ///// <param name="tgIds">消失目标ID</param>
        ///// <returns></returns>
        //Task TargetDisappear(params TargetDisappear[] tgs);

        ///// <summary>
        ///// 消失目标数据保存
        ///// </summary>
        ///// <returns></returns>
        //Task DisappearTargetsSave();

        //历史目标概况30天

        //获取某时间段内的历史目标

        //获取历史目标航迹

        //清理历史目标和航迹


        /// <summary>
        /// 更新设备目标信息
        /// 1.消失超过30秒的目标从消失集合中移除
        /// 2.目标自销毁周期是20s（期间无更新则自动销毁）
        /// </summary>
        /// <param name="devId">设备ID</param>
        /// <param name="targets">更新的目标信息</param>
        /// <param name="distgs">消失的目标ID</param>
        /// <returns></returns>
        Task UpdateTargetsInfo(int devId, IEnumerable<TargetInfo> tgs, IEnumerable<string> distgs);

        /// <summary>
        /// 目标是否存在
        /// </summary>
        /// <param name="tgId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        bool TargetExistence(string tgId, int deviceId);

        /// <summary>
        /// 更新正在追踪的目标信息
        /// </summary>
        /// <param name="devId">目标来源设备ID</param>
        /// <param name="tg">更新的目标信息</param>
        /// <returns></returns>
        Task UpdateTargetInfo(int devId, TargetInfo tg);

        /// <summary>
        /// 消失目标信息保存
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        Task DisappearTgsSave(int devId);
    }
}
