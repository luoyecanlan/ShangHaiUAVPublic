using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using DbOrm.CRUD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AntiUAV.WebApi.Controllers
{


    /// <summary>
    /// 测试频谱web服务
    /// </summary>
    [Route("api/v3/system")]
    [ApiController]
    //[ApiGroup(ApiGroupNames.RealTime)]
    [Authorize(Roles = SystemRole.Super)]
    public class TestSpectrum : ControllerBase
    {
        public IDeviceService _devService;
        private readonly ILogger<DevController> _logger;
        private readonly INoticeDeviceService _notice;
        static string result = "{\"defense_status\":{\"defense_duration\":30},\"devices_info\":{\"00009ddr\":{\"bandwidth\":null,\"can_manual_defend\":false,\"defense_cancellable\":true,\"device_name\":\"p4v2_drone58\",\"finder\":\"df\",\"frequency\":5.8,\"gps\":{\"lat\":49.2398488173378,\"lng\":-122.65134998807083},\"icon\":\"paired_dji_phantom_4_pro_v2_mavic2\",\"id\":\"00009d\",\"in_defending_queue\":false,\"is_df_trackable\":true,\"is_df_tracking\":false,\"is_dual_wbj\":true,\"is_rf\":true,\"model\":\"DJIPhantom4ProV2/Mavic2\",\"paired_rc\":{\"id\":\"000042rc\",\"lat\":49.260427657252336,\"lng\":-122.6841905542442,\"sectors\":{\"SF1200001011\":16},\"sensors\":[\"SF1200001011\"]},\"sectors\":{\"SF1200001011\":15},\"sensors\":[\".011.\"],\"signals\":{\"drone\":[-23.0,-9999,-13.0,-19.0,-25.0,-24.0,-9999,-16.0],\"rc\":[-13.0,-33.0,-31.0,-29.0,-15.0,-25.0,-13.0,-25.0]},\"status\":\"decrypting\",\"switch_for_whitelist\":true,\"type\":\"drone\",\"wl_name\":null},\"00b09frc\":{\"bandwidth\":94300,\"can_manual_defend\":true,\"defense_cancellable\":true,\"device_name\":\"p4p_rc58\",\"finder\":\"df\",\"frequency\":5.8,\"gps\":{\"lat\":49.266846202673314,\"lng\":-122.68864953483121},\"icon\":\"dji_phantom_4_pro_rc\",\"id\":\"00b09f\",\"in_defending_queue\":false,\"is_df_trackable\":true,\"is_df_tracking\":false,\"is_dual_wbj\":false,\"is_rf\":true,\"model\":\"DJIPhantom4Pro/Inspire2\",\"paired_rc\":\"\",\"sectors\":{\"SF1200001011\":39},\"sensors\":[\".011.\"],\"signals\":{\"rc\":[-2.0,-9.0,-9999,-2.0,-6.0,-9999,-4.0,-2.0]},\"status\":\"decrypted\",\"switch_for_whitelist\":false,\"type\":\"rc\",\"wl_name\":null}},\"has_danger\":true,\"is_offline_enabled\":false,\"compass_info\":{\"SF1200001011\":{\"compass\":2}},\"sensors_info\":{\"SF1200001011\":{\"altitude\":0,\"auto_defense_status\":false,\"capture_status\":0,\"defense_radius\":3000,\"detection_radius\":7000,\"dv_status\":0,\"gps\":{\"lat\":49.2637738333,\"lng\":-122.687897833},\"is_wideband_enabled\":false,\"mass_defense_status\":false,\"mgmt_details\":{\"1\":{\"detecting_frequency\":{\"1\":[\"2.4GHz\",\"5.8GHz\"],\"2\":[\"2.4GHz\",\"5.8GHz\"]},\"engine_status\":\"connected\",\"ip\":\"10.8.1.30\",\"is_compatible\":true,\"mcu_status\":\"connected\",\"antenna_status\":\"connected\",\"mgmt_status\":\"connected\",\"software_build\":\"608\",\"software_version\":\"V3.4.1\",\"som_status\":{\"1\":\"connected\",},\"wifi_status\":\"connected\"}},\"sectors\":[{\"lat\":49.32655309309116,\"lng\":-122.69463352399568},{\"lat\":49.32646680624766,\"lng\":-122.67948208185783},{\"lat\":49.32483486554832,\"lng\":-122.6645386489112},{\"lat\":49.32169760521988,\"lng\":-122.65017251980657},{\"lat\":49.3171325530117,\"lng\":-122.63673856525519},{\"lat\":49.311252493850375,\"lng\":-122.62456838304423},{\"lat\":49.304202655508064,\"lng\":-122.6139620536984},{\"lat\":49.29615708926143,\"lng\":-122.60518071157144},{\"lat\":49.28731433894029,\"lng\":-122.59844011815045},{\"lat\":49.27789250946209,\"lng\":-122.59390539538862},{\"lat\":49.26812386046733,\"lng\":-122.5916870440952},{\"lat\":49.25824906168515,\"lng\":-122.59183833703297},{\"lat\":49.248511253967,\"lng\":-122.59435413963897},{\"lat\":49.23915006345152,\"lng\":-122.59917117437779},{\"lat\":49.230395716106656,\"lng\":-122.60616970872243},{\"lat\":49.2224633960739,\"lng\":-122.61517661255694},{\"lat\":49.21554798404015,\"lng\":-122.62596969915023},{\"lat\":49.209819301577646,\"lng\":-122.63828323532793},{\"lat\":49.20541797436049,\"lng\":-122.6518144814629},{\"lat\":49.202452011762624,\"lng\":-122.66623110066526},{\"lat\":49.20099418295706,\"lng\":-122.68117925920376},{\"lat\":49.20108025067283,\"lng\":-122.69629222677904},{\"lat\":49.202708103626534,\"lng\":-122.71119927578394},{\"lat\":49.20583780773163,\"lng\":-122.72553467310038},{\"lat\":49.21039257489952,\"lng\":-122.73894655626525},{\"lat\":49.2162606269799,\"lng\":-122.75110548798428},{\"lat\":49.223297911540214,\"lng\":-122.76171248898932},{\"lat\":49.23133160615529,\"lng\":-122.77050635915586},{\"lat\":49.24016432907012,\"lng\":-122.77727011064607},{\"lat\":49.24957895691468,\"lng\":-122.78183635462696},{\"lat\":49.259343934989715,\"lng\":-122.78409150478093},{\"lat\":49.26921895289835,\"lng\":-122.78397868623635},{\"lat\":49.2789608483397,\"lng\":-122.78149926743852},{\"lat\":49.2883295950435,\"lng\":-122.77671296444117},{\"lat\":49.29709422739441,\"lng\":-122.769736501541},{\"lat\":49.305038554487744,\"lng\":-122.76074084833897},{\"lat\":49.31196652030476,\"lng\":-122.74994709025586},{\"lat\":49.317707074421264,\"lng\":-122.73762102617214},{\"lat\":49.322118429079744,\"lng\":-122.72406662201463},{\"lat\":49.325091593345476,\"lng\":-122.70961848153526}],\"sectors_label\":[{\"lat\":49.31866279416521,\"lng\":-122.68621610298737},{\"lat\":49.31781459373454,\"lng\":-122.67306454686162},{\"lat\":49.31563430316791,\"lng\":-122.66027942884314},{\"lat\":49.312175780483955,\"lng\":-122.64817649960804},{\"lat\":49.30752444427992,\"lng\":-122.63705450257456},{\"lat\":49.30179514504998,\"lng\":-122.62718774633026},{\"lat\":49.2951293058289,\"lng\":-122.61881930644358},{\"lat\":49.28769140566272,\"lng\":-122.61215502832667},{\"lat\":49.27966489586538,\"lng\":-122.60735847991877},{\"lat\":49.271247652929866,\"lng\":-122.60454697630175},{\"lat\":49.262647082985794,\"lng\":-122.60378876892722},{\"lat\":49.25407500060485,\"lng\":-122.60510146095214},{\"lat\":49.24574240942131,\"lng\":-122.60845167825522},{\"lat\":49.237854313433566,\"lng\":-122.61375599399359},{\"lat\":49.23060468604913,\"lng\":-122.6208830739027},{\"lat\":49.224171719077624,\"lng\":-122.62965698067003},{\"lat\":49.21871346617638,\"lng\":-122.63986154921017},{\"lat\":49.2143639849788,\"lng\":-122.65124572098244},{\"lat\":49.21123006959198,\"lng\":-122.66352970494509},{\"lat\":49.209388650668856,\"lng\":-122.67641181553655},{\"lat\":49.20888492419162,\"lng\":-122.68957582434396},{\"lat\":49.209731252805746,\"lng\":-122.70269865191403},{\"lat\":49.2119068653817,\"lng\":-122.7154582195097},{\"lat\":49.215358361817835,\"lng\":-122.72754127751544},{\"lat\":49.22000101129792,\"lng\":-122.73865102764637},{\"lat\":49.22572081364716,\"lng\":-122.74851436011788},{\"lat\":49.23237727545228,\"lng\":-122.75688853449083},{\"lat\":49.23980683559296,\"lng\":-122.76356714400863},{\"lat\":49.24782685913245,\"lng\":-122.76838521786742},{\"lat\":49.256240104494964,\"lng\":-122.77122333392536},{\"lat\":49.26483955685885,\"lng\":-122.77201063572875},{\"lat\":49.27341351104826,\"lng\":-122.77072667216876},{\"lat\":49.281750780204405,\"lng\":-122.76740200523732},{\"lat\":49.289645902418954,\"lng\":-122.76211756073613},{\"lat\":49.2969042165113,\"lng\":-122.75500272779352},{\"lat\":49.30334668035861,\"lng\":-122.74623224489447},{\"lat\":49.3088143106839,\"lng\":-122.73602194195882},{\"lat\":49.313172131925015,\"lng\":-122.72462343883686},{\"lat\":49.316312533590704,\"lng\":-122.71231792941546},{\"lat\":49.31815795010287,\"lng\":-122.69940920631919}],\"sensor_name\":\"SF1200001011\",\"status\":\"detecting\",\"test_mode_status\":false}},\"total_devices\":2,\"upgrade_status\":{},\"version\":\"V3.4.1.236\",\"version_compatible\":\"V3.4.1.608\"}";
        static string newJson = "{\"compass_info\":{\"SF1300022012\":{\"compass\":206}},\"defense_status\":{\"defense_duration\":50},\"devices_info\":{\"000046dr\":{\"azimuth\":90.1,\"bandwidth\":null,\"can_manual_defend\":false,\"color_trajectory\":null,\"defense_cancellable\":true,\"detect_counter\":{\"drone\":20},\"device_name\":\"occusync_dr_10mhz_58\",\"finder\":\"df\",\"frequency\":\"5.8GHz\",\"gps\":{\"lat\":38.08196064716923,\"lng\":115.8059799800743},\"gps_trajectory\":[],\"icon\":\"dji_phantom_4_pro_v2_mavic2\",\"id\":\"000046\",\"in_defending_queue\":false,\"is_dual_wbj\":true,\"is_rf\":true,\"model\":\"DJI Phantom4 Pro V2 / Mavic2 (\\u7a84\\u5e26)\",\"paired_rc\":\"\",\"sectors\":{\"SF1300022012\":10},\"sensors\":{\"012.\":{\"distance\":2364,\"sensor_name\":\"SF1300022012\"}},\"signals\":{\"drone\":[-9999,-9999,-40,-9999,-9999,-9999,-9999,-9999]},\"status\":\"decrypting\",\"switch_for_whitelist\":true,\"type\":\"drone\",\"wl_name\":null}},\"has_danger\":true,\"hide_defense_features\":false,\"is_local_mode\":true,\"sensors_info\":{\"SF1300022012\":{\"antenna\":\"SA-1300-A-5\",\"arch\":\"ARM\",\"auto_defense_status\":false,\"capture_status\":0,\"defense_radius\":3000,\"detection_radius\":5000,\"disk_has_space\":true,\"dv_status\":0,\"external_disk_mounted\":true,\"gps\":{\"altitude\":null,\"lat\":38.082,\"lng\":115.7796},\"has_enhanced_detection\":false,\"is_df_enabled\":true,\"is_enhanced_sensor\":false,\"is_wideband_enabled\":false,\"mass_defense_status\":false,\"mgmt_details\":{\"1\":{\"detecting_frequency\":{\"1\":[\"2.4GHz\",\"5.8GHz\"],\"2\":[\"2.4GHz\",\"5.8GHz\"]},\"disk_usage\":\"3.2G/434.1G\",\"engine_status\":\"connected\",\"gps_pps_status\":\"disconnected\",\"ip\":null,\"is_compatible\":true,\"mcu_status\":\"connected\",\"mgmt_status\":\"connected\",\"software_build\":\"100\",\"software_version\":\"V3.7.0\",\"som_status\":{\"1\":\"connected\",\"2\":\"connected\"},\"wifi_status\":\"connected\"}},\"ou\":\"default\",\"sanity_test_status\":false,\"sectors\":null,\"sectors_label\":null,\"sensor_name\":\"SF1300022012\",\"status\":\"detecting\"}},\"show_enhanced_features\":false,\"show_library_features\":true,\"total_devices\":1,\"unknown_devices_info\":null,\"upgrade_status\":{},\"user_locations\":{},\"version\":\"V3.7.0.131\",\"version_compatible\":\"V3.7.0.100\"}";
        public TestSpectrum(IDeviceService service, ILogger<DevController> logger, INoticeDeviceService notice)
        {
            _devService = service;
            _logger = logger;
            _notice = notice;
        }
        /// <summary>
        /// 测试智控未来频谱
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [AllowAnonymous]
        //[ProducesResponseType(typeof(ServiceResponse<PagingModel<SpectruObject>>), 200)]
        public IActionResult Get()
        {

            return Ok(strJson());
        }
        //字符串转json
        public static SpectruObject strJson()
        {

            SpectruObject obj = JsonConvert.DeserializeObject<SpectruObject>(newJson);//或者JObject jo = JObject.Parse(jsonText);
            Random random = new Random();
            Double rand = random.NextDouble();
            rand *= 0.0000000001;
            obj.devices_info.dr.gps.lat += rand;
            obj.devices_info.dr.gps.lng += rand;
            //Compass_Info test =obj as Compass_Info;
            return obj;

        }

    }
    public class SpectruObject
    {
        public Compass_Info compass_info { get; set; }
        public Defense_Status defense_status { get; set; }
        public Devices_Info devices_info { get; set; }
        public bool has_danger { get; set; }
        public bool hide_defense_features { get; set; }
        public bool is_local_mode { get; set; }
        public Sensors_Info sensors_info { get; set; }
        public bool show_enhanced_features { get; set; }
        public bool show_library_features { get; set; }
        public int total_devices { get; set; }
        public object unknown_devices_info { get; set; }
        public Upgrade_Status upgrade_status { get; set; }
        public User_Locations user_locations { get; set; }
        public string version { get; set; }
        public string version_compatible { get; set; }
    }

    public class Compass_Info
    {
        public SF1300022012 SF1300022012 { get; set; }
    }

    public class SF1300022012
    {
        public int compass { get; set; }
    }

    public class Defense_Status
    {
        public int defense_duration { get; set; }
    }

    public class Devices_Info
    {
        [JsonProperty("000046dr")]
        public dr dr { get; set; }
    }

    public class dr
    {
        public float azimuth { get; set; }
        public object bandwidth { get; set; }
        public bool can_manual_defend { get; set; }
        public object color_trajectory { get; set; }
        public bool defense_cancellable { get; set; }
        public Detect_Counter detect_counter { get; set; }
        public string device_name { get; set; }
        public string finder { get; set; }
        public string frequency { get; set; }
        public Gps gps { get; set; }
        public object[] gps_trajectory { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public bool in_defending_queue { get; set; }
        public bool is_dual_wbj { get; set; }
        public bool is_rf { get; set; }
        public string model { get; set; }
        public string paired_rc { get; set; }
        public Sectors sectors { get; set; }
        public Sensors sensors { get; set; }
        public Signals signals { get; set; }
        public string status { get; set; }
        public bool switch_for_whitelist { get; set; }
        public string type { get; set; }
        public object wl_name { get; set; }
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

    public class Sectors
    {
        public int SF1300022012 { get; set; }
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

    public class Signals
    {
        public int[] drone { get; set; }
    }

    public class Sensors_Info
    {
        [JsonProperty("SF1300022012")]
        public SF13000220121 SF1300022012 { get; set; }
    }

    public class SF13000220121
    {
        public string antenna { get; set; }
        public string arch { get; set; }
        public bool auto_defense_status { get; set; }
        public int capture_status { get; set; }
        public int defense_radius { get; set; }
        public int detection_radius { get; set; }
        public bool disk_has_space { get; set; }
        public int dv_status { get; set; }
        public bool external_disk_mounted { get; set; }
        public Gps1 gps { get; set; }
        public bool has_enhanced_detection { get; set; }
        public bool is_df_enabled { get; set; }
        public bool is_enhanced_sensor { get; set; }
        public bool is_wideband_enabled { get; set; }
        public bool mass_defense_status { get; set; }
        public Mgmt_Details mgmt_details { get; set; }
        public string ou { get; set; }
        public bool sanity_test_status { get; set; }
        public object sectors { get; set; }
        public object sectors_label { get; set; }
        public string sensor_name { get; set; }
        public string status { get; set; }
    }

    public class Gps1
    {
        public object altitude { get; set; }
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Mgmt_Details
    {
        [JsonProperty("1")]
        public _1 _1 { get; set; }
    }

    public class _1
    {
        public Detecting_Frequency detecting_frequency { get; set; }
        public string disk_usage { get; set; }
        public string engine_status { get; set; }
        public string gps_pps_status { get; set; }
        public object ip { get; set; }
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
        public string[] _1 { get; set; }
        [JsonProperty("2")]
        public string[] _2 { get; set; }
    }

    public class Som_Status
    {
        [JsonProperty("1")]
        public string _1 { get; set; }
        [JsonProperty("2")]
        public string _2 { get; set; }
    }

    public class Upgrade_Status
    {
    }

    public class User_Locations
    {
    }
}