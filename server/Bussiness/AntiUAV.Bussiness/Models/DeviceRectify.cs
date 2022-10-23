using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 设备纠偏
    /// </summary>
    public class DeviceRectify 
    {
        public double Lat { get; set; }//位置纬度
        public double Lng { get; set; }//位置经度
        public double Alt { get; set; }//位置海拔
        public double RectifyAz { get; set; }//方位纠偏

        public double RectifyEl { get; set; }//俯仰纠偏
    }
}
