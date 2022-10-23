using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Config;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using DbOrm.AntiUAV.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AntiUAV.WebApi.Controllers
{
    /// <summary>
    /// 系统身份认证
    /// </summary>
    [Route("api")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Access)]
    public class UserAuthController : ControllerBase
    {
        private readonly AuthConfig _config;
        private readonly IUserService _user;
        private readonly ILogger<UserAuthController> _logger;
        public UserAuthController(AuthConfig config, IUserService user,ILogger<UserAuthController> logger)
        {
            _config = config;
            _user = user;
            _logger = logger;
        }
        /// <summary>
        /// 系统登录
        /// </summary>
        /// <param name="data">用户信息</param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<TokenModel>), 200)]
        [ProducesResponseType(typeof(ServiceResponse), 400)]
        public async Task<IActionResult> LoginAsync([FromBody]LoginInfoModel data)
        {
            if (string.IsNullOrEmpty(data?.UserName) || string.IsNullOrEmpty(data?.Password))
                return BadRequest("无效的用户名和密码");
            var info = (await _user.GetAnyAsync<UserAllInfo>(x => x.Username == data.UserName && x.Password == data.Password)).FirstOrDefault();
            if (info == null)
                return BadRequest("用户名或密码错误");
            //存在则直接返回可用的token信息
            var r_key = HttpContext.GetTokenCacheKey(info.Id.ToString());
            _logger.LogInformation($"用户{data.UserName}登录系统。");
            if (RedisHelper.Exists(r_key))
            {
                return Ok(RedisHelper.Get<TokenModel>(r_key));
            }
            return Ok(WriteToCache(info.Id, info.Role, info.Username));
        }
        /// <summary>
        /// 刷新token
        /// </summary>
        /// <returns></returns>
        [HttpPost("rf")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<TokenModel>), 200)]
        [ProducesResponseType(typeof(ServiceResponse), 400)]
        public IActionResult RefreshTokenAsync([FromQuery]string rftoken)
        {
            var old = RedisHelper.Get<TokenModel>(HttpContext.GetCurrentTokenCacheKey());            
            if (old?.refresh_token != rftoken) //验证刷新令牌是否是同一个
                return BadRequest("无效的刷新令牌.");
            var r_key = HttpContext.GetTokenCacheKey(old.uid.ToString());
            //否则需要重新生成
            var token = RefreshAndWriteToken(r_key, old.uid, old.rold, old.username);//从新生成token
            return Ok(token);
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        [ProducesResponseType(typeof(ServiceResponse), 400)]
        public IActionResult LogoutAsync()
        {
            var username = HttpContext.GetCurrentUsername();
            var count = RedisHelper.Del(HttpContext.GetCurrentTokenCacheKey());
            if (count > 0)
            {
                _logger.LogInformation($"用户{username}登出系统。");
                return Ok("已退出登录");
            }
            else
                return BadRequest("退出失败");
        }


        /// <summary>
        /// 获取当前登录的用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<UserInfo>), 200)]
        [ProducesResponseType(typeof(ServiceResponse), 400)]
        public async Task<IActionResult> GetUserAsync()
        {
            if (int.TryParse(HttpContext.GetCurrentUserId(), out int uid))
            {
                return Ok(await _user.GetAsync(uid));
            }
            return BadRequest("用户ID异常.");
        }

        /// <summary>
        /// 生成Token并写入缓存
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="role">角色</param>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        private TokenModel WriteToCache(int uid, string role, string username)
        {
            var r_key = HttpContext.GetTokenCacheKey(uid.ToString());
            var nbf = DateTime.Now;    //授权时间
            var exp = DateTime.Now.AddSeconds(_config.ExpireTime);  //过期时间
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
            RedisHelper.Set(r_key, token, token.expire_in + _config.ClockSkew);//延长5分钟刷新token
            return token;
        }
        /// <summary>
        /// token刷新锁
        /// </summary>
        private static readonly object tokenLock = new object();
        /// <summary>
        /// 刷新并写入token
        /// </summary>
        /// <param name="r_key"></param>
        /// <param name="uid"></param>
        /// <param name="role"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        private TokenModel RefreshAndWriteToken(string r_key,int uid, string role, string username)
        {
            lock (tokenLock)
            { 
                if (RedisHelper.Exists(r_key))
                {
                    var _token = RedisHelper.Get<TokenModel>(r_key);
                    //如果为五分钟内授权  则直接返回
                    if ((DateTime.Now - _token.nbf).TotalMilliseconds < 5)
                    {
                        return _token;
                    }
                    RedisHelper.Set($"{r_key}_old", _token, 20);
                }
                return WriteToCache(uid, role, username);
            }
        }
    }
}