using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.WebApi.Model
{
    /// <summary>
    /// 用户修改密码模型
    /// </summary>
    public class UserPwdUpdate
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPwd { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPwd { get; set; }
    }
}
