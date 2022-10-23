using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AntiUAV.DeviceServer.Test
{
    public static class SocketClienter
    {
        static readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        static readonly EndPoint point = new IPEndPoint(IPAddress.Parse("192.168.9.82"), 5555);
        public static void SendUDP(byte[] content)
        {
            socket.SendTo(content, point);
            Console.WriteLine("Send Sussess");
        }
        public static void Close()
        {
            if (socket != null) socket.Close();
        }
    }
}
