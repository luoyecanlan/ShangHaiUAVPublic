using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using AntiUAV.DeviceServer.Abstractions.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.ProbeR04.Cmd
{
    public class A_StatusCmd : IPeerCmd
    {
        public A_StatusCmd(ILogger<A_StatusCmd> logger, IMemoryCache memory)
        {
            _memory = memory;
            _logger = logger;
        }
        public int Category => PluginConst.Category;
        public string Key => PluginConst.StatusCmdKey;
        public PeerCmdType Order => PeerCmdType.Action;

        private readonly IMemoryCache _memory;
        private readonly ILogger _logger;

        public Task Invoke(IPeerContent content)
        {
            var head = new byte[PluginConst.StatusCheckHead.Length];
            Buffer.BlockCopy(content.Source, 0, head, 0, head.Length);
            var checkCmd = Enumerable.SequenceEqual(PluginConst.StatusCheckHead, head);
            if (checkCmd)
            {
                var dev = _memory.GetDevice();
                var data = content.Source.ToStuct<R_ProbeR04_Status>();
                byte[] bt = { data.Params2[^1] };
                //byte[] bt = new byte[4];
                //Array.Copy(data.Params2, bt, 4);
                //bt[3]
                BitArray bArr = new BitArray(bt);
                var power = bArr.Get(3);
                if (power)
                {
                    dev.Alt = data.RadarHeight;
                    dev.Lng = data.RadarLng;
                    dev.Lat = data.RadarLat;
                    byte[] bt2 = { data.Params[0] };
                    BitArray bArr2 = new BitArray(bt2);
                    if (bArr2.Get(6))
                    {
                        _memory.UpdateDeviceRun(DeviceStatusCode.Running);//设备正常运行
                        var position = new DevPositionInfo
                        {
                            Alt = data.RadarHeight,
                            Lat = data.RadarLat,
                            Lng = data.RadarLng
                        };
                        _memory.UpdateDevPosition(position);
                    }
                    else
                    {
                        _memory.UpdateDeviceRun(DeviceStatusCode.Free);//待机
                    }
                }
                else //雷达连接状态 0-异常
                {
                    _memory.UpdateDeviceRun(DeviceStatusCode.OffLine);
                }
                return Task.CompletedTask;
            }
            else
            {
                _logger.LogWarning("The A_StatusCmd Command failed detection");
                return Task.FromCanceled(new System.Threading.CancellationToken());
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct R_ProbeR04_Status
    {
        public uint Head;//帧头:0x55DDDD55
        public ushort Length; //报文总字节数120
        public UInt64 TimeSpan; //utc 时间戳 ms

        //接收衰减	/	6bit	0～63	/	第15个字节1-6位
        //激励开关	/	1bit	0-	关闭发射 1-	打开发射	/	第15个字节第7位
        //雷达波形	/	3bit	0-7	/	第15个字节第8位 | 第16个字节第1-2位 <<1
        //积累脉冲数	/	3bit	0-64 1-128 2-256 3-512 4-1024 5-2048	/	第16个字节3-5位
        //工作模式	/	3bit	0～7	/	第16个字节6-8位

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Params; //接受衰减-工作模式

        public ushort Threshold; //检测门限16bit	1～65535	/	绝对门限
        public ushort Factor; //门限因子16bit	7～100	/	信躁比门限

        /*
            Resv	/	27bit	/	/	/
            STC开关	/	1bit	0-STC关 1-STC开	/	第24个字节第4位
            轮飞选择	/	1bit	0-轮飞关 1-轮飞开	/	第24个字节第5位
            跟踪开关	/	1bit	0-无效 1-有效	/	第24个字节第6位
            跟踪选择	/	1bit	0-自动跟踪 1-指定跟踪	/	第24个字节第7位
            雷达连接状态	/	1bit	0-异常 1-正常	/	第24个字节第8位
         */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Params2; //

        public float CPI;//CPI时间	float	32bit	/	1ms	默认0
        public uint Duration; //周期持续时间	u-Int	32bit	0～65535	帧	默认0
        public uint LoseNumber; //丢包数	u-Int	32bit	/	/	默认0
        public float TurretData; //转台俯仰数据	float	32bit	-10～30	1°	默认0
        public float ModulationData; //调制俯仰数据	float	32bit	-10～30	1°	默认0
        public ushort FllowTargetId; //跟踪目标编号	u-short	16bit	/	/	/
        public short AngleCorrection; //角度修正	Short	16bit	-18000～18000	0.01°	/
        public ushort TurntableOrientation; //转台方位角	u-Short	16bit	0-36000	0.01°	/
        public short TurntablePitch; //转台俯仰角 Short	16bit	-18000～18000	0.01°	/
        public ushort CompassPosition; //罗盘方位角 u-Short	16bit	0-36000	0.01°	/
        public short CompassPitch; //罗盘俯仰	Short	16bit	-18000～18000	0.01°	/
        public short CompassTilt; //罗盘横倾	Short	16bit	-18000～18000	0.01°	/
        public double RadarLng; //雷达经度	double	64bit	/	/	/
        public double RadarLat; //雷达纬度	double	64bit
        public float RadarHeight; //雷达高度	Float	32bit
        public ushort RadarType; //雷达类型	u-Short	16bit
        public uint RadarId; //雷达ID	u-Int	32bit	/	/	默认0
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] ClutterStatus; //杂波图状态	u-char	8bit	/	/	0-	建立完成 1-	正在建立
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
        public byte[] Resv2; //Resv	/	31*8bit	/	/	/
        public uint CheckCode; //校验码	/	32bit	/	/	32bit累加和
    }
}
