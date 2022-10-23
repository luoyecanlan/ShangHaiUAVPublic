using AntiUAV.Bussiness.Models;
using AntiUAV.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi
{
    public class RealTimeTarget
    {
        public DateTime LastTime { get; set; }
        public IEnumerable<TargetModel> Targets { get; set; } = new List<TargetModel>();
        public IEnumerable<TargetDisappear> DeleteTargets { get; set; } = new List<TargetDisappear>();
    }
}
