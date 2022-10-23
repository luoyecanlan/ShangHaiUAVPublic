using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.WebApi.Config;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using Bussiness.Metadata.Model;
using Bussiness.Metadata.Service;
using DbOrm.AntiUAV.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AntiUAV.WebApi.Controllers.Server
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [Route("api/server/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.MeatdataServer)]
    [Authorize(Roles = SystemRole.Admin)]
    public class UserController : ControllerBase
    {
        public UserController(IUserManagerService user, UserConfig config)
        {
            _user = user;
            _config = config;
        }

        private readonly IUserManagerService _user;
        private readonly UserConfig _config;

        /// <summary>
        /// 新建用户
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>新建的用户信息</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ServiceResponse<UserInfoModel>), 200)]
        public async Task<IActionResult> Add([FromBody]UserAdd user)
        {
            user.Password = _config.NewPwd.Get32MD5();
            return Ok(await _user.Add(user));
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <returns>操作是否成功</returns>
        [HttpDelete]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> Del([FromQuery]int uid)
        {
            var count = await _user.Del(uid);
            if (count > 0)
            {
                //需要删除对应的token
                var key = HttpContext.GetTokenCacheKey(uid.ToString());
                if (RedisHelper.Exists(key))
                {
                    await RedisHelper.DelAsync(key);
                }
            }
            return Ok(count >= 0);
        }

        /// <summary>
        /// 更新用户角色
        /// </summary>
        /// <param name="user">待修改的角色信息</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> UpdateRole([FromBody]UserRoleUpdate user)
        {
            if (user?.Role == SystemRole.Super && HttpContext.GetCurrentUserRole() != SystemRole.Super)
            {
                return BadRequest("无此角色修改权限,如有需要,请联系\"超级管理员\".");
            }
            return Ok(await _user.Update(user));
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost("{uid}")]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> RestPwd([FromRoute]int uid)
        {
            return Ok(await _user.RestPassword(uid, _config.DefaultPwd.Get32MD5()));
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>用户信息集合</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<UserInfoModel[]>), 200)]
        public async Task<IActionResult> Get([FromQuery]string key)
        {
            return Ok(await _user.GetAny(key));
        }


        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <returns>用户信息集合</returns>
        [HttpGet("{uid}")]
        [ProducesResponseType(typeof(ServiceResponse<UserInfoModel>), 200)]
        public async Task<IActionResult> Get([FromRoute]int uid)
        {
            return Ok(await _user.Get(uid));
        }

        /// <summary>
        /// 获取用户(简要信息)
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>用户信息集合</returns>
        [HttpGet("simple")]
        [ProducesResponseType(typeof(ServiceResponse<UserSimpleModel[]>), 200)]
        public async Task<IActionResult> GetSimple([FromQuery]string key)
        {
            return Ok(await _user.GetSimpleAny(key));
        }
    }
}