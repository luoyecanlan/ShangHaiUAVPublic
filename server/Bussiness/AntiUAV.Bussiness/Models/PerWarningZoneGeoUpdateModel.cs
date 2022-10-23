using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 预警区集合图形更新模型
    /// </summary>
    public class PerWarningZoneGeoUpdateModel
    {
        /// <summary>
        /// 预警区ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 几何图形
        /// </summary>
        public string ZonePoints { get; set; }
    }
}
