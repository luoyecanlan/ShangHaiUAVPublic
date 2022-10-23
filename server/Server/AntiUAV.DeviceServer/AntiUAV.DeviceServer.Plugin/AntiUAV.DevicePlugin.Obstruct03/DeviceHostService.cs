using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer;
using AntiUAV.DeviceServer.Abstractions.HostService;
using AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl;
using AntiUAV.DeviceServer.Abstractions.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DevicePlugin.Obstruct03
{
    public class DeviceHostService : DeviceHostUdpServerBase
    {
        public DeviceHostService(ILogger<DeviceHostService> logger, IPeerServer peer, IMemoryCache memory) : base(logger, peer, memory)
        {

        }
        public override int DeviceCategory => PluginConst.Category;
    }
    //public class DeviceHostService : DeviceHostTcpServerBase
    //{
    //    public DeviceHostService(ILogger<DeviceHostTcpServerBase> logger, IPeerServer peer, IMemoryCache memory,IServiceOpt opt) : base(logger, peer, memory)
    //    {
    //        _opt = opt;
    //    }
    //    private IServiceOpt _opt;
    //    private UdpClient _udp;

    //    private Thread _listion;
    //    public override void Start()
    //    {
    //        //base.Start();
    //        byte[] buff = null;
    //        int port=_memory.GetDevice().Port;//////////////////////////////到底是port还是iport？？？？？？？？？不确定 待调试
    //        IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
    //        _udp = new UdpClient(ep);
    //        _listion = new Thread(new ThreadStart(() =>
    //        {

    //            while (true)
    //            {
    //                try
    //                {
    //                    //var gds = _memory.GetRelationshipsGuidance();
    //                    //int a = gds.Select(i => i.FromDeviceId).FirstOrDefault();
    //                    //_memory.
    //                    buff = _udp.Receive(ref ep);

    //                    var gpi = buff.ToObject<GuidancePositionInfo>(4);

    //                    _logger.LogDebug($"收到引导目标信息：{gpi.ToJson()}");
    //                    _opt.Guidance(gpi);
    //                    _memory.UpdateDeviceGdPosition(gpi);
    //                    //_ = Route?.ExcutePipeLineAsync(GetContent(buff, ep));
    //                }
    //                catch (SocketException sex)
    //                {
    //                    //socket 异常处理
    //                    _logger.LogDebug(sex.Message);
    //                    //Route?.GetSysCmd(PeerSysCmdType.Error)?.Invoke(GetErrorContent(buff, ep, "udp server recice data socket error.", sex));
    //                }
    //                catch (Exception ex)
    //                {
    //                    //Route?.GetSysCmd(PeerSysCmdType.Error)?.Invoke(GetErrorContent(buff, ep, "udp server recice data error.", ex));
    //                }
    //            }
    //        }));
    //        _listion.Start();


    //    }
    //    public override int DeviceCategory => PluginConst.Category;
    //}
}
