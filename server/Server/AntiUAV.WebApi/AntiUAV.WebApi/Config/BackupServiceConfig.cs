using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Config
{
    public class BackupServiceConfig
    {
        private readonly IConfiguration _config;
        public BackupServiceConfig(IConfiguration config)
        {
            _config = config.GetSection("BackupConfig");
        }

        public string BaseDictory => _config.GetValue("Path", System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backup"));
        public int BackupInterval => _config.GetValue("Interval", 60 * 1000);
        public int BackupLastDays => _config.GetValue("Days", 15);
    }
}
