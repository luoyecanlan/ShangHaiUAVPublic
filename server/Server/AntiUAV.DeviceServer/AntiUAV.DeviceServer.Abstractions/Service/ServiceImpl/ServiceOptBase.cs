using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.Service.ServiceImpl
{
    public abstract class ServiceOptBase : IServiceOpt
    {
        public ServiceOptBase(IMemoryCache memory, IPeerServer peer, IDeviceOptService devOpt, IMemoryBusEvent memoryBus)
        {
            _peer = peer;
            _memory = memory;
            _devOpt = devOpt;
            _memoryBus = memoryBus;
        }

        protected readonly IDeviceOptService _devOpt;
        protected readonly IMemoryCache _memory;
        protected readonly IPeerServer _peer;
        protected readonly IMemoryBusEvent _memoryBus;

        public abstract Task<bool> InitializationCacheAsync(int deviceId);
        public abstract Task<bool> ReloadDeviceInfoAsync(int deviceId);
        public abstract Task<bool> ReloadDeviceZoneAsync(int deviceId);

        public virtual Task Guidance(GuidancePositionInfo position)
        {
            _memory.UpdateDeviceGdPosition(position);
            var buff = _devOpt.GetGuidanceBuff(position);
            if (buff?.Length > 0)
            {
                Console.WriteLine($"下发引导指令：{CollectionToString(buff)}");
                var dev = _memory.GetDevice();
                if (dev != null)
                    _peer.Send(buff, dev.Ip, dev.Port);
            }
            return Task.CompletedTask;
        }

        public virtual async Task<bool> SetAttack(string json, bool sw, int timeout = 3000)
        {
            var dev = _memory.GetDevice();
            //var buff1 = _devOpt.GetGuidanceBuff(json, sw);
            var buff = _devOpt.GetAttackBuff(json, sw);
            if (dev != null && buff?.Length > 0)
            {
                Console.WriteLine($"下发打击指令：{CollectionToString(buff)}");
                _peer.Send(buff, dev.Ip, dev.Port);
                //var atSw = await _memoryBus.RegistEvent<bool>(MemoryCacheKey.BusEventAttackSwKey, timeout);
                //return atSw;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 集合转字符串
        /// </summary>
        /// <typeparam name="T">集合成员类型</typeparam>
        /// <param name="coll">集合</param>
        /// <param name="split">分隔符（默认：","）</param>
        /// <returns></returns>
        public string CollectionToString<T>(IEnumerable<T> coll, string split = ",")
        {
            if (coll?.Count() > 0)
            {
                var sb = new StringBuilder();
                foreach (var c in coll)
                {
                    if (sb.Length > 0)
                        sb.Append(split);
                    sb.Append(JsonConvert.SerializeObject(c));
                }
                return sb.ToString();
            }
            return "";
        }

        public virtual async Task<bool> SetMointor(string json, bool sw, int timeout = 3000)
        {
            var dev = _memory.GetDevice();
            var buff = _devOpt.GetMonitorBuff(json, sw);
            if (dev != null && buff?.Length > 0)
            {
                Console.WriteLine($"下发光电跟踪指令：{CollectionToString(buff)}");
                _peer.Send(buff, dev.Ip, dev.Port);
                //var moSw = await _memoryBus.RegistEvent<bool>(MemoryCacheKey.BusEventMonitorSwKey, timeout);
                //return moSw;
                return true;
            }
            return false;
        }

        public virtual async Task<bool> SetRectify(int timeout = 3000)
        {
            var dev = _memory.GetDevice();
            var buff = _devOpt.GetRectifyBuff(0, 0);
            if (dev != null && buff?.Length > 0)
            {
                Console.WriteLine($"下发纠偏指令：{CollectionToString(buff)}");
                _peer.Send(buff, dev.Ip, dev.Port);
                //var dev_rect = await _memoryBus.RegistEvent<DevRectifyInfo>(MemoryCacheKey.BusEventRectifyInfoKey, timeout);
                // return dev_rect?.Az == dev.RectifyAz && dev_rect?.El == dev.RectifyEl;
                return true;
            }
            return false;
        }

        public virtual async Task<bool> SetPosition(int timeout = 3000)
        {
            var dev = _memory.GetDevice();
            var buff = _devOpt.GetPositionBuff(dev.Lat, dev.Lng, dev.Alt);
            if (dev != null && buff?.Length > 0)
            {
                Console.WriteLine($"下发位置指令：{CollectionToString(buff)}");
                _peer.Send(buff, dev.Ip, dev.Port);
                var dev_position = await _memoryBus.RegistEvent<DevPositionInfo>(MemoryCacheKey.BusEventRectifyInfoKey, timeout);
                return dev_position?.Lat == dev.Lat && dev_position?.Lng == dev.Lng && dev_position?.Alt == dev.Alt;
            }
            return false;
        }

        public abstract Task<bool> SetRunMode(string mode, int timeout = 3000);

        public virtual async Task<bool> SetOperationDevice(string json, int timeout = 3000)
        {
            var model = JsonConvert.DeserializeObject<DevicePTZInfo>(json);
            var dev = _memory.GetDevice();
            var buff = _devOpt.GetDeviceOpBuff((short)model.operateItem, model.Speed, (short)model.operateCode);
            if (dev != null && buff?.Length > 0)
            {
                Console.WriteLine($"下发设备操作指令：{CollectionToString(buff)}");
                _peer.Send(buff, dev.Ip, dev.Port);
                //var dev_position = await _memoryBus.RegistEvent<DevPositionInfo>(MemoryCacheKey.BusEventRectifyInfoKey, timeout);
                //return dev_position?.Lat == dev.Lat && dev_position?.Lng == dev.Lng && dev_position?.Alt == dev.Alt;
                return true;
            }
            return false;
        }
    }
}
