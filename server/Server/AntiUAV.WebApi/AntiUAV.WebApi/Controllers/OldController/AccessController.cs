using AntiUAV.WebApi.Config;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using Bussiness.Metadata.Model;
using Bussiness.Metadata.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Bussiness.Metadata;
using Microsoft.AspNetCore.Cors;

namespace AntiUAV.WebApi.Controllers
{
    /// <summary>
    /// 访问令牌管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.AccessToken)]
    [Authorize(Roles = SystemRole.All)]
    //[EnableCors("antiuav")]
    public class AccessController : ControllerBase
    {
        public AccessController(AuthConfig config, IUserManagerService user)
        {
            _config = config;
            _user = user;
        }

        private readonly AuthConfig _config;
        private readonly IUserManagerService _user;

        /// <summary>
        /// 身份认证
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <response code="200">返回访问令牌</response>
        /// <response code="400">用户名密码错误</response>
        /// <response code="401">未登录</response>
        /// <response code="403">未授权</response>
        /// <response code="505">未知错误</response>
        /// <response code="507">业务处理错误</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<TokenModel>), 200)]
        [ProducesResponseType(typeof(ServiceResponse), 400)]
        public async Task<IActionResult> LoginAsync([FromQuery]string username, [FromQuery]string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return BadRequest("无效的用户名密码.");

            var info = await _user.Get(username, password);
            if (info == null)
                return BadRequest("用户名密码错误.");
            //生成新的token
            return Ok(await WriteToCacheAsync(info.Uid, info.Role, info.Username));
        }

        /// <summary>
        /// 令牌注销
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ServiceResponse), 200)]
        public async Task<IActionResult> LogoutAsync()
        {
            var count = await RedisHelper.DelAsync(HttpContext.GetCurrentTokenCacheKey());
            if (count > 0)
                return Ok("已登出.");
            else
                return BadRequest("登出失败.");
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="reftoken">刷新令牌</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(ServiceResponse<TokenModel>), 200)]
        public async Task<IActionResult> GetTokenAsync([FromQuery] string reftoken)
        {
            var old = await RedisHelper.GetAsync<TokenModel>(HttpContext.GetCurrentTokenCacheKey());
            if (old?.refresh_token != reftoken) //验证刷新令牌是否是同一个
                return BadRequest("无效的刷新令牌.");
            var token = await WriteToCacheAsync(old.uid, old.rold, old.username);//从新生成token
            return Ok(token);
        }

        /// <summary>
        /// 生成Token并写入缓存
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="role">角色</param>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        private async Task<TokenModel> WriteToCacheAsync(int uid, string role, string username)
        {
            var nbf = DateTime.Now;
            var exp = DateTime.Now.AddSeconds(_config.ExpireTime);
            var claims = new[]
                   {
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(nbf).ToUnixTimeSeconds()}") ,
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(exp).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.NameIdentifier,uid.ToString()),
                    new Claim(ClaimTypes.Role,role)
                   };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: claims,
                expires: exp,
                signingCredentials: creds);
            var ref_token = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            var acc_token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var token = new TokenModel
            {
                uid = uid,
                rold = role,
                username = username,
                access_token = acc_token,
                expire_in = _config.ExpireTime,
                nbf = nbf,
                refresh_token = ref_token
            };
            await RedisHelper.SetAsync(HttpContext.GetTokenCacheKey(token.uid.ToString()), token, token.expire_in + _config.ClockSkew);//延长5分钟刷新token
            return token;
        }
    }
}