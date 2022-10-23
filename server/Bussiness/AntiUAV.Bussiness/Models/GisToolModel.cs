using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AntiUAV.Bussiness.GisTool;

namespace AntiUAV.Bussiness
{
    public class GisToolModel
    {
        /// <summary>
        /// 目标坐标信息
        /// </summary>
        public Position Target { get; set; }
        /// <summary>
        /// 原点坐标西悉尼
        /// </summary>
        public Position Center { get; set; }
    }

    public class PositionModel
    {
        /// <summary>
        /// 原点坐标信息
        /// </summary>
        public Position Center { get; set; }
        /// <summary>
        /// 方位
        /// </summary>
        public double AZ { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public double Dis { get; set; }
        /// <summary>
        /// 俯仰
        /// </summary>
        public double Pitch { get; set; }
        /// <summary>
        /// 高程
        /// </summary>
        public double Alt { get; set; }
    }


}
