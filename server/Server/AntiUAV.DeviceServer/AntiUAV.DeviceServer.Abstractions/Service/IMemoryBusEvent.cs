using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.Service
{
    /// <summary>
    /// 内存事件总线
    /// </summary>
    public interface IMemoryBusEvent
    {
        /// <summary>
        /// 注册内存事件
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="key">事件唯一ID</param>
        /// <param name="millsec">超时时间（默认3000ms）</param>
        /// <returns></returns>
        Task<R> RegistEvent<R>(object key, int millsec);

        /// <summary>
        /// 响应内存事件
        /// </summary>
        /// <param name="key">事件唯一ID</param>
        /// <param name="data">响应数据</param>
        /// <returns></returns>
        Task<bool> ResoponseEvent(object key, object data);
    }
}
