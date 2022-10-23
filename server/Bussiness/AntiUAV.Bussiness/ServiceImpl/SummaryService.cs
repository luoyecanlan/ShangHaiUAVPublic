using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.ServiceImpl
{
    public class SummaryService : MetadataService<SummaryInfo, SummaryInfo, SummaryKey, SummaryAdd>, ISummaryService
    {
        public SummaryService(IEntityCrudService orm) : base(orm)
        {
        }


        public async void DoSummary()
        {
            //获取上次执行统计的日期(yyyyMMdd)
            var _last = RedisHelper.GetSet(RedisCacheKeyConst.SummaryTimeKey, "");
            //var _last = RedisHelper.Get(RedisCacheKeyConst.SummaryTimeKey);
            //第一次统计前三十天的数据
            var _lastTime = string.IsNullOrEmpty(_last) ? DateTime.Now.AddDays(-30) : DateTime.Parse(_last);
            //如果上次执行时间为昨天  则不需要执行下边的代码  统计方法一天仅需要执行一次
            if (DateTime.Now.AddDays(-1).Earliest() < _lastTime.Earliest()) return;
            //获取需要进行统计的目标数据
            var start = _lastTime.Earliest();
            var end = DateTime.Now.AddDays(-1).Latest();
            var _targets = await _orm.GetAnyAsync<HistoryTgInfo>(f => f.Endtime >= start && f.Endtime <= end);
            var _days = _targets?.GroupBy(f => f.Endtime.ToString("yyyy-MM-dd")).Select(f => f.Key)?.ToList();
            if (_days?.Count > 0)
            {
                _days.ForEach(day => DoSingleDaySummary(day));
            }
            RedisHelper.Set(RedisCacheKeyConst.SummaryTimeKey, DateTime.Now, 60 * 60 * 24);
        }
        private async void DoSingleDaySummary(string day)
        {
            if (string.IsNullOrEmpty(day)) return;
            var start = day.Earliest();
            var end = day.Latest();
            var _tgs = await _orm.GetAnyAsync<HistoryTgInfo>(f => f.Endtime <= end && f.Endtime >= start);
            if (_tgs == null || _tgs.Count() == 0) return;
            //统计各项指标（类别、告警等级、航迹点数（平均，最高，最低）、持续时长（平均，最高，最低）、目标点数）  
            SummaryByCategory(_tgs, day);
            SummaryByThreat(_tgs, day);
            SummaryByKeepTime(_tgs, day);
            SummaryByTrackCount(_tgs, day);
            SummaryByCount(_tgs, day);
        }
        /// <summary>
        /// 按目标类型分类
        /// </summary>
        /// <param name="_tgs"></param>
        /// <param name="_stamp"></param>
        private void SummaryByCategory(IEnumerable<HistoryTgInfo> _tgs, string _stamp)
        {
            var _groups = _tgs.GroupBy(f => f.Category).Select(g => new { title = g.Key, value = g.Count() })?.ToList();
            _groups?.ForEach(ginfo =>
            {
                _ = AddAsync(new SummaryAdd()
                {
                    Category = 0,
                    Key = ginfo.title + "",
                    Value = ginfo.value,
                    Timestamp = _stamp,
                    Createtime = DateTime.Now
                });
            });
        }
        /// <summary>
        /// 告警等级
        /// </summary>
        /// <param name="_tgs"></param>
        /// <param name="_stamp"></param>
        private void SummaryByThreat(IEnumerable<HistoryTgInfo> _tgs, string _stamp)
        {
            var _groups = _tgs.GroupBy(f => f.ThreatLevel).Select(g => new { title = g.Key, value = g.Count() })?.ToList();
            _groups?.ForEach(ginfo =>
            {
                _ = AddAsync(new SummaryAdd()
                {
                    Category = 3,
                    Key = ginfo.title + "",
                    Value = ginfo.value,
                    Timestamp = _stamp,
                    Createtime = DateTime.Now
                });
            });
        }
        /// <summary>
        /// 持续时长
        /// </summary>
        /// <param name="_tgs"></param>
        /// <param name="_stamp"></param>
        private void SummaryByKeepTime(IEnumerable<HistoryTgInfo> _tgs, string _stamp)
        {
            //保存以天为单位的统计数据
            SaveRangData(1, "day", _stamp, _tgs.Max(f => f.KeepScounds), _tgs.Average(f => f.KeepScounds), _tgs.Min(f => f.KeepScounds));
            for (int i = 0; i < 24; i++)
            {
                var _hourData = _tgs.Where(f => f.Endtime.Hour == i);
                if (_hourData == null || _hourData.Count() == 0)
                    SaveRangData(1, i.ToString(), _stamp, 0, 0, 0);
                else
                {
                    SaveRangData(1, i.ToString(), _stamp, _hourData.Max(f => f.KeepScounds), _hourData.Average(f => f.KeepScounds), _hourData.Min(f => f.KeepScounds));
                }
            }
        }
        /// <summary>
        /// 航迹点数
        /// </summary>
        /// <param name="_tgs"></param>
        /// <param name="_stamp"></param>
        private void SummaryByTrackCount(IEnumerable<HistoryTgInfo> _tgs, string _stamp)
        {
            //保存以天为单位的统计数据
            SaveRangData(2, "day", _stamp, _tgs.Max(f => f.Count), _tgs.Average(f => f.Count), _tgs.Min(f => f.Count));
            for (int i = 0; i < 24; i++)
            {
                var _hourData = _tgs.Where(f => f.Endtime.Hour == i);
                if (_hourData == null || _hourData.Count() == 0)
                    SaveRangData(2, i.ToString(), _stamp, 0, 0, 0);
                else
                {
                    SaveRangData(2, i.ToString(), _stamp, _hourData.Max(f => f.Count), _hourData.Average(f => f.Count), _hourData.Min(f => f.Count));
                }
            }
        }
        /// <summary>
        /// 目标点数
        /// </summary>
        /// <param name="_tgs"></param>
        /// <param name="_stamp"></param>
        private void SummaryByCount(IEnumerable<HistoryTgInfo> _tgs, string _stamp)
        {
            //保存以天为单位的统计数据
            _ = AddAsync(new SummaryAdd()
            {
                Category = 4,
                Key = "day",
                Value = _tgs.Count(),
                Timestamp = _stamp,
                Createtime = DateTime.Now
            });
            for (int i = 0; i < 24; i++)
            {
                var _add = new SummaryAdd()
                {
                    Category = 4,
                    Key = i + "",
                    Timestamp = _stamp,
                    Createtime = DateTime.Now
                };
                var _hourData = _tgs.Where(f => f.Endtime.Hour == i);
                if (_hourData == null)
                    _add.Value = 0;
                else
                    _add.Value = _hourData.Count();
                _ = AddAsync(_add);
            }
        }
        /// <summary>
        /// 保存范围类型的数据
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_stamp"></param>
        /// <param name="_max"></param>
        /// <param name="_avg"></param>
        /// <param name="_min"></param>
        private void SaveRangData(int _category, string _key, string _stamp, double _max, double _avg, double _min)
        {
            //最大
            _ = AddAsync(new SummaryAdd()
            {
                Category = _category,
                Key = $"{_key}.max",
                Value = _max,
                Timestamp = _stamp,
                Createtime = DateTime.Now
            });
            //最小
            _ = AddAsync(new SummaryAdd()
            {
                Category = _category,
                Key = $"{_key}.min",
                Value = _min,
                Timestamp = _stamp,
                Createtime = DateTime.Now
            });
            //平均
            _ = AddAsync(new SummaryAdd()
            {
                Category = _category,
                Key = $"{_key}.avg",
                Value = _avg,
                Timestamp = _stamp,
                Createtime = DateTime.Now
            });
        }

        /// <summary>
        /// 统计一段时间内信息
        /// </summary>
        /// <param name="summaryCategory">统计分类</param>
        /// <param name="timeCategory">时间类型</param>
        /// <param name="isHour">单位是否为小时（持续时长、航迹点数、目标数&&time>week）</param>
        /// <returns></returns>
        public async Task<IEnumerable<SummaryInfo>> GetHistorySummary(SummaryCategory summaryCategory, SummaryTimeCategory timeCategory, bool isHour = true)
        {
            DateTime start, end;
            DateTime.Now.CalcDateRang(timeCategory, out start, out end);
            if (!isHour)
            {
                if (summaryCategory == SummaryCategory.KeepTime ||
                    summaryCategory == SummaryCategory.TrackCount ||
                    summaryCategory == SummaryCategory.TargetCount)
                    return await _orm.GetAnyAsync<SummaryInfo>(f => f.Category == (int)summaryCategory && DateTime.Parse(f.Timestamp) <= end && DateTime.Parse(f.Timestamp) >= start && f.Key.Contains("day"));
            }
            return await _orm.GetAnyAsync<SummaryInfo>(f => f.Category == (int)summaryCategory && DateTime.Parse(f.Timestamp) <= end && DateTime.Parse(f.Timestamp) >= start );
        }
    }
}
