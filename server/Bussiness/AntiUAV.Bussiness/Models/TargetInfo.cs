using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 目标信息
    /// </summary>
    public class TargetInfo : TargetPosition
    {

        /// <summary>
        /// 方位
        /// </summary>
        public double ProbeAz { get; set; }

        /// <summary>
        /// 俯仰
        /// </summary>
        public double PeobeEl { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double ProbeHigh { get; set; }

        /// <summary>
        /// 距离
        /// </summary>
        public double ProbeDis { get; set; }

        /// <summary>
        /// 频点
        /// </summary>
        public double Freq { get; set; }

    }
}
