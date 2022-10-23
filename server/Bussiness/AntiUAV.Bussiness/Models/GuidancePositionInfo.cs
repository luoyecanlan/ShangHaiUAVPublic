using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 引导目标位置信息
    /// </summary>
    public class GuidancePositionInfo
    {
        public GuidancePositionInfo(string tgid)
        {
            TargetId = tgid;
        }

        /// <summary>
        /// 目标ID
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// 坐标类型
        /// </summary>
        public TargetCoordinateType Coordinate { get; set; }

        /// <summary>
        /// 经度/方位
        /// </summary>
        public double TargetX { get; set; }

        /// <summary>
        /// 纬度/俯仰
        /// </summary>
        public double TargetY { get; set; }

        /// <summary>
        /// 海拔/距离
        /// </summary>
        public double TargetZ { get; set; }

        /// <summary>
        /// 航迹时间
        /// </summary>
        public DateTime TrackTime { get; set; }

        ///// <summary>
        ///// 目标ID
        ///// </summary>
        //public string TargetId { get; private set; }

        ///// <summary>
        ///// 目标纬度
        ///// </summary>
        //public double Lat { get; set; }

        ///// <summary>
        ///// 目标经度
        ///// </summary>
        //public double Lng { get; set; }

        ///// <summary>
        ///// 目标海拔
        ///// </summary>
        //public double Alt { get; set; }

        ///// <summary>
        ///// 目标探测方位
        ///// </summary>
        //public double Az { get; set; }

        ///// <summary>
        ///// 目标探测俯仰（或高度）
        ///// </summary>
        //public double El { get; set; }

        ///// <summary>
        ///// 目标距离
        ///// </summary>
        //public double Dis { get; set; }

        ///// <summary>
        ///// 探测设备ID
        ///// </summary>
        //public int ProbeDevId { get; set; }

        ///// <summary>
        ///// 探测设备纬度
        ///// </summary>
        //public double ProbeDevLat { get; set; }

        ///// <summary>
        ///// 探测设备经度
        ///// </summary>
        //public double ProbeDevLng { get; set; }

        ///// <summary>
        ///// 探测设备海拔
        ///// </summary>
        //public double ProbeDevAlt { get; set; }

        ///// <summary>
        ///// 目标点时间
        ///// </summary>
        //public long PointTime { get; set; }

        ///// <summary>
        ///// 更新时间
        ///// </summary>
        //public DateTime UpdateTime { get; set; }
    }
}
