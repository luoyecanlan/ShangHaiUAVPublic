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

namespace AntiUAV.WebApi.Controllers.Device
{
    /// <summary>
    /// 实时数据服务
    /// </summary>
    [Route("api/device/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Device)]
    [Authorize(Roles = SystemRole.Device)]
    public class RealTimeController : ControllerBase
    {
        public RealTimeController(IDeviceRealTimeService realtime)
        {
            _realtime = realtime;
        }

        private readonly IDeviceRealTimeService _realtime;

        /// <summary>
        /// 上报航迹
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <param name="tgs">更新目标</param>
        /// <returns></returns>
        [HttpPost("target/{did}")]
        [ProducesResponseType(typeof(ServiceResponse), 200)]
        public async Task<IActionResult> UpdateTrack([FromRoute] int did,[FromBody]TargetReportModel[] tgs)
        {
            await _realtime.ReportDeviceProbeTrack(did, tgs);
            return Ok($"设备{did}更新航迹上报成功.");
        }

        /// <summary>
        /// 上报状态
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <param name="status">更新状态</param>
        /// <returns></returns>
        [HttpPost("status/{did}")]
        [ProducesResponseType(typeof(ServiceResponse), 200)]
        public async Task<IActionResult> UpdateStatus([FromRoute] int did, [FromBody]Dictionary<string,string> status)
        {
            await _realtime.ReportDeviceStatus(did, status);
            return Ok($"设备{did}状态上报成功");
        }

        /// <summary>
        /// 上报目标消失
        /// </summary>
        /// <param name="did">设备ID</param>
        /// <param name="ids">消失目标ID集合</param>
        /// <returns></returns>
        [HttpPost("disappear/{did}")]
        [ProducesResponseType(typeof(ServiceResponse), 200)]
        public async Task<IActionResult> DisappearTrack([FromRoute] int did, [FromBody]string[] ids)
        {
            await _realtime.ReportDisappearTrack(did, ids);
            return Ok($"设备{did}消失航迹上报成功");
        }
    }
}