using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions
{
    public static class HostedExtension
    {

        private static Type[] GetAllChildClass(Type baseType)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
            //取得实现了某个接口的类
            //.SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ISecurity))))  .ToArray();
            //取得继承了某个类的所有子类
            .SelectMany(a => a.GetTypes().Where(t => t.BaseType == baseType))
            .ToArray();

            return types;
        }


        public static Type[] GetAllBackgroundService()
        {
            return GetAllChildClass(typeof(BackgroundService));
        }

        /// <summary>
        /// 自动增加后台任务.所有继承自BackgroundService的类都会自动运行
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBackgroundServices(this IServiceCollection services, IEnumerable<Assembly> assemblies, Type baseType)
        {
            if (assemblies?.Count() <= 0 || baseType == null) return services;
            var backtypes = assemblies?
               //取得实现了某个接口的类
               //.SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ISecurity))))  .ToArray();
               //取得继承了某个类的所有子类
               .SelectMany(a => a.GetTypes().Where(t => t.BaseType == baseType))
               .ToArray();

            foreach (var backtype in backtypes)
            {
                services.AddTransient(typeof(IHostedService), backtype);
            }
            return services;
        }
    }
}
