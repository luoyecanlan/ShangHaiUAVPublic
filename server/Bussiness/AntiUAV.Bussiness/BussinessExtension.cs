using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.Bussiness.ServiceImpl;
using DbOrm.AntiUAV.Entity;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace AntiUAV.Bussiness
{
    public static class BussinessExtension
    {
        /// <summary>
        /// 依赖注入方法
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>        
        public static IServiceCollection AddAntiUAVBussinessServices(this IServiceCollection services, IConfiguration config)
        {
            //.AddTransient<IMetadataService, MetadataService>()
            services.AddSingleton<IDeviceService, DeviceService>()
                .AddSingleton<IPreWarningZoneService, PreWarningZoneService>()
                .AddSingleton<ITargetService, TargetService>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IMapConfigService, MapConfigService>()
                .AddSingleton<ITargetTurnService, TargetTurnService>()
                .AddSingleton<IPersonalizationService, PersonalizationService>()
                .AddSingleton<ISystemConfigService, SystemConfigService>()
                .AddSingleton<INoticeDeviceService, NoticeDeviceService>()
                .AddSingleton<IHistoryTargetService, HistoryTargetService>()
                .AddSingleton<IHistoryTrackService, HistoryTrackService>()
                .AddSingleton<ISummaryService, SummaryService>()
                .AddSingleton<IWhiteListService, WhiteListService>()
                .AddSingleton<IUavModelService, UavModelService>()
                .AddSingleton<GisTool>()
                .AddRedisCache(config);
            return services;
        }

        public static IServiceCollection AddRabbitMqService(this IServiceCollection services, IConfiguration configuration/*, Assembly assembly*/)
        {
            return services.RegisterEasyNetQ(configuration.GetConnectionString("RabbitMq"));

            ////var subscriber = new AutoSubscriber(bus, "my_applications_subscriptionId_prefix");
            ////subscriber.Subscribe(Assembly.GetExecutingAssembly());
            //var bus = RabbitHutch.CreateBus(configuration.GetConnectionString("RabbitMq"));
            //var autoSubscriber = new AutoSubscriber(bus, "test")
            //{
            //    ConfigureSubscriptionConfiguration = c => c.WithAutoDelete()
            //                                               .WithPriority(10)

            //};
            //autoSubscriber.Subscribe(assembly);
            //autoSubscriber.SubscribeAsync(assembly);
            //return services.AddSingleton(bus);
        }

        /// <summary>
        /// 获取枚举值描述
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns>枚举描述</returns>
        public static string GetDescription(this Enum value)
        {
            return value.GetType()
                        .GetMember(value.ToString())
                        .FirstOrDefault()?
                        .GetCustomAttribute<DescriptionAttribute>()?
                        .Description;
        }

        /// <summary>
        /// 获取枚举值码
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns>int32码</returns>
        public static int GetValue(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// 判断设备类型是否是探测设备
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool IsProbeDevice(this int category)
        {
            return category < 20000;
        }


        /// <summary>
        /// 判断设备类型是否是监测(光电)设备
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool IsMonitorDevice(this int category)
        {
            return category < 30000 && category > 20000;
        }


        /// <summary>
        /// 判断设备类型是否是打击设备
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool IsHitDevice(this int category)
        {
            return category > 30000;
        }

        /// <summary>
        /// 转换JSON字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj) where T : class
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 转换实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 转换UTF-8编码字节数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="pix"></param>
        /// <returns></returns>
        public static byte[] ToBytes<T>(this T obj, string pix = "") where T : class
        {
            return Encoding.UTF8.GetBytes($"{pix}{obj?.ToJson() ?? ""}");
        }

        /// <summary>
        /// UTF-8编码字节数组转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static T ToObject<T>(this byte[] buff) where T : class
        {
            return buff.ToObject<T>(0, buff.Length);
        }

        /// <summary>
        /// UTF-8编码字节数组转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buff"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T ToObject<T>(this byte[] buff, int index) where T : class
        {
            return buff.ToObject<T>(index, buff.Length - index);
        }

        /// <summary>
        /// UTF-8编码字节数组转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buff"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T ToObject<T>(this byte[] buff, int index, int count) where T : class
        {
            return Encoding.UTF8.GetString(buff, index, count).ToObject<T>();
        }

        /// <summary>
        /// 获取目标的发现设备ID
        /// </summary>
        /// <param name="tgid"></param>
        /// <returns></returns>
        public static int ToDeviceId(this string tgid)
        {
            if (!string.IsNullOrEmpty(tgid) && tgid.Contains('.'))
            {
                if (int.TryParse(tgid.Split(".")[1], out int devId))
                {
                    return devId;
                }
            }
            return default;
        }

        /// <summary>
        /// 格式化小数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static double Format(this double num, int len = 2)
        {
            return Math.Round(num, len);
        }

        /// <summary>
        /// 返回当日最晚时间
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static DateTime Latest(this DateTime now)
        {
            return DateTime.Parse(now.ToShortDateString() + " 23:59:59");
        }
        /// <summary>
        /// 返回当日最早时间
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static DateTime Earliest(this DateTime now)
        {
            return DateTime.Parse(now.ToShortDateString() + " 00:00:00");
        }
        /// <summary>
        /// 返回当日最晚时间（字符串格式:2020-05-05）
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static DateTime Latest(this string now)
        {
            return DateTime.Parse($"{now} 23:59:59");
        }
        /// <summary>
        /// 返回当日最早时间（字符串格式:2020-05-05）
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static DateTime Earliest(this string now)
        {
            return DateTime.Parse($"{now} 00:00:00");
        }
        /// <summary>
        /// 威胁度与告警等级转换（1：红；2：黄；3：蓝；0：正常）
        /// </summary>
        /// <param name="threat"></param>
        /// <returns></returns>
        public static int CalcThreat(this double threat)
        {
            if (threat >= 70) return 1;
            else if (threat < 70 && threat >= 60) return 2;
            else if (threat < 60 && threat >= 50) return 3;
            else return 0;
        }

        /// <summary>
        /// 根据类型计算时间范围(一天中的最大与最小)
        /// </summary>
        /// <param name="now"></param>
        /// <param name="category"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void CalcDateRang(this DateTime now, SummaryTimeCategory category, out DateTime start, out DateTime end)
        {
            switch (category)
            {
                case SummaryTimeCategory.Week:
                    {
                        var days = now.DayOfWeek == 0 ? 7 : (int)now.DayOfWeek;
                        start = now.AddDays(-1 * (days + 6)).Earliest();
                        end = now.AddDays(-1 * days).Latest();
                        break;
                    }
                case SummaryTimeCategory.Month:
                    {
                        start = now.AddMonths(-1).ToString("yyyy-MM-01").Earliest();
                        end = now.AddDays(-1 * now.Day).Latest();
                        break;
                    }
                case SummaryTimeCategory.Year:
                    {
                        start = now.AddYears(-1).ToString("yyyy-01-01").Earliest();
                        end = now.AddYears(-1).ToString("yyyy-12-31").Latest();
                        break;
                    }
                default:
                    {
                        start = now.AddDays(-1).Earliest();
                        end = now.AddDays(-1).Latest();
                        break;
                    }
            }
        }
        /// <summary>
        /// 根据类型计算时间范围(一天中的最大与最小)
        /// </summary>
        /// <param name="now"></param>
        /// <param name="category"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void CalcHourRang(this DateTime now, out DateTime start, out DateTime end)
        {
            start = DateTime.Parse(now.ToString("yyyy-MM-dd HH:00:00"));
            end = DateTime.Parse(now.ToString("yyyy-MM-dd HH:59:59"));
        }
        /// <summary>
        /// 保存内容到文件
        /// </summary>
        /// <param name="content"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool SaveToFile(this string content, DateTime date, string basePath)
        {
            string fileDirectory = Path.Combine(basePath, date.Year + "", date.Month + "", date.ToString("yyyy-MM-dd"));
            string fileName = $"{date.ToString("yyyy-MM-dd")}.{date.Hour}.data.txt";
            string filePath = Path.Combine(fileDirectory, fileName);
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
            //存在则不进行重复备份
            if (File.Exists(filePath)) return true;
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(content);
                sw.Close();
                fs.Close();
            }
            return true;
        }
    }
}
