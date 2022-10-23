using AntiUAV.DevicePlugin.ProbeR05.Cmd;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.PluginService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR05
{
    public class DeviceHostService : DeviceHostUdpServerBase
    {
        public DeviceHostService(ILogger<DeviceHostService> logger, IPeerServer peer, IMemoryCache memory) : base(logger, peer, memory)
        {
        }
        public override int DeviceCategory => PluginConst.Category;
        public override void Start()
        {
            var dev = _memory.GetDevice();
            try
            {
                base.Start();
                
                byte[] buff = { 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa };
                _peer.SendAsync(buff, dev.Ip, dev.Port);

                _logger.LogDebug($"设备{dev.Id}：建立连接成功");
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"设备{dev.Id}：建立连接失败");
                throw ex;
            }
            //协议规定 硬件连接后发送一条链接报文后建立连接，雷达向上位机发送航迹报文 1s数据率
            //Task.Run(() =>
            //{

            //});


            Thread thread = new Thread(new ThreadStart(StatusQuery));
            thread.Start();
            thread.IsBackground = true;

        }
        /// <summary>
        /// 雷达状态查询（当前只查bit状态，如果后期需要查gps状态则可以根据协议把status2改成0x02即可 也可以0x01 0x02轮换
        /// </summary>
        public void StatusQuery()
        {
            var dev = _memory.GetDevice();
            byte[] head = { 0xaa, 0xaa, 0xaa, 0xaa };
            byte[] type= { 0x0b, 0x00, 0x00, 0x00 };
            R_Status r_Status = new R_Status()
            {
                Head = BitConverter.ToUInt32(head),
                InfoType = BitConverter.ToUInt32(type),
                ParamType = 0x01,
                TargetStation=0x00,
                TargetFront=0x00,
                CheckSum=0x01,
                Reverse =new byte[]{0x00, 0x00, 0x00, 0x00},
                Status1= new byte[] { 0x00, 0x00, 0x00, 0x00 },
                Status2= new byte[] { 0x01, 0x00, 0x00, 0x00 },
                Status3= new byte[] { 0x00, 0x00, 0x00, 0x00 },
                Status4 = new byte[] {0x00, 0x00, 0x00, 0x00 },

            };
            int size = Marshal.SizeOf(r_Status);
            byte[] r_Statusresult = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);//分配结构体大小的内存空间
            Marshal.StructureToPtr(r_Status, structPtr, false);//将结构体拷到分配好的内存空间
            Marshal.Copy(structPtr, r_Statusresult, 0, size);//从内存空间拷到byte数组
            Marshal.FreeHGlobal(structPtr);//释放内存空间
            while (true)
            {
                _peer.Send(r_Statusresult, dev.Ip, dev.Port,"");
              
                Thread.Sleep(3000);
            }
        }
    }
}
