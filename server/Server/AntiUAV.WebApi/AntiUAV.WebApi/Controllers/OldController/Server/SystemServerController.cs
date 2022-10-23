using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using Bussiness.Metadata.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntiUAV.WebApi.Controllers.Server
{
    /// <summary>
    /// 系统服务查看
    /// </summary>
    [Route("api/server/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.MeatdataServer)]
    [Authorize(Roles = SystemRole.Admin)]
    public class SystemServerController : ControllerBase
    {
        /// <summary>
        /// 获取当前在线的服务
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>服务集合</returns>
        [HttpGet()]
        [ProducesResponseType(typeof(ServiceResponse<string[]>), 200)]
        public async Task<IActionResult> Get([FromQuery]string key)
        {
            return Ok(await Task.FromResult($"未完成{key}"));
        }
    }
}