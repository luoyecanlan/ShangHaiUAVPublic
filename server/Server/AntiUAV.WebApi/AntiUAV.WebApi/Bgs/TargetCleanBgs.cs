using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Config;
using DbOrm.AntiUAV.Entity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Bgs
{
    /// <summary>
    /// 冗余目标清理任务
    /// </summary>
    public class TargetCleanBgs : BackgroundService
    {
        ILogger<TargetCleanBgs> _logger;
        IHistoryTargetService _target;
        IHistoryTrackService _track;
        private readonly BackupServiceConfig _config;
        private readonly object _lock;
        public TargetCleanBgs(ILogger<TargetCleanBgs> logger, IHistoryTargetService targetService, IHistoryTrackService trackService, BackupServiceConfig config)
        {
            _logger = logger;
            _target = targetService;
            _track = trackService;
            _config = config;
            _lock = new object();
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var work = Task.Run(() =>
                        {
                            var _backupTag = RedisHelper.Get(RedisCacheKeyConst.LastBackupKey);
                            _ = BackupData(_backupTag);
                            //清理数据
                            _ = CleanData(_backupTag);
                        });
                        Task.WaitAll(work, Task.Delay(_config.BackupInterval));
                    }
                    catch (Exception es)
                    {
                        _logger.LogError($"summary target error:{es.Message}");
                    }
                }
            });
        }

        /// <summary>
        /// 备份数据
        /// </summary>
        private async Task BackupData(string _backupTag)
        {
            var _nowTime = DateTime.Now;
            //默认备份前15天记录
            var _lastTime = string.IsNullOrEmpty(_backupTag) ? _nowTime.AddDays(-1 * _config.BackupLastDays).Earliest() : DateTime.Parse(_backupTag);
            var _totalHours = (int)(_nowTime - _lastTime).TotalHours;
            if (_totalHours < 1) return;
            for (int i = 1; i <= _totalHours; i++)
            {
                DateTime start, end;
                var _doTime = _lastTime.AddHours(i);
                _doTime.CalcHourRang(out start, out end);
                var _tgs = await _target.GetAnyAsync<HistoryBackup>(f => f.Endtime <= end && f.Endtime >= start);
                if (_tgs == null || _tgs.Count() == 0) continue;
                _tgs?.ToList()?.ForEach(async f =>
                {
                    f.TrackData = await _track.GetAnyAsync<HistoryTrackInfo>(tt => tt.TargetId == f.TgId);
                });
                lock (_lock)
                {
                    _tgs.ToJson().SaveToFile(_doTime, _config.BaseDictory);
                }
            }
            //备份标签写入redis
            RedisHelper.Set(RedisCacheKeyConst.LastBackupKey, _nowTime);
        }

        /// <summary>
        /// 清理数据 
        /// </summary>
        /// <param name="_backupTag"></param>
        private async Task CleanData(string _backupTag)
        {
            //未进行备份数据  则先不进行数据清理
            if (string.IsNullOrEmpty(_backupTag)) return;
            //上次备份时间
            var _lastBackTime = DateTime.Parse(_backupTag);
            var _lastSaveTime = DateTime.Now.AddDays(-1 * _config.BackupLastDays).Latest(); 
            //判断上次备份时间晚于数据保留最晚时间  则进行清理
            if (_lastBackTime > _lastSaveTime) 
            {
                //删除数据
                var _delData =await _target.GetAnyAsync(f => f.Endtime <= _lastSaveTime);
                var _delTrackData = await _track.GetAnyAsync(f => f.TrackTime <= _lastSaveTime);
            }
        }
    }
}
