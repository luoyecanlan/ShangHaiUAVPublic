using AntiUAV.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness
{
    public class RedisCacheKeyConst
    {
        public const string DeviceStatusKey = "dev.status";

        public const string DeviceHostKey = "dev.host";

        public const string DeviceTrackKey = "dev.target.last";

        public const string DeviceHistoryKey = "dev.target.history";

        public const string DeviceTrackDisKey = "dev.target.dis";

        public const string SummaryTimeKey = "summary.last.time";

        public const string DeviceSelectKey = "select.device.";
        public const string LastBackupKey = "backup.last.time";
        public static string GetRelationshipsCacheKey() => "relationships";

        public static string GetHitLogCacheKey() => "hitlog";

        public static string GetRelationshipsFieldCacheKey(string tgid, int devid, RelationshipsType type) => $"Tg-{tgid}.To-{devid}.T-{(int)type}";

        public static string GetDeviceStatusCacheKey(int devId)
        {
            if (devId == 0)
                return $"{DeviceStatusKey}.*";
            return $"{DeviceStatusKey}.{devId}";
        }

        public static string GetDeviceHostCacheKey(int devId)
        {
            return devId > 0 ? $"{DeviceHostKey}.{devId}" : $"{DeviceHostKey}.*";
        }

        public static string GetDeviceTrackCacheKey(string tgid, int devId)
        {
            return $"{DeviceTrackKey}.{devId}.{tgid}";
        }

        public static string GetDeviceAllTrackCacheKey()
        {
            return $"{DeviceTrackKey}.*";
        }

        public static string GetDeviceHistoryCacheKey(string tgid, int devId)
        {
            return $"{DeviceHistoryKey}.{devId}.{tgid}";
        }

        public static string GetDeviceTrackDisCacheKey(int devId)
        {
            return $"{DeviceTrackDisKey}.{devId}";
        }

        public static string ConvertHistoryKeyToTrackKey(string history)
        {
            return history.Replace("history", "last");
        }
    }
}
