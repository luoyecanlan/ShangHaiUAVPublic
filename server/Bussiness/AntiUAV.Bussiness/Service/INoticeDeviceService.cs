using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.NoticeModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.Service
{
    /// <summary>
    /// 设备服务通知服务
    /// </summary>
    public interface INoticeDeviceService
    {
        /// <summary>
        /// 通知设备信息变化
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task NoticeDeviceChange(int devid, DeviceInfoNoticeCode code);

        /// <summary>
        /// 通知预警区信息变化
        /// </summary>
        /// <param name="zoneid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task NoticePerZonesChange(int zoneid, PerZonesNoticeCode code);

        /// <summary>
        /// 通知消除关系变化
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        Task NoticeRelationRemove(string rid);

        /// <summary>
        /// 通知新增关系变化
        /// </summary>
        /// <param name="relationships"></param>
        /// <returns></returns>
        Task NoticeRelationAdd(Relationships relationships);
        Task NoticeRpcRequest(RpcRequestModel rpcRequestModel);
    }
}
