using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using Bussiness.Metadata.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Controllers
{
    /// <summary>
    /// 令牌管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Token)]
    [Authorize(Roles = SystemRole.Super)]
    //[EnableCors("antiuav")]
    public class TokenController : ControllerBase
    {
        /// <summary>
        /// 获取所有令牌
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<TokenModel[]>), 200)]
        public async Task<IActionResult> GetAllTokenAsync()
        {
            var keys = await RedisHelper.KeysAsync("Token_*");
            return Ok(await RedisHelper.MGetAsync<TokenModel>(keys));
        }

        /// <summary>
        /// 强制失效令牌
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(ServiceResponse), 200)]
        public async Task<IActionResult> DelTokenAsync([FromQuery]string userid)
        {
            var key = HttpContext.GetTokenCacheKey(userid);
            if (RedisHelper.Exists(key))
            {
                if (await RedisHelper.DelAsync(key) > 0)
                    return Ok("已删除令牌.");
            }
            return BadRequest($"待删除的令牌已失效.");
        }
    }
}