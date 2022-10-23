using AntiUAV.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.Models
{
    public class TargetCacheInfo
    {
        public TargetCacheInfo()
        {
            Points = new List<TargetInfo>();
        }

        public TargetCacheInfo(TargetInfo info)
        {
            Last = info;
            Points = new List<TargetInfo>() { info };
        }

        public TargetInfo Last { get; private set; }

        public List<TargetInfo> Points { get; }

        public bool Disappear { get; set; }

        public void Update(TargetInfo info)
        {
            if (Last == null || info?.Id == Last.Id)
            {
                if(Last!=null)
                    info.MaxHeight = (info.Alt > Last.MaxHeight ? info.Alt : Last.MaxHeight);
                Last = info;
                Points.Add(info);
            }
        }
    }
}
