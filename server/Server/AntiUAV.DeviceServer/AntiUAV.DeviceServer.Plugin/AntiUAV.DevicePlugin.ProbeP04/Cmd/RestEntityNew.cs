using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.ProbeP04.Cmd
{

    public class LoginRequest
    {

        public string password { get; set; }
        public string username { get; set; }
    }

    public class LoginObject
    {
        public int id { get; set; }
        public Ou ou { get; set; }
        public string real_name { get; set; }
        public int role { get; set; }
        public string token { get; set; }
        public string username { get; set; }
    }

    public class Ou
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class RestEntityNew
    {
        public Rootobject data { get; set; }
    }
    public class Rootobject
    {
        public bool db_migration_error { get; set; }
        public Defense_Status defense_status { get; set; }
        public Devices_Info devices_info { get; set; }
        public int gps_limit { get; set; }
        public bool has_danger { get; set; }
        public bool hide_defense_features { get; set; }
        public bool is_all_expired { get; set; }
        public bool is_local_mode { get; set; }
        public bool offline_map_loaded { get; set; }
        public bool online_map_enabled { get; set; }
        public Sensors_Info sensors_info { get; set; }
        public bool show_enhanced_features { get; set; }
        public int total_devices { get; set; }
        public Upgrade_Status upgrade_status { get; set; }
        public User_Locations user_locations { get; set; }
        public string version { get; set; }
    }

    public class Defense_Status
    {
        public int defense_duration { get; set; }
    }

    public class Devices_Info
    {
        public TargetInfoSpectrum[] targetInfoSpectrums { get; set; }
    }

    public class TargetInfoSpectrum
    {
        public Actions actions { get; set; }
        public object color_trajectory { get; set; }
        public int detect_counter { get; set; }
        public string detection_engine { get; set; }
        public string finder { get; set; }
        public string frequency { get; set; }
        public Gps gps { get; set; }
        public object[] gps_trajectory { get; set; }
        public string gui_id { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public bool in_defending_queue { get; set; }
        public bool is_dual_wbj { get; set; }
        public object is_ldval_testing { get; set; }
        public Ld_Result ld_result { get; set; }
        public object lf_error_radius { get; set; }
        public string model { get; set; }
        public object paired_rc { get; set; }
        public string prev_id { get; set; }
        public int seen_times { get; set; }
        public string[] sensors { get; set; }
        public Shared_Names shared_names { get; set; }
        public int[] signals { get; set; }
        public string status { get; set; }
        public bool switch_for_whitelist { get; set; }
        public Threat threat { get; set; }
        public string type { get; set; }
        public object wl_name { get; set; }
    }

    public class Actions
    {
        public bool can_cancel_defense { get; set; }
        public bool can_friend { get; set; }
        public bool can_ignore { get; set; }
        public bool can_ldval_test { get; set; }
        public bool can_manual_defend { get; set; }
        public bool can_smart_defend { get; set; }
        public bool is_decrypted { get; set; }
    }

    public class Gps
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Ld_Result
    {
        //[JsonProperty("SF1310012264")]
        //SF1311012340神州明达
        //SF1311012336金鹰
        //SF1311012298宝武
        public ProbeInfo SF1311012298 { get; set; }
    }

    public class ProbeInfo
    {
        public double azimuth { get; set; }
        public object distance { get; set; }
    }

    public class Shared_Names
    {
        public Device[] devices { get; set; }
        public string protocol { get; set; }
    }

    public class Device
    {
        public string icon { get; set; }
        public string name { get; set; }
    }

    public class Threat
    {
        public int color { get; set; }
        public int level { get; set; }
    }

    public class Sensors_Info
    {
        [JsonProperty("SF1200001011")]
        public devs dev { get; set; }
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
