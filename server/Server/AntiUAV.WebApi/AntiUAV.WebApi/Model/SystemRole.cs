using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Model
{
    /// <summary>
    /// 系统角色
    /// </summary>
    public class SystemRole
    {
        /// <summary>
        /// 超管角色
        /// </summary>
        public const string Super = "super";
        /// <summary>
        /// 管理员角色
        /// </summary>
        public const string Admin = "super,admin";
        /// <summary>
        /// 客户端角色
        /// </summary>
        public const string Client = "super,admin,devopt,client";
        /// <summary>
        /// 设备端角色
        /// </summary>
        public const string Device = "device";//super,admin,
        /// <summary>
        /// 设备端操作员角色
        /// </summary>
        public const string DeviceOperation = "super,admin,devopt";//,device
        /// <summary>
        /// 全部角色
        /// </summary>
        public const string All = "super,admin,client,device,devopt";
    }
}
