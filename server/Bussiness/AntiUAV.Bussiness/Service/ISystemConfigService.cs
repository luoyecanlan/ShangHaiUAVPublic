using DbOrm.AntiUAV.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.Service
{
    /// <summary>
    /// 系统配置服务
    /// </summary>
    public interface ISystemConfigService : IMetadataService<SysConfig, SysConfig, SysConfigDel, SysConfigAdd>
    {
        /// <summary>
        /// 根据名称获取对应的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<SysConfig> GetConfigAsync(string name);
    }
}
