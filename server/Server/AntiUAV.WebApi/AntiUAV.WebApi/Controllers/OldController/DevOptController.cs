using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using Bussiness.Metadata.Model;
using Bussiness.Reverse;
using Bussiness.Reverse.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Bussiness.Metadata.Service;

namespace AntiUAV.WebApi.aa.Controllers
{
    /// <summary>
    /// 设备操作服务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.DeviceOpt)]
    [Authorize(Roles = SystemRole.DeviceOperation)]
    public class DevOptController : ControllerBase
    {
        public DevOptController(IReverseCall call, IUserDeviceManagerService udService)
        {
            _call = call;
            _udService = udService;
        }

        private readonly IReverseCall _call;
        private readonly IUserDeviceManagerService _udService;


        private async Task<UserSimpleModel> GetMapUserAsync(int did)
        {
            var user = await _udService.GetDeviceMapDevUser(did);
            if (user == null)
                throw new Exception($"设备{did}未应用到任何服务,无法执行此操作.");
            return user;
        }

        /// <summary>
        /// 开始工作
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <param name="param">启动参数</param>
        /// <returns></returns>
        [HttpPost("work/{did}")]
        [ProducesResponseType(typeof(ServiceResponse), 200)]
        public async Task<IActionResult> WorkStartAsync([FromRoute]int did, [FromBody] object param)
        {
            var user = await GetMapUserAsync(did);
            if (await _call.OrderDeviceService(new DeviceNoticeModel()
            {
                DeviceId = did,
                DevUserId = user.Uid,
                Code = DeviceNoticeCode.WorkStart,
                Data = JsonSerializer.Serialize(param)
            }))
                return Ok("开始工作指令下发成功");
            else
                return BadRequest("操作失败.");
            //if (await _call.CallDownDevice(new DeviceSetParameterModel<string>()
            //{
            //    DeviceId = did,
            //    Code = DeviceWorkCode.WorkStart,
            //    Data = JsonSerializer.Serialize(param)
            //}))
            //    return Ok("开始工作指令下发成功");
            //else
            //    return BadRequest("操作失败.");
        }

        /// <summary>
        /// 停止工作
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <param name="param">停止参数</param>
        /// <returns></returns>
        [HttpPut("work/{did}")]
        [ProducesResponseType(typeof(ServiceResponse), 200)]
        public async Task<IActionResult> WorkStopAsync([FromRoute]int did, [FromBody] object param)
        {
            var user = await GetMapUserAsync(did);
            if (await _call.OrderDeviceService(new DeviceNoticeModel()
            {
                DeviceId = did,
                DevUserId = user.Uid,
                Code = DeviceNoticeCode.WorkStop,
                Data = JsonSerializer.Serialize(param)
            }))
                return Ok("停止工作指令下发成功");
            else
                return BadRequest("操作失败.");
            //if (await _call.CallDownDevice(new DeviceSetParameterModel<string>()
            //{
            //    DeviceId = did,
            //    Code = DeviceWorkCode.WorkStop,
            //    Data = JsonSerializer.Serialize(param)
            //}))
            //    return Ok("停止工作指令下发成功");
            //else
            //    return BadRequest("操作失败.");
        }

        /// <summary>
        /// 开始引导
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <param name="tgid">目标ID</param>
        /// <param name="tgdid">目标来源设备ID</param>
        /// <returns></returns>
        [HttpPost("guidance/{did}")]
        [ProducesResponseType(typeof(ServiceResponse), 200)]
        public async Task<IActionResult> GuidanceStartAsync([FromRoute]int did, [FromQuery]string tgid, [FromQuery]int tgdid)
        {
            var user = await GetMapUserAsync(did);
            if (await _call.OrderDeviceService(new DeviceNoticeModel()
            {
                DeviceId = did,
                DevUserId = user.Uid,
                Code = DeviceNoticeCode.GuidanceStar,
                Data = JsonSerializer.Serialize(new DeviceGuidanceSetModel() { FromDeviceId = tgdid, ToDeviceId = did, TargetId = tgid })
            }))
                return Ok("开始引导指令下发成功");
            else
                return BadRequest("操作失败.");
            //if (await _call.CallDownDevice(new DeviceSetParameterModel<string>()
            //{
            //    DeviceId = did,
            //    Code = DeviceWorkCode.GuidanceStar,
            //    Data = $"{ tgid},{tgdid }"
            //}))
            //    return Ok("开始引导指令下发成功");
            //else
            //    return BadRequest("操作失败.");
        }

        /// <summary>
        /// 停止引导
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <returns></returns>
        [HttpPut("guidance/{did}")]
        [ProducesResponseType(typeof(ServiceResponse), 200)]
        public async Task<IActionResult> GuidanceStopAsync([FromRoute]int did)
        {
            var user = await GetMapUserAsync(did);
            if (await _call.OrderDeviceService(new DeviceNoticeModel()
            {
                DeviceId = did,
                DevUserId = user.Uid,
                Code = DeviceNoticeCode.GuidanceStop
            }))
                return Ok("停止引导指令下发成功");
            else
                return BadRequest("操作失败.");
            //if (await _call.CallDownDevice(new DeviceSetParameterModel<string>()
            //{
            //    DeviceId = did,
            //    Code = DeviceWorkCode.GuidanceStop
            //}))
            //    return Ok("停止引导指令下发成功");
            //else
            //    return BadRequest("操作失败.");
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <param name="setting">设置参数</param>
        /// <returns></returns>
        [HttpPost("{did}")]
        [ProducesResponseType(typeof(ServiceResponse), 200)]
        public async Task<IActionResult> ParameterSettingAsync([FromRoute]int did, [FromBody]object setting)
        {
            var user = await GetMapUserAsync(did);
            if (await _call.OrderDeviceService(new DeviceNoticeModel()
            {
                DeviceId = did,
                DevUserId = user.Uid,
                Code = DeviceNoticeCode.RunParams,
                Data = JsonSerializer.Serialize(setting)
            }))
                return Ok("设置参数指令下发成功");
            else
                return BadRequest("操作失败.");
            //if (await _call.CallDownDevice(new DeviceSetParameterModel<string>()
            //{
            //    DeviceId = did,
            //    Code = DeviceWorkCode.RunParams,
            //    Data = JsonSerializer.Serialize(setting)
            //}))
            //    return Ok("设置参数指令下发成功");
            //else
            //    return BadRequest("操作失败.");
        }
    }
}