using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using Bussiness.Metadata.Model;
using Bussiness.Metadata.Service;
using DbOrm.AntiUAV.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntiUAV.WebApi.Controllers.Device
{
    /// <summary>
    /// 元数据服务（设备端）
    /// </summary>
    [Route("api/device/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Device)]
    [Authorize(Roles = SystemRole.Device)]
    public class MetadataController : ControllerBase
    {
        public MetadataController(IUserDeviceManagerService udMap, IDeviceManagerService device,
            IDevicePreWarningZoneManagerService dpwzMap, IPreWarningZoneManagerService pwz)
        {
            _udMap = udMap;
            _device = device;
            _dpwzMap = dpwzMap;
            _pwz = pwz;
        }

        public readonly IUserDeviceManagerService _udMap;
        public readonly IDeviceManagerService _device;
        public readonly IDevicePreWarningZoneManagerService _dpwzMap;
        public readonly IPreWarningZoneManagerService _pwz;

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <returns>设备信息集合</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<DeviceInfo[]>), 200)]
        public async Task<IActionResult> GetDevice()
        {
            var uid = Convert.ToInt32(HttpContext.GetCurrentUserId());
            var devs = (await _udMap.GetUserMapDevice(uid)).Select(x => x.DeviceId).ToList();
            return Ok(await _device.GetAny(x => devs.Contains(x.Id)));
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="did"></param>
        /// <returns>设备信息集合</returns>
        [HttpGet("info")]
        [ProducesResponseType(typeof(ServiceResponse<DeviceInfo>), 200)]
        public async Task<IActionResult> GetDevice([FromQuery] int did)
        {
            var uid = Convert.ToInt32(HttpContext.GetCurrentUserId());
            var devs = await _udMap.GetUserMapDevice(uid);
            if (devs.Any(y => y.DeviceId == did))
                return Ok(await _device.Get(did));
            return BadRequest("无此资源访问权限,请联系管理员.");
        }

        /// <summary>
        /// 获取预警区信息
        /// </summary>
        /// <returns>设备信息集合</returns>
        [HttpGet("zone")]
        [ProducesResponseType(typeof(ServiceResponse<PerWarningZoneInfo[]>), 200)]
        public async Task<IActionResult> GetZone([FromQuery] int did)
        {
            var zones = (await _dpwzMap.GetMapToDevice(did)).Select(x => x.Id).ToList();
            return Ok(await _pwz.GetAny(x => zones.Contains(x.Id)));
        }
    }
}