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

namespace AntiUAV.WebApi.Controllers.Server
{
    /// <summary>
    /// 实时数据查看（单项需要有手动刷新（间隔不低于5s）和自动刷新（间隔10s））
    /// </summary>
    [Route("api/server/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.MeatdataServer)]
    [Authorize(Roles = SystemRole.Admin)]
    public class RealTimeController : ControllerBase
    {
        public RealTimeController(IDeviceRealTimeService realtime, ITargetTrackService target)
        {
            _realtime = realtime;
            _target = target;
        }

        private readonly IDeviceRealTimeService _realtime;
        private readonly ITargetTrackService _target;

        /// <summary>
        /// 获取所有在线设备
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>在线设备简要信息集合</returns>
        [HttpGet("device")]
        [ProducesResponseType(typeof(ServiceResponse<DeviceSimpleModel[]>), 200)]
        public async Task<IActionResult> GetDevice([FromQuery]string key)
        {
            return Ok(await _realtime.GetOnLineDevice(key));
        }

        /// <summary>
        /// 实时设备状态查看（单设备）
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <returns>设备状态</returns>
        [HttpGet("status/{did}")]
        [ProducesResponseType(typeof(ServiceResponse<Dictionary<string, string>>), 200)]
        public async Task<IActionResult> GetStatus([FromRoute]int did)
        {
            return Ok(await _realtime.GetDeviceStatus(did));
        }

        /// <summary>
        /// 实时设备目标查看（单设备）
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <returns>设备目标集合</returns>
        [HttpGet("targets/{did}")]
        [ProducesResponseType(typeof(ServiceResponse<TargetPositionModel[]>), 200)]
        public async Task<IActionResult> GetTargets([FromRoute]int did)
        {
            return Ok(await _target.GetLastUpdateTarget(did));
        }

        /// <summary>
        /// 实时目标详情（单目标）
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <param name="tgid">目标ID</param>
        /// <returns>实时目标详情</returns>
        [HttpGet("targets/{did}/{tgid}")]
        [ProducesResponseType(typeof(ServiceResponse<TargetInfoModel>), 200)]
        public async Task<IActionResult> GetTargetInfo([FromRoute]int did, [FromRoute]string tgid)
        {
            return Ok(await _target.GetTargetInfo(did, tgid));
        }
    }
}