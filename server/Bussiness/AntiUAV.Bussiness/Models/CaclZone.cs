using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    internal class CaclZone
    {
        public double CircumcircleR { get; set; } // double
        public double CircumcircleRadiationR { get; set; } // double
        public double CircumcircleLat { get; set; } // double
        public double CircumcircleLng { get; set; } // double
        public double BDistance { get; set; } // double
        public double ADistance { get; set; } // double
        public double NormalDistance { get; set; } // double
    }
}
