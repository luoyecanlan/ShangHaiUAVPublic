using AntiUAV.Bussiness.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.NoticeModels
{
    /// <summary>
    /// 关联关系通知消息
    /// </summary>
    public class RelationNotice
    {
        [JsonConstructor]
        public RelationNotice(string id)
        {
            RId = id;
            Code = RelationNoticeCode.Remove;
        }

        public RelationNotice(Relationships relationships)
        {
            RId = relationships?.Id;
            Relationships = relationships;
            Code = RelationNoticeCode.Add;
        }

        /// <summary>
        /// 关系ID
        /// </summary>
        public string RId { get; set; }
        /// <summary>
        /// 关联关系
        /// </summary>
        public Relationships Relationships { get; set; }
        /// <summary>
        /// 通知类型
        /// </summary>
        public RelationNoticeCode Code { get; set; }
    }

    public enum RelationNoticeCode
    {
        /// <summary>
        /// 新增关联关系
        /// </summary>
        Add,
        /// <summary>
        /// 移除关联关系
        /// </summary>
        Remove
    }
}
