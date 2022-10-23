using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using DbOrm.AntiUAV.Entity;
using System.Linq;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.ServiceImpl
{
    /// <summary>
    /// 历史目标服务
    /// </summary>
    public class HistoryTargetService : MetadataService<HistoryTargetInfo, HistoryTargetUpdate, HistoryTargetDel, HistoryTargetAdd>, IHistoryTargetService
    {
        public HistoryTargetService(IEntityCrudService orm) : base(orm)
        {
        }

        public Task<bool> BackupAndClear()
        {
            return Task.FromResult(true);
        }
        public Task<string> ExportData(string[] tgids)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<HistoryTargetInfo>> GetRiverAsync(SeachConditions conditions)
        {
            if (conditions == null)
                throw new BussinessException(BussinessExceptionCode.ParamNull);
          
            var _targets = await _orm.GetAnyAsync<HistoryTgInfo>();
            //开始结束时间
            if (conditions.Start != null && conditions.End != null && conditions.End > conditions.Start)
            {
                _targets = _targets.Where(f => f.Endtime <= conditions.End && f.Starttime >= conditions.Start);
            }

            return _targets;


        }
        public async Task<IEnumerable<HistoryTrackInfo>> GetHeatMapHisIdsAsync(SeachConditions conditions)
        {
            if (conditions == null)
                throw new BussinessException(BussinessExceptionCode.ParamNull);
            //var _targets = await _orm.GetAnyAsync<HistoryTgInfo>();
            
            ////开始结束时间
            //if (conditions.Start != null && conditions.End != null && conditions.End > conditions.Start)
            //{
            //    _targets = _targets.Where(f => f.Endtime <= conditions.End && f.Starttime >= conditions.Start);
            //}

           
            var _track = await _orm.GetAnyAsync<HistoryTrackInfo>();
            //开始结束时间
            if (conditions.Start != null && conditions.End != null && conditions.End > conditions.Start)
            {
                _track = _track.Where(f => f.TrackTime <= conditions.End && f.TrackTime >= conditions.Start);
            }

            return _track;

            
        }



        public async Task<IEnumerable<int>> GetHisAnyIdsAsync(SeachConditions conditions)
        {
            if (conditions == null)
                throw new BussinessException(BussinessExceptionCode.ParamNull);
            var _targets = await _orm.GetAnyAsync<HistoryTgInfo>();
            if (_targets == null || _targets.Count() == 0)
                return new List<int>();
            //关键字
            if (!string.IsNullOrEmpty(conditions.Key))
            {
                //_targets = _targets.Where(f => f.TgId.Contains(conditions.Key));
                _targets = _targets.Where(f => f.Sn?.ToUpper().Contains(conditions.Key.ToUpper()) ?? false);
            }
            //开始结束时间
            if (conditions.Start != null && conditions.End != null && conditions.End > conditions.Start)
            {
                _targets = _targets.Where(f => f.Endtime <= conditions.End && f.Starttime >= conditions.Start);
            }
            //类型（0:全部；）
            if (conditions.Category > 0)
            {
                _targets = _targets.Where(f => f.Category == conditions.Category);
            }
            //威胁等级(-1：全部；【0-3】)
            //if (conditions.Threat > -1)
            //{
            //    _targets = _targets.Where(f => f.ThreatMax == conditions.Threat);
            //}
            //无人机型号
            if (conditions.UAVModel >= 0)
            {
                _targets = _targets.Where(f => f.UAVModel == conditions.UAVModel);
            }
            return _targets.OrderByDescending(f => f.Endtime).Select(x => x.Id).ToList();
        }

        public async Task<PagingModel<HistoryTgInfo>> GetHisAnyAsync(int size, int index, SeachConditions conditions)
        {
            var _targets = await _orm.GetAnyAsync<HistoryTgInfo>();
            if (_targets == null || _targets.Count() == 0)
                return new PagingModel<HistoryTgInfo>(index, size)
                {
                    SnumSize = 0,
                    Data = _targets.OrderByDescending(f => f.Endtime).Skip((index - 1) * size).Take(size)
                };
            //关键字
            if (!string.IsNullOrEmpty(conditions.Key))
            {
                //_targets = _targets.Where(f => f.TgId.Contains(conditions.Key));
                _targets = _targets.Where(f => f.Sn?.ToUpper().Contains(conditions.Key.ToUpper()) ?? false);
            }
            //开始结束时间
            if (conditions.Start != null && conditions.End != null && conditions.End > conditions.Start)
            {
                _targets = _targets.Where(f => f.Endtime <= conditions.End && f.Starttime >= conditions.Start);
            }
            //类型（0:全部；）
            if (conditions.Category > 0)
            {
                _targets = _targets.Where(f => f.Category == conditions.Category);
            }
            //威胁等级(-1：全部；【0-3】)
            //if (conditions.Threat > -1)
            //{
            //    _targets = _targets.Where(f => f.ThreatMax == conditions.Threat);
            //}
            //无人机型号
            if (conditions.UAVModel >= 0)
            {
                _targets = _targets.Where(f => f.UAVModel == conditions.UAVModel);
            }
            return new PagingModel<HistoryTgInfo>(index, size)
            {
                SnumSize = _targets.Count(),
                Data = _targets.OrderByDescending(f => f.Endtime).Skip((index - 1) * size).Take(size)
            };
        }
    }
}
