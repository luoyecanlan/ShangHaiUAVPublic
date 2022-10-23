using AntiUAV.Bussiness;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.Models
{
    /// <summary>
    /// 预警区信息
    /// </summary>
    public class PerWarningZoneInfo
    {
        /// <summary>
        /// 预警区ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 预警区名称
        /// </summary>
        public string Name { get; set; } // varchar(255)

        public double CircumcircleR { get; set; } // double
        public double CircumcircleRadiationR { get; set; } // double
        public double CircumcircleLat { get; set; } // double
        public double CircumcircleLng { get; set; } // double
        public double BDistance { get; set; } // double
        public double ADistance { get; set; } // double
        public double NormalDistance { get; set; } // double

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime Updatetime { get; set; } // datetime

        public string ZPoints { get; set; }

        public List<GisTool.Position> ZPointsPosition { get; set; }
    }

    /// <summary>
    /// 预警区信息集合
    /// </summary>
    public class PerWarningZoneInfoCollection
    {
        public PerWarningZoneInfoCollection()
        {
            Zones = new List<PerWarningZoneInfo>();

        }
        /// <summary>
        /// 预警区集合
        /// </summary>
        public List<PerWarningZoneInfo> Zones { get; }

        /// <summary>
        /// 本地数据更新时间
        /// </summary>
        public DateTime LocalUpdateTime { get; set; }
    }
}
