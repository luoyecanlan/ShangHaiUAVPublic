using AntiUAV.WebApi.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi
{
    public static class SignalRExtension
    {
        public static IServiceCollection AddSignalRHost(this IServiceCollection services)
        {
            services.AddSignalR();
            return services;
        }
    }
}
