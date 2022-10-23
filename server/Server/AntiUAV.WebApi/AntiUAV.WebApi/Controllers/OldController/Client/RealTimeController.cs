using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using Bussiness.Metadata.Model;
using Bussiness.Metadata.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntiUAV.WebApi.Controllers.Client
{
    /// <summary>
    /// 实时数据服务（客户端）
    /// </summary>
    [Route("api/client/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Client)]
    [Authorize(Roles = SystemRole.Client)]
    public class RealTimeController : ControllerBase
    {
        public RealTimeController(IDeviceRealTimeService realtime, ITargetTrackService target, IUserDeviceManagerService udMap)
        {
            _realtime = realtime;
            _target = target;
            _udMap = udMap;
        }

        private readonly IDeviceRealTimeService _realtime;
        private readonly ITargetTrackService _target;
        private readonly IUserDeviceManagerService _udMap;

        /// <summary>
        /// 获取所有设备简要状态信息
        /// </summary>
        /// <returns>设备简要状态信息</returns>
        [HttpGet("status")]
        [ProducesResponseType(typeof(ServiceResponse<DeviceStatusModel[]>), 200)]
        public async Task<IActionResult> GetStatus()
        {
            var uid = HttpContext.GetCurrentUserId();
            var devs = await _udMap.GetUserMapDevice(Convert.ToInt32(uid));//当前用户关联设备
            return Ok(await _realtime.GetDeviceSimpleStatus(devs));
        }

        /// <summary>
        /// 获取单设备状态
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <returns></returns>
        [HttpGet("status/{did}")]
        [ProducesResponseType(typeof(ServiceResponse<Dictionary<string, string>>), 200)]
        public async Task<IActionResult> GetStatus([FromRoute] int did)
        {
            return Ok(await _realtime.GetDeviceStatus(did));
        }

        /// <summary>
        /// 获取单设备所有目标
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <param name="sec">最后N秒（若N=0则将所有目标返回）</param>
        /// <returns></returns>
        [HttpGet("targets/{did}")]
        [ProducesResponseType(typeof(ServiceResponse<TargetPositionModel[]>), 200)]
        public async Task<IActionResult> GetTgs([FromRoute] int did, [FromQuery] int sec)
        {
            if (sec > 0)
                return Ok(await _target.GetLastUpdateTarget(did, sec));//最后N秒内目标
            else
                return Ok(await _target.GetLastUpdateTarget(did));//当前最后目标
        }

        /// <summary>
        /// 获取目标详情
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <param name="tgid">目标ID</param>
        /// <returns></returns>
        [HttpGet("targets/{did}/{tgid}")]
        [ProducesResponseType(typeof(ServiceResponse<TargetInfoModel[]>), 200)]
        public async Task<IActionResult> GetTgInfo([FromRoute] int did, [FromRoute] string tgid)
        {
            return Ok(await _target.GetTargetInfo(did, tgid));
        }
    }
}