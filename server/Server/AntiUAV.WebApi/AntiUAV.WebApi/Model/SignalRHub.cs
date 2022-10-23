using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Model
{
    public class SignalRHub : Hub
    {
        public const string hubRouter = "/lads_channel";
        public const string client_channel = "client_channel";
        public const string target_channel = "target_channel";
        public const string device_status_channel = "device_status_channel";
        public const string device_relation_ship_channel = "device_relation_ship_channel";

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
