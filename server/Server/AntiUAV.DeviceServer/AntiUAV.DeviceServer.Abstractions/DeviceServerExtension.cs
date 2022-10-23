using AntiUAV.DeviceServer.Abstractions;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.ServiceImpl;
using AntiUAV.DeviceServer.Abstractions.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AntiUAV.DeviceServer
{
    public static class DeviceServerExtension
    {
        /// <summary>
        /// 添加设备插件注入 这里需要全部注入
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDevicePlugin(this IServiceCollection services, Action<string> log = null)
        {
            var files = GetAllFiles(AppContext.BaseDirectory, "AntiUAV.DevicePlugin.*.dll");//寻找目录下所有的插件dll文件
            foreach (var file in files)
            {
                var assembly = Assembly.LoadFrom(file.FullName);
                var all = assembly?.GetTypes();
                var devHostType = all.LastOrDefault(x => x.GetInterfaces().Any(i => i == typeof(IDeviceHostService)));//最后一个可用的
                var devOptType = all.LastOrDefault(x => x.GetInterfaces().Any(i => i == typeof(IDeviceOptService)));//最后一个可用
                var devThreadWtg = all.LastOrDefault(x => x.GetInterfaces().Any(i => i == typeof(IThreatWeight)));//最后一个可用
                //var devHost = Activator.CreateInstance(devHostType) as IDeviceHostService;
                if (devHostType != null || devOptType != null)//== category)
                {
                    services.AddSingleton(typeof(IDeviceHostService), devHostType);//注入插件服务
                    services.AddSingleton(typeof(IDeviceOptService), devOptType);//注入设备操作服务
                    services.AddSingleton(typeof(IThreatWeight), devThreadWtg ?? typeof(ThreatWeight));//注入设备操作服务
                                                                                                       //数智元干扰炮是tcp协议，暂时没想到其他通用的判断tcp协议的办法，先这么凑合一下 
                                                                                                       //这是框架设计有缺陷 如果想添加其他通信协议改动会很大 暂时不动 把所有服务都注入 获取ipeerserver时只能获取最后一个 如果需要tcp的就把插件dll文件夹里除tcp之外的dll全删掉
                                                                                                       //if (file.FullName.Contains("Query01"))
                                                                                                       //{
                    services.AddSingleton(typeof(IPeerServer), all.FirstOrDefault(x => x.GetInterfaces().Any(i => i == typeof(IPeerServer)))
?? typeof(PeerServerUdp));//注入系统UDP服务
                    //}
                    //else
                    //{

                    //    services.AddSingleton(typeof(IPeerServer), all.LastOrDefault(x => x.GetInterfaces().Any(i => i == typeof(IPeerServer)))
                    //    ?? typeof(PeerServerTcp));//注入系统TCP服务
                    //}

                    // if (devHost.IsSysUdp)//判断是否使用系统UDP服务，并且判断是否有自主实现的服务和路由
                    //{


                    //}
                    //else
                    //{

                    //}
                    services.AddSingleton(typeof(IPeerRoute), all.FirstOrDefault(x => x.GetInterfaces().Any(i => i == typeof(IPeerRoute)))
                        ?? typeof(PeerRouteBase));//注入系统UDP服务路由
                    foreach (var cmd in all.Where(x => x.GetInterfaces().Any(i => i == typeof(IPeerCmd))))
                    {
                        services.AddTransient(typeof(IPeerCmd), cmd);//注入UDP服务处理命令
                    }
                    foreach (var cmd in all.Where(x => x.GetInterfaces().Any(i => i == typeof(IPeerSysCmd))))
                    {
                        services.AddTransient(typeof(IPeerSysCmd), cmd);//注入UDP服务处理命令
                    }

                    //注入插件中的IHostService
                    services.AddBackgroundServices(new[] { assembly }, typeof(BackgroundService));

                    //devHost.AddIocService(services);//注入其他插件需要的内容

                    //还有其他东西需要注入不？不确定，先留着
                    log?.Invoke($"loaded plugin for resource {file.Name}. ");//备用输出
                }
                else
                    log?.Invoke($"loaded plugin {file.Name} faild !!!!!! ");//备用输出
            }
            return services;
            static IEnumerable<FileInfo> GetAllFiles(string path, string search)//递归寻找目录
            {
                var files = new List<FileInfo>();
                var directory = new DirectoryInfo(path);
                files.AddRange(directory.GetFiles(search));
                foreach (var dic in directory.GetDirectories())
                {
                    files.AddRange(GetAllFiles(dic.FullName, search));
                }
                return files;
            }
        }

        /// <summary>
        /// 设备服务启动
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static bool DevServRun(this IServiceProvider provider)
        {
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);//设置grpc允许http和http/2的访问互通

            var memory = provider.GetService<IMemoryCache>();
            var dev = memory.GetDevice();
            if (dev == null) return false;

            var host = provider.GetServices<IDeviceHostService>().LastOrDefault(x => x.DeviceCategory == dev.Category);

            if (host == null) return false;
            if (host.IsSysUdp)
            {
                //初始化系统UDP监听
                var serv = provider.GetService<IPeerServer>();
                serv.Route.LoadCmds(provider);//初始化命令
            }
            host.RunCode = ConvertExtension.GetRandomString();
            host.Start();//开启本地服务
            return true;
        }
    }
}
