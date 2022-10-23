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
    /// 设备管理
    /// </summary>
    [Route("api/server/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.MeatdataServer)]
    [Authorize(Roles = SystemRole.Admin)]
    public class DeviceController : ControllerBase
    {
        public DeviceController(IDeviceManagerService device, IReverseCall call, IUserDeviceManagerService udService)
        {
            _device = device;
            _call = call;
            _udService = udService;
        }

        private readonly IDeviceManagerService _device;
        private readonly IReverseCall _call;
        private readonly IUserDeviceManagerService _udService;


        /// <summary>
        /// 新建设备
        /// </summary>
        /// <param name="device">设备信息</param>
        /// <returns>新建的设备信息</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ServiceResponse<DeviceInfo>), 200)]
        public async Task<IActionResult> Add([FromBody]DeviceAdd device)
        {
            return Ok(await _device.Add(device));
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> Del([FromQuery]int did)
        {
            var user = await _udService.GetDeviceMapDevUser(did);
            var count = await _device.Del(did);
            if (count > 0 && user != null)
            {
                _ = _call.NoticeDeviceService(new DeviceNoticeModel() { DevUserId = user.Uid, DeviceId = did, Code = DeviceNoticeCode.DeviceChange });
                //_ = _call.CallDownDevice(new DeviceSetParameterModel<string>() { Code = DeviceWorkCode.DeviceInfoChange, DeviceId = did });
            }
            return Ok(count >= 0);
        }

        /// <summary>
        /// 更新设备信息
        /// </summary>
        /// <param name="device">待修改的设备信息</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> Update([FromBody]DeviceUpdate device)
        {
            var info = await _device.Update(device);
            if (info != null)
            {
                var user = await _udService.GetDeviceMapDevUser(device.Id);
                if (user != null)
                {
                    _ = _call.NoticeDeviceService(new DeviceNoticeModel() { DevUserId = user.Uid, DeviceId = device.Id, Code = DeviceNoticeCode.DeviceInfoChange });
                }
                //_ = _call.CallDownDevice(new DeviceSetParameterModel<string>() { Code = DeviceWorkCode.DeviceInfoChange, DeviceId = info.Id });
            }
            return Ok(info);
        }

        /// <summary>
        /// 获取设备
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>设备信息集合</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<DeviceInfo[]>), 200)]
        public async Task<IActionResult> Get([FromQuery]string key)
        {
            return Ok(await _device.GetAny(key));
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

        /// <summary>
        /// 获取设备(简要信息)
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>设备信息集合</returns>
        [HttpGet("simple")]
        [ProducesResponseType(typeof(ServiceResponse<DeviceSimpleModel[]>), 200)]
        public async Task<IActionResult> GetSimple([FromQuery]string key)
        {
            return Ok(await _device.GetSimpleAny(key));
        }
    }
}
