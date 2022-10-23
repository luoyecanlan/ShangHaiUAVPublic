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

namespace AntiUAV.WebApi.Controllers.Client
{
    /// <summary>
    /// 元数据服务（客户端）
    /// </summary>
    [Route("api/client/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Client)]
    [Authorize(Roles = SystemRole.Client)]
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
        [HttpGet("device")]
        [ProducesResponseType(typeof(ServiceResponse<DeviceInfo[]>), 200)]
        public async Task<IActionResult> GetDevice()
        {
            var uid = Convert.ToInt32(HttpContext.GetCurrentUserId());
            var devs = (await _udMap.GetUserMapDevice(uid)).Select(x => x.DeviceId).ToArray();
            return Ok(await _device.GetAny(x => devs.Contains(x.Id)));
        }

        /// <summary>
        /// 获取预警区信息
        /// </summary>
        /// <returns>设备信息集合</returns>
        [HttpGet("zone")]
        [ProducesResponseType(typeof(ServiceResponse<PerWarningZoneInfo[]>), 200)]
        public async Task<IActionResult> GetZone()
        {
            var zones = new List<int>();
            var uid = Convert.ToInt32(HttpContext.GetCurrentUserId());
            var devs = (await _udMap.GetUserMapDevice(uid)).Select(x => x.DeviceId).ToArray();
            foreach (var dev in devs)
            {
                zones.AddRange((await _dpwzMap.GetMapToDevice(dev)).Select(x => x.Id).ToArray());
            }
            zones = zones.Distinct().ToList();
            if (zones?.Count() > 0)
                return Ok(await _pwz.GetAny(x => zones.Contains(x.Id)));
            return Ok(new PerWarningZoneInfo[0]);
        }

        /// <summary>
        /// 获取设备分类信息
        /// </summary>
        /// <returns>设备分类信息集合</returns>
        [HttpGet("category")]
        [ProducesResponseType(typeof(ServiceResponse<DeviceCategoryInfo[]>), 200)]
        public async Task<IActionResult> GetCategory()
        {
            return Ok(await _device.GetCategory());
        }
    }
}