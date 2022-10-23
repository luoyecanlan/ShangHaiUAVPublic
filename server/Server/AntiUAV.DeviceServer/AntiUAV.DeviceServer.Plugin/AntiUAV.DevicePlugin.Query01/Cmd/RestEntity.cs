using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DevicePlugin.Query01.Cmd
{
    public class jsondata
    {
        public SpectruObject data { get; set; }
    }

    public class SpectruObject
    {
        /// <summary>
        /// 已部署设备列表罗盘信息
        /// </summary>
        public Compass_Info compass_info { get; set; }
        /// <summary>
        /// 防御状态
        /// </summary>
        public Defense_Status defense_status { get; set; }
        /// <summary>
        /// 当前识别到的无人机
        /// </summary>
        public Devices_Info devices_info { get; set; }
        /// <summary>
        /// 是否需要报警，true 的时候代表有至少一台非白名单探测
        /// </summary>
        public bool has_danger { get; set; }
        public bool hide_defense_features { get; set; }///
        public bool is_local_mode { get; set; }///
        /// <summary>
        /// 是否使用离线地图
        /// </summary>
        //public bool is_offline_enabled { get; set; }
        /// <summary>
        /// 已部署设备列表信息
        /// </summary>
        public Sensors_Info sensors_info { get; set; }///
        public bool show_enhanced_features { get; set; }///
        public bool show_library_features { get; set; }///
        /// <summary>
        /// 探测发现总数
        /// </summary>
        public int total_devices { get; set; }
        public object unknown_devices_info { get; set; }///
        /// <summary>
        /// 当前升级状态
        /// </summary>
        public Upgrade_Status upgrade_status { get; set; }
        public User_Locations user_locations { get; set; }///
        public string version { get; set; }
        public string version_compatible { get; set; }
    }

    public class User_Locations
    {
    }

    /// <summary>
    /// 已部署设备列表罗盘信息
    /// </summary>    
    public class Compass_Info
        //TODO此处应该是一个设备列表（支持多设备）暂时只写一个
    {
        [JsonProperty("SF1200001011")]
        public DevID SF1200001011 { get; set; }
    }

    public class DevID
    {
        public int compass { get; set; }
    }
    /// <summary>
    /// 防御状态
    /// </summary>
    public class Defense_Status
    {
        public int defense_duration { get; set; }
    }
    /// <summary>
    /// 当前识别到的无人机
    /// </summary>
    public class Devices_Info
    {
       
        [JsonProperty("00b09frc")]
        public TargetInfoSpectrum[] rc { get; set; }
    }

    public class TargetInfoSpectrum
    {
        /// <summary>
        /// 方位角
        /// </summary>
        public float azimuth { get; set; }
        public string bandwidth { get; set; }
        public object color_trajectory { get; set; }
        public Detect_Counter detect_counter { get; set; }
        /// <summary>
        /// 带宽
        /// </summary>
        public bool can_manual_defend { get; set; }
        /// <summary>
        /// 是否可以手动开启对此机器的防御
        /// </summary>
        public bool defense_cancellable { get; set; }
        /// <summary>
        /// 无人机名字 
        /// </summary>
        public string device_name { get; set; }
        /// <summary>
        /// 'df' 代表定向发现
        /// </summary>
        public string finder { get; set; }
        /// <summary>
        /// 无人机所在频段 无
        /// </summary>
        public string frequency { get; set; }
        /// <summary>
        /// 无人机坐标
        /// </summary>
        public Gps gps { get; set; }
        public object[] gps_trajectory { get; set; }
        /// <summary>
        /// 页面显示无人机图片的文件名
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 无人机 ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 自动防御中时，提示此机器是否在防御等待队列中
        /// </summary>
        public bool in_defending_queue { get; set; }
        /// <summary>
        /// 是否可以对此机器开启精准定向
        /// </summary>
       // public bool is_df_trackable { get; set; }
        /// <summary>
        /// 此机器是否在精准定向中
        /// </summary>
       // public bool is_df_tracking { get; set; }
        /// <summary>
        /// 如果对此机器开启大功率干扰，是否需要双频同时打击
        /// </summary>
        public bool is_dual_wbj { get; set; }
        /// <summary>
        /// 是否是 rf 通信，false 的话代表是 wifi 机型
        /// </summary>
        public bool is_rf { get; set; }
        /// <summary>
        /// 机型
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 如果当前识别是无人机（模拟数据是识别到一对dr)，并且这个值不为空，则代表此无人机的配对遥控器也识别到了。
        /// </summary>
        public Paired_Rc paired_rc { get; set; }
        /// <summary>
        /// 此机型被哪个设备在哪个方向识别到的
        /// </summary>
        public Sectors1 sectors { get; set; }
        /// <summary>
        /// 识别到此机器的所有设备 ID
        /// </summary>
        public Sensors sensors { get; set; }
        /// <summary>
        /// DEBUG 用的信息，看信号强度
        /// </summary>
        public Signals signals { get; set; }
        /// <summary>
        /// 当前无人机探测状态： decrypted 代表已解密，whitelisted 代表加入白名单，decrypting 代表解密中，defending 代表防御中，unsupported 代表已探测但是无法解密比如 DIY
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 是否需要在加入该机型到白名单后，手动修改无人机下行频率，如果当前频率为 10mghz，请手动改成 20mghz。如果当前频率为 20mghz，请手动改成 10mghz。目前，只有 MAVIC2 需要作此操作，保证此机型可以被准备加入白名单
        /// </summary>
        public bool switch_for_whitelist { get; set; }
        /// <summary>
        /// 当前识别无人机来源，rc 代表遥控器，drone 代表无人机
        /// </summary>
        public string type { get; set; }
        public object wl_name { get; set; }//
    }

    public class Sensors
    {
        [JsonProperty("012.")]
        public _012 _012 { get; set; }
    }
    public class _012
    {
        public int distance { get; set; }
        public string sensor_name { get; set; }
    }
    public class Detect_Counter
    {
        public int drone { get; set; }
    }
    public class Gps
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Paired_Rc
    {
        public string id { get; set; }
        public float lat { get; set; }
        public float lng { get; set; }
        public Sectors sectors { get; set; }
        public string[] sensors { get; set; }
    }

    public class Sectors
    {
        [JsonProperty("SF1200001011")]
        public int devID { get; set; }
    }

    public class Sectors1
    {
        [JsonProperty("SF1200001011")]
        public int devID { get; set; }
    }

    public class Signals
    {
        public float[] drone { get; set; }
        public float[] rc { get; set; }
    }

    

    public class Gps1
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Sectors2
    {
        [JsonProperty("SF1200001011")]
        public int devID { get; set; }
    }

    public class Signals1
    {
        public float[] rc { get; set; }
    }

    public class Sensors_Info
    {
        [JsonProperty("SF1200001011")]
        public devs dev { get; set; }
    }

    public class devs
    {
        //public int altitude { get; set; }
        public string antenna { get; set; }//天线
        public string arch { get; set; }//
        public bool auto_defense_status { get; set; }
        public int capture_status { get; set; }
        public int defense_radius { get; set; }
        public int detection_radius { get; set; }
        public bool disk_has_space { get; set; }//
        public int dv_status { get; set; }
        public bool external_disk_mounted { get; set; }//
        public Gps2 gps { get; set; }
        public bool is_wideband_enabled { get; set; }
        public bool mass_defense_status { get; set; }
        public Mgmt_Details mgmt_details { get; set; }
        public string ou { get; set; }
        public bool sanity_test_status { get; set; }//正常测试状态
        public object sectors { get; set; }
        public object sectors_label { get; set; }
        //public Sector[] sectors { get; set; }
        //public Sectors_Label[] sectors_label { get; set; }
        public string sensor_name { get; set; }
        public string status { get; set; }
        //public bool test_mode_status { get; set; }
    }

    public class Gps2
    {
        public object altitude { get; set; }
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
        public string antenna_status { get; set; }
        public Detecting_Frequency detecting_frequency { get; set; }
        public string engine_status { get; set; }
        public string ip { get; set; }
        public bool is_compatible { get; set; }
        public string mcu_status { get; set; }
        public string mgmt_status { get; set; }
        public string software_build { get; set; }
        public string software_version { get; set; }
        public Som_Status som_status { get; set; }
        public string wifi_status { get; set; }
    }

    public class Detecting_Frequency
    {
        [JsonProperty("1")]
        public string[] subSystem1Freq { get; set; }
        [JsonProperty("2")]
        public string[] subSystem2Freq { get; set; }
    }

    public class Som_Status
    {
        [JsonProperty("1")]
        public string subSystemStatus { get; set; }
    }

    public class Sector
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Sectors_Label
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Upgrade_Status
    {
    }

}
