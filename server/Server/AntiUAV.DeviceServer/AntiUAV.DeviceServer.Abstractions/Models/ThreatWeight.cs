using AntiUAV.DeviceServer.Abstractions.PluginService;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.Models
{
    /// <summary>
    /// 威胁判定权重
    /// </summary>
    public class ThreatWeight : IThreatWeight
    {
        /// <summary>
        /// 航迹连续时间权重
        /// </summary>
        public virtual double Weight4ConTime { get; set; } = 0.3;
        /// <summary>
        /// 平均航速权重
        /// </summary>
        public virtual double Weight4AvgVecolity { get; set; } = 0.15;
        /// <summary>
        /// 距离权重
        /// </summary>
        public virtual double Weight4Distance { get; set; } = 0.3;
        /// <summary>
        /// 瞬时速度权重
        /// </summary>
        public virtual double Weight4InstantVecolity { get; set; } = 0.15;
        /// <summary>
        /// 航迹质量权重
        /// </summary>
        public virtual double Weight4TackQuality { get; set; } = 0.1;
        /// <summary>
        /// 航迹连续时间A系数 A*lnT+B
        /// </summary>
        public virtual double AConTime { get; set; } = 1;
        /// <summary>
        /// 航迹连续时间B系数 A*lnT+B
        /// </summary>
        public virtual double BConTime { get; set; } = 0.5;
        /// <summary>
        /// 平均速度A系数 A*lnVavg
        /// </summary>
        public virtual double AAvgVecolity { get; set; } = 1;
        /// <summary>
        /// 距离A系数 A/x+B
        /// </summary>
        //public virtual double ADistance { get; set; } = 3000;//该值取预警区中心到边界的距离（即蓝色告警区）
        /// <summary>
        /// 距离B系数 A/x+B
        /// </summary>
        //public virtual double BDistance { get; set; } = 3;//该值取蓝色告警区除以红色告警区边界
        /// <summary>
        /// 瞬时速度A系数 A*lnVi
        /// </summary>
        public virtual double AInstantVecolity { get; set; } = 1;
        /// <summary>
        /// 航迹连续时间归一化系数
        /// </summary>
        public virtual double NormalContime { get; set; } = 6.71;
        /// <summary>
        /// 平均速度归一化系数
        /// </summary>
        public virtual double NormalAvgVecolity { get; set; } = 3.4;
        /// <summary>
        /// 距离归一化系数
        /// </summary>
        //public virtual double NormalDistance { get; set; } = 300;
        /// <summary>
        /// 瞬时归一化系数
        /// </summary>
        public virtual double NormalInstantVecolity { get; set; } = 3.4;
    }
}
