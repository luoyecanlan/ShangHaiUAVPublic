using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Config
{
    public class PushTaskConfig
    {
        private readonly IConfiguration _config;
        public PushTaskConfig(IConfiguration config)
        {
            _config = config.GetSection("PushTask");
        }
        public int SummaryInterval => _config.GetValue<int>("SummaryInterval", 60000);
        /// <summary>
        /// 目标更新时间间隔
        /// </summary>
        public int TargetInterval => _config.GetValue<int>("TargetInterval", 1000);
        
        /// <summary>
        /// 设备状态更新时间间隔
        /// </summary>
        public int DevStatusInterval => _config.GetValue<int>("DevStatusInterval", 2000);

        /// <summary>
        /// 关联关系更新时间间隔
        /// </summary>
        public int RelationShipInterval => _config.GetValue("RelationShipInterval", 2000);
    }
}
