using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.Service.ServiceImpl
{
    public class MemoryBusEvent : IMemoryBusEvent
    {
        public MemoryBusEvent(IMemoryCache memory)
        {
            _memory = memory;
        }

        private readonly IMemoryCache _memory;

        public Task<R> RegistEvent<R>(object key, int millsec = 3000)
        {
            var bus = _memory.GetOrCreate(key, entity =>
            {
                entity.SetSlidingExpiration(TimeSpan.FromMilliseconds(millsec));
                return new MemoryBusEventModel(key);
            });
            if (bus.Manual.WaitOne(TimeSpan.FromMilliseconds(millsec)))
            {
                Task.FromResult(bus.ResData);
            }
            return default;
        }

        public Task<bool> ResoponseEvent(object key, object data)
        {
            if (_memory.TryGetValue(key, out MemoryBusEventModel bus))
            {
                bus.ResData = data;
                return Task.FromResult(bus.Manual.Set());
            }
            return default;
        }
    }

    internal class MemoryBusEventModel
    {
        public MemoryBusEventModel(object key)
        {
            Key = key;
            Manual = new ManualResetEvent(false);
        }
        public object Key { get; }

        public ManualResetEvent Manual { get; }

        public object ResData { get; set; }
    }
}
