using DbOrm.AntiUAV.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    public class HistoryTgInfo : HistoryTargetInfo
    {
        /// <summary>
        /// 目标持续时长（秒）
        /// </summary>
        public double KeepScounds
        {
            get
            {
                return (Endtime - Starttime).TotalSeconds;
            }
        }

        /// <summary>
        /// 告警等级
        /// </summary>
        public int ThreatLevel
        {
            get
            {
                return ThreatMax.CalcThreat();
            }
        }

    }

    public class HistoryBackup : HistoryTgInfo
    {
        /// <summary>
        /// 轨迹信息
        /// </summary>
        public IEnumerable<HistoryTrackInfo> TrackData { get; set; }
    }

    public class HistoryTrkInfo : HistoryTrackInfo
    {
        /// <summary>
        /// 顺序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 轨迹时间（第几秒）
        /// </summary>
        public double RightScound { get; set; }
    }

    /// <summary>
    /// 历史目标统计
    /// </summary>
    public class HistoryTargetSummary
    {
        public HistoryTargetSummary() { }
        public HistoryTargetSummary(int index, string title, SummaryCategory category, double val, string date)
        {
            Index = index;
            Title = title;
            Category = category;
            Value = val;
            Date = date;
        }
        public int Index { get; set; }
        public string Title { get; set; }
        public SummaryCategory Category { get; set; }
        public double Value { get; set; }
        public string Date { get; set; }
    }

    /// <summary>
    /// 历史目标统计（含最大、最小值）
    /// </summary>
    public class HistoryTargetRangSummary : HistoryTargetSummary
    {
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
    }

    /// <summary>
    /// 统计项
    /// </summary>
    public enum SummaryCategory
    {
        /// <summary>
        /// 目标类型
        /// </summary>
        TargetCategory = 0,
        /// <summary>
        /// 目标持续时长
        /// </summary>
        KeepTime,
        /// <summary>
        /// 目标轨迹点数
        /// </summary>
        TrackCount,
        /// <summary>
        /// 威胁等级
        /// </summary>
        Threat,
        /// <summary>
        /// 目标数
        /// </summary>
        TargetCount
    }

    /// <summary>
    /// 统计时间类型 >> 0:day;1:week;2:month;3:year
    /// </summary>----              
    public enum SummaryTimeCategory
    {        
        /// <summary>
        /// 上一天  单位小时
        /// </summary>
        Day=0,
        /// <summary>
        /// 上周  单位天
        /// </summary>
        Week,
        /// <summary>
        /// 上月  单位天
        /// </summary>
        Month,
        /// <summary>
        /// 上季度 单位月
        /// </summary>
        //Season,
        /// <summary>
        /// 上一年 单位月
        /// </summary>
        Year
    }

    /// <summary>
    /// 历史目标检索条件
    /// </summary>
    public class SeachConditions
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Start { get; set; } 
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// 目标类型（0：全部；）
        /// </summary>
        public int Category { get; set; } = 0;
        /// <summary>
        /// 威胁等级(-1:全部；[0-3])
        /// </summary>
        public int Threat { get; set; } = -1;
        /// <summary>
        /// 是否降序（按目标最后出现时间进行排序）
        /// </summary>
        public bool Desc { get; set; }
        /// <summary>
        /// 无人机型号
        /// </summary>
        public int UAVModel { get; set; }
    }
}
