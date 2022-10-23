using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using DbOrm.AntiUAV.Entity;
using System.Linq;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AntiUAV.Bussiness.ServiceImpl
{
    public class HistoryTrackService : MetadataService<HistoryTrackInfo, HistoryTrackInfo, HistoryTrackDel, HistoryTrackAdd>, IHistoryTrackService
    {
        public HistoryTrackService(IEntityCrudService orm, ILogger<HistoryTrackService> logger) : base(orm)
        {
            _logger = logger;
        }
        private readonly ILogger<HistoryTrackService> _logger;

        public Task AddHitMark(string tgid)
        {
            if (!string.IsNullOrWhiteSpace(tgid))
            {
                var b = RedisHelper.HSet(RedisCacheKeyConst.GetHitLogCacheKey(), tgid, true);
                _logger.LogInformation($"hit mark :{b}");
            }
            return Task.CompletedTask;
        }

        public Task<bool> RemoveHitMark(string tgid)
        {
            var res = false;
            if (!string.IsNullOrWhiteSpace(tgid))
            {
                res = RedisHelper.HDel(RedisCacheKeyConst.GetHitLogCacheKey(), tgid) > 0;
            }
            return Task.FromResult(res);
        }

        public async Task<IEnumerable<HistoryTrkInfo>> GetTracks(string tgId)
        {
            var tracks = await _orm.GetAnyAsync<HistoryTrackInfo>(f => f.TargetId == tgId, f => f.TrackTime, false);
            if (tracks?.Count() > 0)
            {
                var firstTrack = tracks.First();
                var _tracks = new List<HistoryTrkInfo>();
                int _order = 0;
                tracks.ToList().ForEach(f =>
                {
                    _tracks.Add(new HistoryTrkInfo()
                    {
                        TargetId = f.TargetId,
                        DeviceId = f.DeviceId,
                        Lat = f.Lat,
                        Lng = f.Lng,
                        Alt = f.Alt,
                        Category = f.Category,
                        Mode = f.Mode,
                        Threat = f.Threat,
                        TrackTime = f.TrackTime,
                        Order = ++_order,
                        RightScound = (f.TrackTime - firstTrack.TrackTime).TotalSeconds
                    });
                });
                return _tracks;
            }
            return null;
        }
    }
}
