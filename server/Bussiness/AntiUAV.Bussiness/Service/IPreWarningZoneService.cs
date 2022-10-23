using AntiUAV.Bussiness.Models;
using DbOrm.AntiUAV.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.Service
{
    /// <summary>
    /// 预警区服务业务
    /// </summary>
    public interface IPreWarningZoneService : IMetadataService<PerWarningZoneInfo, PerWarningZoneUpdate, PerWarningZoneDel, PerWarningZoneAdd>
    {
        /// <summary>
        /// 更新预警区几何图形
        /// </summary>
        /// <param name="zone">预警区几何图形</param>
        /// <returns></returns>
        Task<PerWarningZoneInfo> UpdateGeoAsync(PerWarningZoneGeoUpdate zone);
    }
}
