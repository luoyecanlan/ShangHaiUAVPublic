using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Config
{
    public class ReportServiceConfig
    {
        private readonly IConfiguration _config;
        public ReportServiceConfig(IConfiguration config)
        {
            _config = config.GetSection("ReportServiceConfig");
        }

        
        public string ReportIp => _config.GetValue("ReportIp", "101.133.163.180");
        public int ReportPort => _config.GetValue("ReportPort", 8800);
        public string ZQCDeviceId => _config.GetValue("ZQCDeviceId", " 2000000405");
        public string ZQCPassword => _config.GetValue("ZQCPassword", "12344321");
        public string DSNDeviceId => _config.GetValue("DSNDeviceId", "2000000195");
        public string DSNPassword => _config.GetValue("DSNPassword", "12344321");
        public string ZYDeviceId => _config.GetValue("DSNDeviceId", "2000000195");
        public string ZYPassword => _config.GetValue("DSNPassword", "12344321");

        public string ServerIp => _config.GetValue("ServerIp", "http://127.0.0.1");

        public int LocalPort => _config.GetValue("LocalEndpoint", 8081);

    }
}
