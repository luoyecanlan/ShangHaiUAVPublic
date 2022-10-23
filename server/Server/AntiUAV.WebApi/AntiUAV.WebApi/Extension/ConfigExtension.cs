using AntiUAV.WebApi.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.WebApi
{
    public static class ConfigExtension
    {
        public static IServiceCollection AddConfigService(this IServiceCollection services, IConfiguration config)
        {
            return services.AddSingleton(config)
                    .AddScoped<AllConfigModel>()
                    .AddScoped<AuthConfig>()
                    .AddScoped<UserConfig>()
                    .AddSingleton<PushTaskConfig>()
                    .AddSingleton<DeviceServiceConfig>()
                    .AddSingleton<ReportServiceConfig>()
                    .AddSingleton<BackupServiceConfig>();
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string Get32MD5(this string pwd)
        {
            if (string.IsNullOrEmpty(pwd)) return pwd;
            using MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            string hash = sBuilder.ToString();
            return hash.ToUpper();
        }
    }
}
