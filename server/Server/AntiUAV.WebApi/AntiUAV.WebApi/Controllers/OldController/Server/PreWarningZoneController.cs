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
using DbOrm.AntiUAV.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntiUAV.WebApi.Controllers.Server
{
    /// <summary>
    /// 预警区管理
    /// </summary>
    [Route("api/server/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.MeatdataServer)]
    [Authorize(Roles = SystemRole.Admin)]
    public class PreWarningZoneController : ControllerBase
    {
        public PreWarningZoneController(IPreWarningZoneManagerService zone, IDevicePreWarningZoneManagerService dpwzMap,
            IReverseCall call, IUserDeviceManagerService udService)
        {
            _call = call;
            _zone = zone;
            _dpwzMap = dpwzMap;
            _udService = udService;
        }

        private readonly IReverseCall _call;
        private readonly IPreWarningZoneManagerService _zone;
        private readonly IDevicePreWarningZoneManagerService _dpwzMap;
        private readonly IUserDeviceManagerService _udService;

        /// <summary>
        /// 新建预警区
        /// </summary>
        /// <param name="device">预警区信息</param>
        /// <returns>新建的预警区信息</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ServiceResponse<PerWarningZoneInfo>), 200)]
        public async Task<IActionResult> Add([FromBody]PerWarningZoneAdd device)
        {
            return Ok(await _zone.Add(device));
        }

        /// <summary>
        /// 删除预警区
        /// </summary>
        /// <param name="zoneid">预警区ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> Del([FromQuery]int zoneid)
        {
            var devs = await _dpwzMap.GetMapToZone(zoneid);
            var count = await _zone.Del(zoneid);
            if (count > 0)
            {
                foreach (var dev in devs)
                {
                    var user = await _udService.GetDeviceMapDevUser(dev.DeviceId);
                    if (user != null)
                    {
                        _ = _call.NoticeDeviceService(new DeviceNoticeModel() { DevUserId = user.Uid, DeviceId = dev.DeviceId, Code = DeviceNoticeCode.PerWarningChange });
                    }
                    //_ = _call.CallDownDevice(new DeviceSetParameterModel<string>() { Code = DeviceWorkCode.PerWarningChange, DeviceId = dev.DeviceId });
                }
            }
            return Ok(count >= 0);
        }

        /// <summary>
        /// 更新预警区信息
        /// </summary>
        /// <param name="zone">待修改的预警区信息</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> Update([FromBody]PerWarningZoneUpdateModel zone)
        {
            var info = await _zone.Update(zone);
            if (info != null)
            {
                var devs = await _dpwzMap.GetMapToZone(info.Id);
                foreach (var dev in devs)
                {
                    var user = await _udService.GetDeviceMapDevUser(dev.DeviceId);
                    if (user != null)
                    {
                        _ = _call.NoticeDeviceService(new DeviceNoticeModel() { DevUserId = user.Uid, DeviceId = dev.DeviceId, Code = DeviceNoticeCode.PerWarningChange });
                    }
                    //_ = _call.CallDownDevice(new DeviceSetParameterModel<string>() { Code = DeviceWorkCode.PerWarningChange, DeviceId = dev.DeviceId });
                }
            }
            return Ok(info);
        }

        /// <summary>
        /// 更新预警区几何图形信息
        /// </summary>
        /// <param name="zone">待修改的预警区信息</param>
        /// <returns></returns>
        [HttpPut("geo")]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> Update([FromBody]PerWarningZoneGeoUpdateModel zone)
        {
            var info = await _zone.Update(zone);
            if (info != null)
            {
                var devs = await _dpwzMap.GetMapToZone(info.Id);
                foreach (var dev in devs)
                {
                    //var user = await _udService.GetDeviceMapDevUser(dev.DeviceId);
                    //if (user != null)
                    //{
                    //    _ = _call.NoticeDeviceService(new DeviceNoticeModel() { DevUserId = user.Uid, DeviceId = dev.DeviceId, Code = DeviceNoticeCode.PerWarningChange });
                    //}
                    _ = _udService.GetDeviceMapDevUser(dev.DeviceId).ContinueWith(user =>
                      {
                          if (user.Result != null)
                          {
                              _call.NoticeDeviceService(new DeviceNoticeModel() { DevUserId = user.Result.Uid, DeviceId = dev.DeviceId, Code = DeviceNoticeCode.PerWarningChange });
                          }
                      });
                    //_ = _call.CallDownDevice(new DeviceSetParameterModel<string>() { Code = DeviceWorkCode.PerWarningChange, DeviceId = dev.DeviceId });
                }
            }
            return Ok(info);
        }

        /// <summary>
        /// 获取预警区
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>预警区信息集合</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<PerWarningZoneInfo[]>), 200)]
        public async Task<IActionResult> Get([FromQuery]string key)
        {
            return Ok(await _zone.GetAny(key));
        }


        /// <summary>
        /// 获取预警区(简要信息)
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>设备信息集合</returns>
        [HttpGet("simple")]
        [ProducesResponseType(typeof(ServiceResponse<PerWarningSimpleZoneModel[]>), 200)]
        public async Task<IActionResult> GetSimple([FromQuery]string key)
        {
            return Ok(await _zone.GetSimpleAny(key));
        }
    }
}