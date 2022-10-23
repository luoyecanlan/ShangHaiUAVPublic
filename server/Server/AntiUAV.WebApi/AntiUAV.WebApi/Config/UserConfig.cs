using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Config
{
    public class UserConfig
    {
        private readonly IConfigurationSection _configSection;

        public UserConfig(IConfiguration configuration)
        {
            _configSection = configuration.GetSection("User");
        }


        /// <summary>
        /// 默认密码
        /// </summary>
        public string DefaultPwd => _configSection.GetValue("DefaultPwd", "999999");
        /// <summary>
        /// 新用户默认密码
        /// </summary>
        public string NewPwd => _configSection.GetValue("NewPwd", "000000");

    }
}
