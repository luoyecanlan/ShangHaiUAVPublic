using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    public class WarningZone
    {
        /// <summary>
        /// 区域类型
        /// </summary>
        public string type { get; set; }
        public string layout { get; set; }
    }

    public class CircleWarningZone : WarningZone
    {
        /// <summary>
        /// 中心点
        /// </summary>
        public double[] center { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public double radius { get; set; }
    }

    public class PolygonWarningZone : WarningZone
    {
        /// <summary>
        /// 坐标集合
        /// </summary>
        public IEnumerable<double[][]> coordinates { get; set; }
    }
}
