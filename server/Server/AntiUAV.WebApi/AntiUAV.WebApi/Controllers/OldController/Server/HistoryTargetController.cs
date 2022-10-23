using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using Bussiness.Metadata.Model;
using Bussiness.Metadata.Service;
using DbOrm.CRUD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntiUAV.WebApi.Controllers.Server
{
    /// <summary>
    /// 历史目标管理
    /// </summary>
    [Route("api/server/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.MeatdataServer)]
    [Authorize(Roles = SystemRole.Admin)]
    public class HistoryTargetController : ControllerBase
    {
        public HistoryTargetController(ITargetTrackService track)
        {
            _track = track;
        }

        private readonly ITargetTrackService _track;

        /// <summary>
        /// 获取历史目标总体情况
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(ServiceResponse<TargetHistoryProfileModel>), 200)]
        public async Task<IActionResult> GetProfile([FromQuery] int day = 30)
        {
            return Ok(await _track.GetHistoryProfile(day));
        }

        /// <summary>
        /// 获取历史目标数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<PagingModel<TargetHistoryModel>>), 200)]
        public async Task<IActionResult> Get([FromQuery]int pageindex, [FromQuery]int pagesize, [FromQuery]DateTime start, [FromQuery]DateTime end)
        {
            return Ok(await _track.GetHitoryTarget(pageindex, pagesize, start, end));
        }

        /// <summary>
        /// 清理历史目标数据
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(ServiceResponse<int>), 200)]
        public async Task<IActionResult> Clean([FromBody]string[] ids)
        {
            return Ok(await _track.CleanHitsoryTarget(ids));
        }
    }
}