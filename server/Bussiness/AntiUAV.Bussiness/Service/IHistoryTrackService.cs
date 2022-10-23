using AntiUAV.Bussiness.Models;
using DbOrm.AntiUAV.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.Service
{
    public interface IHistoryTrackService : IMetadataService<HistoryTrackInfo, HistoryTrackInfo, HistoryTrackDel, HistoryTrackAdd>
    {
        /// <summary>
        /// 获取目标轨迹集合
        /// </summary>
        /// <param name="tgId"></param>
        /// <returns></returns>
        Task<IEnumerable<HistoryTrkInfo>> GetTracks(string tgId);

        /// <summary>
        /// 增加打击标记
        /// </summary>
        /// <param name="tgid"></param>
        /// <returns></returns>
        Task AddHitMark(string tgid);

        /// <summary>
        /// 移除打击标记
        /// </summary>
        /// <param name="tgid"></param>
        /// <returns></returns>
        Task<bool> RemoveHitMark(string tgid);
    }
}
