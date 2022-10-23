using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 目标位置信息
    /// </summary>
    public class TargetPosition
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
        /// 坐标类型
        /// </summary>
        public TargetCoordinateType CoordinateType { get; set; }

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
        /// 径向速度
        /// </summary>
        public double Vt { get; set; }

        /// <summary>
        /// 切向速度
        /// </summary>
        public double Vr { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// 航迹模式（0-真实/1-外推）
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// 威胁度（0-100）%
        /// </summary>
        public double Threat { get; set; }

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
        public string FlyDirection { get; set; }
        public double MaxHeight { get; set; }
        public bool GUNSStatus { get; set; }

    }
}
