using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Config
{
    public class AuthConfig
    {
        private readonly IConfigurationSection _configSection;

        public AuthConfig(IConfiguration configuration)
        {
            _configSection = configuration.GetSection("JwtAuth");
        }


        /// <summary>
        /// 安全密钥
        /// </summary>
        public string SecurityKey => _configSection.GetValue("SecurityKey", "antiuav.webapi.server- by 18611198909_coderLiu");

        /// <summary>
        /// 颁布者
        /// </summary>
        public string Issuer => _configSection.GetValue("Issuer", "antiuav.webapi.server");

        /// <summary>
        /// 受众
        /// </summary>
        public string Audience => _configSection.GetValue("Audience", "LADS_client");

        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        public int ExpireTime => _configSection.GetValue("ExpireTime", 60 * 60);

        /// <summary>
        /// 时钟偏差
        /// </summary>
        public int ClockSkew => _configSection.GetValue("ClockSkew", 60 * 5);

        public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
        {
            var configSection = configuration.GetSection("JwtAuth");
            return new TokenValidationParameters
            {
                ValidateIssuer = true,//是否验证Issuer
                ValidateAudience = true,//是否验证Audience
                ValidateLifetime = true,//是否验证失效时间
                ClockSkew = TimeSpan.FromSeconds(configSection.GetValue("ClockSkew", 5 * 60)),
                ValidateIssuerSigningKey = true,//是否验证SecurityKey
                ValidAudience = configSection.GetValue("Audience", "LADS_client"),//Audience
                ValidIssuer = configSection.GetValue("Issuer", "antiuav.webapi.server"),//Issuer，这两项和前面签发jwt的设置一致
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configSection.GetValue("SecurityKey", "antiuav.webapi.server- by 18611198909_coderLiu")))//拿到SecurityKey
            };
        }
    }
}
