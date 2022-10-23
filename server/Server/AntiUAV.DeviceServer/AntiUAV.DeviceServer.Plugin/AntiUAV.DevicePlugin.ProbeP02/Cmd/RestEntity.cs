using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.ProbeP02.Cmd
{
    

    public class Rootobject
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public bool autoAttack { get; set; }
        public Drone[] drone { get; set; }
    }

    public class Drone
    {
        public int[] attack_bands { get; set; }
        public bool attacking { get; set; }
        public bool can_attack { get; set; }
        public bool can_takeover { get; set; }
        public string created_time { get; set; }
        public DateTime deleted_time { get; set; }
        public string description { get; set; }
        public int direction { get; set; }
        public string id { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public Seen_Sensor[] seen_sensor { get; set; }
        public string state { get; set; }
        public bool whitelisted { get; set; }
    }

    public class Seen_Sensor
    {
        public int detected_freq_khz { get; set; }
        public int detected_time { get; set; }
        public string sensor_id { get; set; }
    }


}
