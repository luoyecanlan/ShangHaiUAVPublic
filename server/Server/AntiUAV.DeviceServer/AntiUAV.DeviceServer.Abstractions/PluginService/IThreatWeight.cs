using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.PluginService
{
    /// <summary>
    /// 威胁判定权重
    /// </summary>
    public interface IThreatWeight
    {
        /// <summary>
        /// 航迹连续时间权重
        /// </summary>
        double Weight4ConTime { get; set; }
        /// <summary>
        /// 平均航速权重
        /// </summary>
        double Weight4AvgVecolity { get; set; }
        /// <summary>
        /// 距离权重
        /// </summary>
        double Weight4Distance { get; set; } 
        /// <summary>
        /// 瞬时速度权重
        /// </summary>
        double Weight4InstantVecolity { get; set; } 
        /// <summary>
        /// 航迹质量权重
        /// </summary>
        double Weight4TackQuality { get; set; }
        /// <summary>
        /// 航迹连续时间A系数 A*lnT+B
        /// </summary>
        double AConTime { get; set; } 
        /// <summary>
        /// 航迹连续时间B系数 A*lnT+B
        /// </summary>
        double BConTime { get; set; } 
        /// <summary>
        /// 平均速度A系数 A*lnVavg
        /// </summary>
        double AAvgVecolity { get; set; }
        /// <summary>
        /// 距离A系数 A/x+B
        /// </summary>
        // double ADistance { get; set; } = 3000;//该值取预警区中心到边界的距离（即蓝色告警区）
        /// <summary>
        /// 距离B系数 A/x+B
        /// </summary>
        // double BDistance { get; set; } = 3;//该值取蓝色告警区除以红色告警区边界
        /// <summary>
        /// 瞬时速度A系数 A*lnVi
        /// </summary>
        double AInstantVecolity { get; set; }
        /// <summary>
        /// 航迹连续时间归一化系数
        /// </summary>
        double NormalContime { get; set; }
        /// <summary>
        /// 平均速度归一化系数
        /// </summary>
        double NormalAvgVecolity { get; set; }
        /// <summary>
        /// 距离归一化系数
        /// </summary>
        //public virtual double NormalDistance { get; set; }
        /// <summary>
        /// 瞬时归一化系数
        /// </summary>
        double NormalInstantVecolity { get; set; } 
    }
}
