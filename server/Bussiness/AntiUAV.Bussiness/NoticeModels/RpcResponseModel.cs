using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.NoticeModels
{
    public class RpcResponseModel
    {
        public RpcResponseEnum Code { get; set; }

        public object Data { get; set; }
    }


    public enum RpcResponseEnum
    {
        Ok,
        Fail,
        UnKnow
    }
}
