using AntiUAV.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Model
{
    /// <summary>
    /// 推送目标数据类型
    /// </summary>
    public class TargetModel : TargetPosition
    {
        ///// <summary>
        ///// 正在跟踪
        ///// </summary>
        //public bool IsTracking { get; set; }

        ///// <summary>
        ///// 正在转发
        ///// </summary>
        //public bool IsTranspond { get; set; }

        ///// <summary>
        ///// 正在打击
        ///// </summary>
        //public bool IsHitting { get; set; }

        ///// <summary>
        ///// 是否诱骗
        ///// </summary>
        //public bool IsTicking { get; set; }

        ///// <summary>
        ///// 是否监视
        ///// </summary>
        //public bool IsMonitoring { get; set; }
        
        /// <summary>
        /// 跟踪关联关系id
        /// </summary>
        public string TrackRelationShip { get; set; }

        /// <summary>
        /// 转发关联关系id
        /// </summary>
        public string TranspondRelationShip { get; set; }

        /// <summary>
        /// 打击关联关系id
        /// </summary>
        public string HitRelationShip { get; set; }

        /// <summary>
        /// 诱骗关联关系id
        /// </summary>
        public string TickRelationShip { get; set; }

        /// <summary>
        /// 监视关联关系id
        /// </summary>
        public string MonitorRelationShip { get; set; }
    }
}
