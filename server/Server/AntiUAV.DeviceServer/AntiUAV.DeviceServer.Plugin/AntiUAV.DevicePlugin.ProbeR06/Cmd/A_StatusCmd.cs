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

namespace AntiUAV.DevicePlugin.ProbeR06.Cmd
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
            
            if (content.Source != null)
            {
                var dev = _memory.GetDevice();
                var data = content.Source.ToStuct<R_ProbeR06_Status>();
                //byte[] bt = { data.Params2[^1] };
                //byte[] bt = new byte[4];
                //Array.Copy(data.Params2, bt, 4);
                //bt[3]
                
                if ((byte)(data.BeamState & 0xff) == 1)
                {
                    _memory.UpdateDeviceRun(DeviceStatusCode.Free);


                }
                else //雷达连接状态 0-异常
                {
                    _memory.UpdateDeviceRun(DeviceStatusCode.Running);
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
    public struct R_ProbeR06_Status
    {
        public UInt32 RadarId;
        public byte BeamState;     //收发故障从这个取       最低位为1，表示功放关。
        public byte FreqsyntState;   //保留
        public byte FpgaState;   //保留
        public byte type;         //保留      区分实时状态还是 回放

        public float ReceiverTemper;   //收发温度
        public float FreqsyntTemper;   //频综温度
        public float FpgaTemper;      //fpga温度

        public float ServoTiltAngle1;     //伺服俯仰状态
        public float ServoTiltAngle2;
        public int ServoTiltWorkMode;
        public int ServoTiltSpeed;

        public float ServoPanAngle1;     //伺服方位状态
        public float ServoPanAngle2;
        public int ServoPanWorkMode;
        public int ServoPanSpeed;

        public uint BeamEnable;          //收发状态
        public uint BeamCodeMode;    //码型
        public uint BeamFreqPoint; //频率
        public uint BeamStc;  //stc曲线
        public int BeamDelay;//保留

        public float tiltAngle;  //天线俯仰角;  //单位：度
        public float panAngle;  //天线方位角;  //单位：度  

        public long utctime;    //自 0001 年 1 月 1 日午夜 12:00:00 以来已经过的时间的以 100 毫微秒为间隔的间隔数，
    }
}
