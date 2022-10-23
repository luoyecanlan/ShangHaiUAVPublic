using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using Bussiness.Metadata.Model;
using Bussiness.Metadata.Service;
using Bussiness.Reverse;
using Bussiness.Reverse.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntiUAV.WebApi.Controllers.Server
{
    /// <summary>
    /// 关联管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.MeatdataServer)]
    [Authorize(Roles = SystemRole.Admin)]
    public class MapController : ControllerBase
    {
        public MapController(IUserDeviceManagerService udMap, IDevicePreWarningZoneManagerService dpwzMap,
            IReverseCall call, IUserDeviceManagerService udService)
        {
            _udMap = udMap;
            _dpwzMap = dpwzMap;
            _call = call;
            _udService = udService;
        }

        private readonly IUserDeviceManagerService _udMap;
        private readonly IDevicePreWarningZoneManagerService _dpwzMap;
        private readonly IReverseCall _call;
        private readonly IUserDeviceManagerService _udService;

        /// <summary>
        /// 获取设备相关的用户
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <returns></returns>
        [HttpGet("d-u")]
        [ProducesResponseType(typeof(ServiceResponse<UserSimpleModel[]>), 200)]
        public async Task<IActionResult> GetUserByDevice([FromQuery] int did)
        {
            return Ok(await _udMap.GetDeviceMapUser(did));
        }

        /// <summary>
        /// 获取设备相关的服务用户信息
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <returns>服务用户信息</returns>
        [HttpGet("d-su")]
        [ProducesResponseType(typeof(ServiceResponse<UserSimpleModel>), 200)]
        public async Task<IActionResult> GetDevUserByDevice([FromQuery] int did)
        {
            return Ok(await _udMap.GetDeviceMapDevUser(did));
        }

        /// <summary>
        /// 获取用户相关的设备
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        [HttpGet("u-d")]
        [ProducesResponseType(typeof(ServiceResponse<DeviceSimpleModel[]>), 200)]
        public async Task<IActionResult> GetDeviceByUser([FromQuery] int uid)
        {
            return Ok(await _udMap.GetUserMapDevice(uid));
        }

        /// <summary>
        /// 建立非服务用户与设备的关联
        /// </summary>
        /// <param name="map">关系信息</param>
        /// <returns></returns>
        [HttpPost("u-d")]
        [ProducesResponseType(typeof(ServiceResponse), 200)]
        public async Task<IActionResult> MapUserToDevice([FromBody] MapUserToDeviceModel map)
        {
            if (await _udMap.UserMapDevice(map))
            {
                return Ok("成功.");
            }
            else
                return BadRequest("建立关联失败.");
        }

        /// <summary>
        /// 建立设备与服务用户的关联
        /// </summary>
        /// <param name="map">关系信息</param>
        /// <returns>关联ID</returns>
        [HttpPost("d-su")]
        [ProducesResponseType(typeof(ServiceResponse<int>), 200)]
        public async Task<IActionResult> MapDeviceToServUser([FromBody] MapDeviceToDevUserModel map)
        {
            var o_user = await _udService.GetDeviceMapDevUser(map.DevId);
            var id = await _udMap.DeviceMapDevUser(map);
            if (id >= 0)
            {
                var user = await _udService.GetDeviceMapDevUser(map.DevId);
                if (user != null && user.Uid == map.Uid)
                {
                    _ = _call.NoticeDeviceService(new DeviceNoticeModel() { DevUserId = user.Uid, DeviceId = map.DevId, Code = DeviceNoticeCode.DeviceChange });
                }
                if (o_user != null)
                {
                    _ = _call.NoticeDeviceService(new DeviceNoticeModel() { DevUserId = o_user.Uid, DeviceId = map.DevId, Code = DeviceNoticeCode.DeviceChange });
                }
                //_ = _call.CallDownDevice(new DeviceSetParameterModel<string>() { DeviceId = map.DevId, Code = DeviceWorkCode.DeviceInfoChange });//通知设备服务解绑该设备
                //_ = _call.CallDownDeviceUser(new DeviceUserNoticeModel<string>() { Uid = map.Uid, Code = DeviceUserNoticeCode.DeviceInfoChange, Data = map.DevId.ToString() });//通知用户绑定该设备
                return Ok(id);
            }
            else
            {
                return BadRequest("建立关联失败.");
            }
        }

        ///// <summary>
        ///// 删除用户与设备的关联
        ///// </summary>
        ///// <param name="id">关系ID</param>
        ///// <returns></returns>
        //[HttpDelete("ud")]
        //[ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        //public async Task<IActionResult> DelUserDeviceMap([FromQuery] int id)
        //{
        //    return Ok(await _udMap.DelMapToUser(id));
        //}

        ///// <summary>
        ///// 建立用户与设备的关联
        ///// </summary>
        ///// <param name="uid">用户ID</param>
        ///// <param name="did">设备ID</param>
        ///// <returns></returns>
        //[HttpPost("ud")]
        //[ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        //public async Task<IActionResult> UserDeviceMap([FromQuery] int uid, [FromQuery] int did)
        //{
        //    return Ok(await _udMap.MapToUser(did, uid) > 0);
        //}

        ///// <summary>
        ///// 建立用户与设备的关联(全量)
        ///// </summary>
        ///// <param name="uid">用户ID</param>
        ///// <param name="did">设备ID集合</param>
        ///// <returns></returns>
        //[HttpPost("ud-u")]
        //[ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        //public async Task<IActionResult> UserDeviceAllMapToDevice([FromQuery] int uid, [FromBody] int[] did)
        //{
        //    return Ok(await _udMap.MapToDevice(uid, did));
        //}

        ///// <summary>
        ///// 建立设备与用户的关联(全量)
        ///// </summary>
        ///// <param name="uid">用户ID</param>
        ///// <param name="did">设备ID</param>
        ///// <returns></returns>
        //[HttpPost("ud-d")]
        //[ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        //public async Task<IActionResult> UserDeviceAllMapToUser([FromQuery] int did, [FromBody] int[] uid)
        //{
        //    return Ok(await _udMap.MapToUser(did, uid));
        //}

        /// <summary>
        /// 获取设备相关的预警区
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <returns></returns>
        [HttpGet("d-pwz")]
        [ProducesResponseType(typeof(ServiceResponse<PerWarningSimpleZoneModel[]>), 200)]
        public async Task<IActionResult> GetPreWarningZoneByDevice([FromQuery] int did)
        {
            return Ok(await _dpwzMap.GetMapToDevice(did));
        }

        /// <summary>
        /// 获取预警区相关的设备
        /// </summary>
        /// <param name="pwzid">预警区ID</param>
        /// <returns></returns>
        [HttpGet("pwz-d")]
        [ProducesResponseType(typeof(ServiceResponse<DeviceSimpleModel[]>), 200)]
        public async Task<IActionResult> GetDeviceByPreWarningZone([FromQuery] int pwzid)
        {
            return Ok(await _dpwzMap.GetMapToZone(pwzid));
        }

        /// <summary>
        /// 设备关联预警区
        /// </summary>
        /// <param name="map">关系信息</param>
        /// <returns></returns>
        [HttpPost("d-pwz")]
        [ProducesResponseType(typeof(ServiceResponse), 200)]
        public async Task<IActionResult> DevicePreWarningZoneMap([FromBody] MapDeviceToZoneModel map)
        {
            if (await _dpwzMap.MapDeviceToZone(map))
            {
                var user = await _udService.GetDeviceMapDevUser(map.DevId);
                if (user != null)
                {
                    _ = _call.NoticeDeviceService(new DeviceNoticeModel() { DevUserId = user.Uid, DeviceId = map.DevId, Code = DeviceNoticeCode.PerWarningChange });
                }
                //_ = _call.CallDownDevice(new DeviceSetParameterModel<string>() { DeviceId = map.DevId, Code = DeviceWorkCode.PerWarningChange });
                return Ok("成功.");
            }
            else
                return BadRequest("建立关系失败.");
        }

        ///// <summary>
        ///// 删除预警区与设备的关联
        ///// </summary>
        ///// <param name="id">关系ID</param>
        ///// <returns></returns>
        //[HttpDelete("dpwz")]
        //[ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        //public async Task<IActionResult> DelDevicePreWarningZone([FromQuery] int id)
        //{
        //    return Ok(await _dpwzMap.DelMapDeviceToZoner(id));
        //}

        ///// <summary>
        ///// 建立预警区与设备的关联
        ///// </summary>
        ///// <param name="zid">预警区ID</param>
        ///// <param name="did">设备ID</param>
        ///// <returns></returns>
        //[HttpPost("dpwz")]
        //[ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        //public async Task<IActionResult> DevicePreWarningZoneMap([FromQuery] int zid, [FromQuery] int did)
        //{
        //    return Ok(await _dpwzMap.MapDeviceToZone(did, zid) > 0);
        //}
    }
}
