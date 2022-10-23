using AntiUAV.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Model
{
    /// <summary>
    /// 目标位置信息
    /// </summary>
    public class SHReportModel
    {
        /// <summary>
        /// 目标ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属设备ID
        /// </summary>
        public int DeviceId { get; set; }


        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lng { get; set; }

        /// <summary>
        /// 海拔
        /// </summary>
        public double Alt { get; set; }
        /// <summary>
        /// 航向
        /// </summary>
        public string FlyDirection { get; set; }
        /// <summary>
        /// 切向速度
        /// </summary>
        public double V { get; set; }

        /// <summary>
        /// 航迹时间
        /// </summary>
        public DateTime TrackTime { get; set; }
        public double AppLng { get; set; }
        public double AppLat { get; set; }
        public double HomeLng { get; set; }
        public double HomeLat { get; set; }
        public DateTime BeginAt { get; set; }
        public string UAVType { get; set; }
        public string UAVSn { get; set; }
        public string AppAddr { get; set; }
    }
}
