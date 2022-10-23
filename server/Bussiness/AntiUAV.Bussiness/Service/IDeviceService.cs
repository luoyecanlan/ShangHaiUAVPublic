using AntiUAV.Bussiness.Models;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.Service
{
    /// <summary>
    /// 设备服务业务
    /// </summary>
    public interface IDeviceService : IMetadataService<DeviceInfo, DeviceUpdate, DeviceDel, DeviceAdd>
    {
        /// <summary>
        /// 获取设备分类信息
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DeviceCategoryInfo>> GetCategory();

        /// <summary>
        /// 更新设备位置信息
        /// </summary>
        /// <param name="device">可更新的设备信息</param>
        /// <returns>设备信息</returns>
        Task<DeviceInfo> Update(DeviceUpdatePosition position);

        /// <summary>
        /// 获取设备状态
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        Task<IEnumerable<DeviceStatus>> GetStatus(params int[] devId);
        /// <summary>
        /// 获取全部设备状态
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DeviceStatus>> GetStatus();

        /// <summary>
        /// 获取单一设备状态
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        Task<DeviceStatus> GetStatusOne(int devId);

        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        bool UpdateStatus(DeviceStatus status, int time);

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="category"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        Task<bool> RegistHost(int deviceId, int category, string host);

        /// <summary>
        /// 获取服务地址
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<DeviceServiceInfo> GetHost(int deviceId);

        /// <summary>
        /// 获取所有在线的探测设备服务地址
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DeviceServiceInfo>> GetProbeHost();

        /// <summary>
        /// 根据目标ID获取最近的可用设备
        /// </summary>
        /// <param name="tgid">目标id</param>
        /// <param name="func">设备类型筛选</param>
        /// <returns></returns>
        Task<DeviceInfo> GetBastDevice(string tgId, Func<DeviceInfo, bool> predicate,RelationshipsType type);

        /// <summary>
        /// 获取被当前目标引导的设备信息集合
        /// </summary>
        /// <param name="tgId"></param>
        /// <returns></returns>
        Task<IEnumerable<DeviceInfo>> GetBeGuidanceDevice(string tgId);

        /// <summary>
        /// 增加一个关联关系
        /// </summary>
        /// <param name="relationships"></param>
        /// <returns></returns>
        Task<bool> AddRelationships(Relationships relationships);

        /// <summary>
        /// 移除一个关联关系
        /// </summary>
        /// <param name="tgId">目标ID</param>
        /// <param name="type">关联类型</param>
        /// <returns></returns>
        Task<bool> RemoveRelationships(string tgId, RelationshipsType type);

        /// <summary>
        /// 移除一个关联关系
        /// </summary>
        /// <param name="rid">目标ID</param>
        /// <returns></returns>
        Task<bool> RemoveRelationships(string rid);

        /// <summary>
        /// 移除一个（多个）关联关系
        /// 与该目标相关的关联关系都会移除
        /// </summary>
        /// <param name="tgId"></param>
        /// <returns></returns>
        Task<bool> RemoveRelationships(Func<Relationships, bool> predicate = null);

        /// <summary>
        /// 获取关联关系
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IEnumerable<Relationships>> GetRelationships(Func<Relationships, bool> predicate = null);

        /// <summary>
        /// 设备是否在线
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        bool? IsOnline(int devid);
    }
}
