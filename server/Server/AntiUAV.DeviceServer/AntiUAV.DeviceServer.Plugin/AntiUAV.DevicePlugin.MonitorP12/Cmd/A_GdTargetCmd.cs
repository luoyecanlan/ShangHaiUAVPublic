using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Cmds;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.MonitorP11.Cmd
{
    public class A_GdTargetCmd : IPeerCmd
    {
        public A_GdTargetCmd(ILogger<A_GdTargetCmd> logger, IMemoryCache memory, IServiceOpt opt)
        {
            _logger = logger;
            _memory = memory;
            _opt = opt;
        }

        private readonly ILogger _logger;
        private readonly IMemoryCache _memory;
        private readonly IServiceOpt _opt;
        private const int GUID_ARITH_INTERVAL = 3;
        private const int GUID_ARITH_MAX_INTERVAL = 60;
        private List<GuidancePositionInfo> guidDatas = new List<GuidancePositionInfo>();
        private Task _guideTask;
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        public int Category => PluginConst.Category;

        public string Key => PluginConst.GuidanceCmdKey;

        public PeerCmdType Order => PeerCmdType.Action;

        public Task Invoke(IPeerContent content)
        {
            var gpi = content.Source.ToObject<GuidancePositionInfo>(4);
            //_logger.LogError($"收到引导目标信息：{gpi.ToJson()}");
            Console.WriteLine($"【{DateTime.Now}】开始引导，{gpi.TargetId},{gpi.TargetX}、{gpi.TargetY}");
            //_memory.GetTracks();
            if (!(double.IsNaN(gpi.TargetX) || double.IsNaN(gpi.TargetY)))
            {
                if (!guidDatas.Any(f => gpi.TrackTime.Second == f.TrackTime.Second))
                {
                    guidDatas.Insert(0, gpi);
                }
                if (guidDatas.Count > 4)
                {
                    guidDatas.RemoveRange(4, guidDatas.Count - 4);
                }
                _opt.Guidance(gpi);
                //Task.WaitAll(_opt.Guidance(gpi), Task.Delay(8000));
                //启动跟踪算法
                if (_guideTask == null)
                {
                    _guideTask = new Task(() =>
                   {
                       while (!tokenSource.IsCancellationRequested)
                       {
                           Task.WaitAll(DoGuideArithmetic(), Task.Delay(GUID_ARITH_INTERVAL * 1000));
                            //await DoGuideArithmetic();
                            //await Task.Delay(1000);
                        }
                   }, tokenSource.Token);
                }
                if (_guideTask != null && _guideTask.Status == TaskStatus.Created) _guideTask.Start();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 执行引导预测
        /// </summary>
        private Task DoGuideArithmetic()
        {
            var lastGpi = guidDatas?.First();
            var time = DateTime.Now;
            //判断当前探测设备是否正在引导
            //判断上次引导的时间戳
            //比对当前的时间差是否超过给定的时间间隔
            //判断到引导目标信息超过最大时间间隔未更新，则认为停止引导
            if (lastGpi != null && time.Subtract(lastGpi.TrackTime).TotalSeconds < GUID_ARITH_MAX_INTERVAL)
            {
                //if (time.Subtract(lastGpi.TrackTime).TotalSeconds > GUID_ARITH_INTERVAL)
                //{
                //如果超过给定的时间间隔，则采用预测轨迹，否则不执行
                Arithmetic(ref lastGpi);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"【{DateTime.Now}】引导预测算法结果：{lastGpi.TargetId}:{lastGpi.TargetX},{lastGpi.TargetY}");
                //_opt.Guidance(lastGpi);
                //}
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 预测引导数据
        /// </summary>
        /// <param name="gpi"></param>
        private void Arithmetic(ref GuidancePositionInfo gpi)
        {
            var list = guidDatas.ToList();
            if (list.Count > 3)
            {
                var start = list.Last().TrackTime;

                var t3 = (int)(Math.Ceiling((list[0].TrackTime - start).TotalSeconds / 1));
                var t2 = (int)(Math.Ceiling((list[1].TrackTime - start).TotalSeconds / 1));
                var t1 = (int)(Math.Ceiling((list[2].TrackTime - start).TotalSeconds / 1));
                var Az = getValue(t1, t2, t3, list[1].TargetX, list[2].TargetX, list[3].TargetX);
                var El = getValue(t1, t2, t3, list[1].TargetY, list[2].TargetY, list[3].TargetY);
                gpi.TargetX = Az;
                gpi.TargetY = El;
            }
        }
        /// <summary>
        /// 多项式插值轨迹预算算法
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        private double getValue(int t1, int t2, int t3, double v1, double v2, double v3)
        {
            double a0, a1, a2;
            int dt = 2;
            var A1 = t2 * Math.Pow(t3,2) + t1 * Math.Pow(t2, 2) + Math.Pow(t1, 2) * t3 - Math.Pow(t1, 2) * t2 - t1 * Math.Pow(t3, 2) - t3 * Math.Pow(t2, 2);
            a0 = Math.Round(((t2 * Math.Pow(t3, 2) - Math.Pow(t2,2) * t3) * v1 + ((-t1 * Math.Pow(t3, 2) + Math.Pow(t1, 2) * t3) * v2) + (t1 * Math.Pow(t2, 2) - Math.Pow(t1, 2) * t2) * v3) / A1, 8);
            a1 = Math.Round(((-Math.Pow(t3, 2) + Math.Pow(t2, 2)) * v1 + (Math.Pow(t3, 2) - Math.Pow(t1, 2)) * v2 + (-Math.Pow(t2, 2) + Math.Pow(t1, 2)) * v3) / A1, 8);
            a2 = Math.Round(((t3 - t2) * v1 + (t1 - t3) * v2 + (t2 - t1) * v3) / A1, 8);
            var value = a0 + a1 * (t3 + 2) + a2 * Math.Pow((t3 + dt), 2);
            return value;
        }

    }
}