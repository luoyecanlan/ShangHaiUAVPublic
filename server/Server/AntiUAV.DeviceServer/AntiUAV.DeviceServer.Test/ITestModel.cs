using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Test
{
    public interface ITestModel
    {
        byte[] 过界数据通信协议();
        byte[] 航迹数据通信协议();
        byte[] 雷达状态数据通信协议();
    }
}
