using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using AntiUAV.DeviceServer.Abstractions.Service;
using AntiUAV.DeviceServer.Serrvice;
using AntiUAV.Bussiness;
using AntiUAV.DeviceServer.Hosted;
using AntiUAV.DeviceServer;
using NLog;
using AntiUAV.DeviceServer.Abstractions.Service.ServiceImpl;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions;

namespace AntiUAV.DeviceServer
{
    public class Program
    {
        //public static int DeviceId;
        public static int Port;
        public static async Task Main(string[] args)
        {
            //if (!int.TryParse(args.FirstOrDefault(), out DeviceId) || DeviceId <= 0)
            //    Console.WriteLine("启动参数错误:未识别到有效的设备ID");
            //else if (!int.TryParse(args.LastOrDefault(), out Port) || Port < 3000 || Port > 65532)
            //{
            //    Console.WriteLine("启动参数错误:启动端口无效(3000~65532)");
            //}
            //else
            //{
            //    CreateHostBuilder(args).Build().Run();
            //}        
            var logger = LogManager.GetCurrentClassLogger();
            if (!int.TryParse(args.FirstOrDefault(), out int deviceId) || deviceId <= 0)
                logger.Error($"device id {args.FirstOrDefault()} error !!!!!!");
            else
            {
                var host = new HostBuilder()
                           .ConfigureHostConfiguration(configHost =>
                           {
                               configHost.SetBasePath(Directory.GetCurrentDirectory());
                               configHost.AddJsonFile("appsettings.json", optional: true);
                               //configHost.AddEnvironmentVariables(prefix: "PREFIX_");
                               configHost.AddCommandLine(args);
                           })
                           .ConfigureServices((context, services) =>
                           {
                               services.AddMemoryCache()
                                       .AddDbService(context.Configuration)
                                       .AddAntiUAVBussinessServices(context.Configuration)
                                       .AddRabbitMqService(context.Configuration)
                                       .AddDevicePlugin(message => logger.Info(message))
                                       .AddBackgroundServices(AppDomain.CurrentDomain.GetAssemblies(), typeof(BackgroundService))
                                       .AddSingleton<IServiceOpt, ServiceOpt>()
                                       .AddSingleton<IMemoryBusEvent, MemoryBusEvent>();
                           })
                           .ConfigureLogging((hostingContext, logging) =>
                           {
                               logging.ClearProviders();
                               //logging.AddConsole();
                               logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
                               logging.AddNLog();
                           })
                           .Build();

                var opt = host.Services.GetService<IServiceOpt>();
                //var ho=host.Services.GetServices<IDeviceHostService>().LastOrDefault(x => x.DeviceCategory == 30201);
                if (await opt.InitializationCacheAsync(deviceId) && host.Services.DevServRun())//初始化缓存&&服务启动
                {
                    await host.RunAsync();
                }
                else
                {
                    logger.Error("device server star fail !!!!!! 1.check plugin; 2.check run parameter; 3.check config connstring");
                }
            }
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.ConfigureKestrel(options =>
        //            {
        //                options.Listen(IPAddress.Any, Port, listenOptions =>
        //                {
        //                    listenOptions.Protocols = HttpProtocols.Http2;
        //                    //listenOptions.UseHttps("<path to .pfx file>",
        //                    //    "<certificate password>");
        //                });
        //            });
        //            //webBuilder.UseUrls("https://*:17878");
        //            webBuilder.UseStartup<Startup>();
        //        }).ConfigureLogging((hostingContext, logging) =>
        //        {
        //            logging.AddConsole();
        //            logging.AddNLog();
        //        });
    }
}
