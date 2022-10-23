using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.WebApi.Model
{
    /// <summary>
    /// 令牌模型
    /// </summary>
    public class TokenModel
    {

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 用户ID
        /// </summary>
        public int uid { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public string rold { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 访问令牌
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 过期时长
        /// </summary>
        public int expire_in { get; set; }

        /// <summary>
        /// 授权时间
        /// </summary>
        public DateTime nbf { get; set; }
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string refresh_token { get; set; }
#pragma warning restore IDE1006 // 命名样式
    }
}
