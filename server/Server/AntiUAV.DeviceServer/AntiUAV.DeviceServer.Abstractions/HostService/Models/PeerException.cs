using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions.HostService.Models
{
    public class PeerException : Exception
    {
        public PeerException()
        {

        }

        public PeerException(string message)
        {
            OutMessage = message;
        }

        public PeerException(Exception exception)
        {
            SourceException = exception;
        }

        public PeerException(string message, Exception exception)
        {
            OutMessage = message;
            SourceException = exception;
        }

        public string OutMessage { get; set; }

        public Exception SourceException { get; set; }
    }
}
