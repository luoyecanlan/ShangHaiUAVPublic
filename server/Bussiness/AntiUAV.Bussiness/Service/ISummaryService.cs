using AntiUAV.Bussiness.Models;
using DbOrm.AntiUAV.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.Service
{
    public interface ISummaryService : IMetadataService<SummaryInfo, SummaryInfo, SummaryKey, SummaryAdd>
    {
        /// <summary>
        /// 获取固定时间段内的数据统计信息
        /// </summary>
        /// <param name="category">统计类型</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        Task<IEnumerable<SummaryInfo>> GetHistorySummary(SummaryCategory summaryCategory, SummaryTimeCategory timeCategory, bool isHour = true);
        /// <summary>
        /// 定时统计信息(每天执行)
        /// </summary>
        /// <returns></returns>
        void DoSummary();
    }
}
