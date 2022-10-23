using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Config
{
    public class DeviceServiceConfig
    {
        private readonly IConfiguration _config;
        public DeviceServiceConfig(IConfiguration config)
        {
            _config = config.GetSection("DeviceService");
        }

        public string Path => _config.GetValue("Path", "");
        public string FileName => _config.GetValue("FileName", "");
        public string Extension => _config.GetValue("Extension", "");
    }
}
