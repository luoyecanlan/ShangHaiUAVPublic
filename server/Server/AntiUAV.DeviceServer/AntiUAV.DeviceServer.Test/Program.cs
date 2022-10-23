using AntiUAV.DeviceServer.Test.TestModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ITestModel test = new ProbeR04TestModel();
            Console.WriteLine("键入a航迹数据通信协议,b过界数据通信协议,c雷达状态数据通信协议,esc结束");
            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.A)
                    SocketClienter.SendUDP(test.航迹数据通信协议());
                else if (Console.ReadKey().Key == ConsoleKey.B)
                    SocketClienter.SendUDP(test.过界数据通信协议());
                else if (Console.ReadKey().Key == ConsoleKey.C)
                    SocketClienter.SendUDP(test.雷达状态数据通信协议());
                else if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;
                else
                    continue;
            }
            SocketClienter.Close();
            Console.WriteLine("任意键结束");
            Console.ReadKey();
        }
    }
}
