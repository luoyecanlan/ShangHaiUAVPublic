using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.Obstruct04
{
    public static class GlobalVarAndFunc
    {
        public static string sensorname = "";
        public static string token = "";

    }

    public class devs
    {
        public bool ai_enabled { get; set; }
        public string antenna { get; set; }
        public bool auto_defense_status { get; set; }
        public bool capture_status { get; set; }
        public int compass { get; set; }
        public bool compass_enabled { get; set; }
        public bool configured_gps { get; set; }
        public int core_protection_radius { get; set; }
        public int defense_radius { get; set; }
        public int detection_radius { get; set; }
        public Df_Cone_Sectors df_cone_sectors { get; set; }
        public bool disk_has_space { get; set; }
        public bool dv_status { get; set; }
        public Gps1 gps { get; set; }
        public bool has_tdd_pa { get; set; }
        public bool is_df_enabled { get; set; }
        public bool is_wideband_enabled { get; set; }
        public bool mass_defense_status { get; set; }
        public Mgmt_Details mgmt_details { get; set; }
        public object[] noise_list { get; set; }
        public string ou { get; set; }
        public bool sanity_test_status { get; set; }
        public object sectors { get; set; }
        public object sectors_label { get; set; }
        public string sensor_name { get; set; }
        public string status { get; set; }
        public long termdate { get; set; }
    }

    public class Df_Cone_Sectors
    {
    }

    public class Gps1
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Mgmt_Details
    {
        [JsonProperty("1")]
        public ManageId manageId { get; set; }
    }

    public class ManageId
    {
        public float cpu_temp { get; set; }
        public float cpu_usage { get; set; }
        public Detecting_Frequency detecting_frequency { get; set; }
        public Disk_Io_Usage disk_io_usage { get; set; }
        public string disk_usage { get; set; }
        public string engine_status { get; set; }
        public Fixed_Gain fixed_gain { get; set; }
        public string gps_pps_status { get; set; }
        public int[] gps_signal { get; set; }
        public bool gps_signal_good { get; set; }
        public float gpu_temp { get; set; }
        public float gpu_usage { get; set; }
        public string ip { get; set; }
        public string mcu_status { get; set; }
        public float mem_usage { get; set; }
        public string mgmt_status { get; set; }
        public Network_Usage network_usage { get; set; }
        public int power_consumption { get; set; }
        public string power_options_status { get; set; }
        public string software_build { get; set; }
        public string software_version { get; set; }
        public Som_Status som_status { get; set; }
        public float uptime { get; set; }
        public string wifi_status { get; set; }
        public float xpu_freq { get; set; }
    }

    public class Detecting_Frequency
    {
        public string[] _1 { get; set; }
    }

    public class Disk_Io_Usage
    {
        public string io_read { get; set; }
        public string io_write { get; set; }
    }

    public class Fixed_Gain
    {
        public object _1 { get; set; }
    }

    public class Network_Usage
    {
        public Dummy0 dummy0 { get; set; }
        public Eth0 eth0 { get; set; }
        public Eth1 eth1 { get; set; }
        public L4tbr0 l4tbr0 { get; set; }
        public Lo lo { get; set; }
        public Rndis0 rndis0 { get; set; }
        public Usb0 usb0 { get; set; }
    }

    public class Dummy0
    {
        public float rx { get; set; }
        public float tx { get; set; }
    }

    public class Eth0
    {
        public string rx { get; set; }
        public float tx { get; set; }
    }

    public class Eth1
    {
        public float rx { get; set; }
        public float tx { get; set; }
    }

    public class L4tbr0
    {
        public float rx { get; set; }
        public float tx { get; set; }
    }

    public class Lo
    {
        public float rx { get; set; }
        public float tx { get; set; }
    }

    public class Rndis0
    {
        public float rx { get; set; }
        public float tx { get; set; }
    }

    public class Usb0
    {
        public float rx { get; set; }
        public float tx { get; set; }
    }

    public class Som_Status
    {
        public string _1 { get; set; }
    }

    public class Upgrade_Status
    {
    }

    public class User_Locations
    {
    }
}
