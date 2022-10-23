using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Model
{
    /// <summary>
    /// 关联关系模型
    /// </summary>
    public class Relation
    {
        /// <summary>
        /// 来源设备id
        /// </summary>
        public int From { get; set; }
        /// <summary>
        /// 目标id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 关联到（设备）服务地址
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// 关系类型
        /// </summary>
        public RelationCategory Category { get; set; }
        /// <summary>
        /// 引导信息是否被验证
        /// 需要与被引导设备上的被引导信息进行比较
        /// </summary>
        public bool IsValidation { get; set; }
    }

    /// <summary>
    /// 关系类型
    /// </summary>
    public enum RelationCategory
    {
        /// <summary>
        /// 转发
        /// </summary>
        Transpond = 0,
        /// <summary>
        /// 跟踪
        /// </summary>
        Tracking,
        /// <summary>
        /// 打击
        /// </summary>
        Hitting,
        /// <summary>
        /// 诱骗
        /// </summary>
        Ticking,
        /// <summary>
        /// 监视
        /// </summary>
        Monitoring
    }
}
