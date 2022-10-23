using DbOrm.AntiUAV.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 目标描述
    /// </summary>
    public struct TargetDescription
    {
        public HistoryTargetAdd Target { get; set; }
        public IEnumerable<HistoryTrackAdd> TrackList { get; set; }
    }
}
