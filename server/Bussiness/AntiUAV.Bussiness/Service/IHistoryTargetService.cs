using AntiUAV.Bussiness.Models;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.Service
{
    /// <summary>
    /// 历史目标服务
    /// </summary>
    public interface IHistoryTargetService : IMetadataService<HistoryTargetInfo, HistoryTargetUpdate, HistoryTargetDel, HistoryTargetAdd>
    {
        Task<IEnumerable<HistoryTargetInfo>> GetRiverAsync(SeachConditions conditions);
        Task<IEnumerable<HistoryTrackInfo>> GetHeatMapHisIdsAsync(SeachConditions conditions);

        /// <summary>
        /// 筛选历史目标数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        Task<PagingModel<HistoryTgInfo>> GetHisAnyAsync(int index, int size, SeachConditions conditions);

        /// <summary>
        /// 获取满足条件的所有历史目标数据ID
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        Task<IEnumerable<int>> GetHisAnyIdsAsync(SeachConditions conditions);

        /// <summary>
        /// 备份同时清理（系统自动执行）
        /// </summary>
        /// <returns></returns>
        Task<bool> BackupAndClear();
        /// <summary>
        /// 导出目标数据到csv文件中
        /// </summary>
        /// <param name="tgids"></param>
        /// <returns></returns>
        Task<string> ExportData(string[] tgids);
        //比较特定数据

    }
}
