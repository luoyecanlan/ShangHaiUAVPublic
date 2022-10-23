using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using DbOrm.CRUD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiUAV.WebApi.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Token)]
    [Authorize(Roles = SystemRole.Super)]
    public class TokenController : ControllerBase
    {
        /// <summary>
        /// 获取全部token
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="size">分页大小</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<PagingModel<TokenModel>>), 200)]
        public IActionResult Get([FromQuery]int index,[FromQuery]int size)
        {
            var keys = RedisHelper.Keys("Token_*");
            var tokens= RedisHelper.MGet<TokenModel>(keys);
            return Ok(new PagingModel<TokenModel>(index, size) { Data = tokens.Skip((index - 1) * size).Take(size) });
        }

        /// <summary>
        /// 注销token
        /// </summary>
        /// <param name="id">token编号</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public IActionResult Distory([FromRoute]string id)
        {
            var key = HttpContext.GetTokenCacheKey(id);
            if (RedisHelper.Exists(key))
            {
                if (RedisHelper.Del(key) > 0)
                    return Ok("已删除令牌.");
            }
            return BadRequest($"待删除的令牌已失效.");
        }
    }
}