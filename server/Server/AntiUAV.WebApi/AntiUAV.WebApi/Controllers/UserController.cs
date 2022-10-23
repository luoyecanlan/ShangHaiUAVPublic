using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Config;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using DbOrm.AntiUAV.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiUAV.WebApi.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Meatdata)]
    public class UserController : ControllerBase
    {
        public UserController(IUserService user, UserConfig config)
        {
            _user = user;
            _config = config;
        }

        private readonly IUserService _user;
        private readonly UserConfig _config;

        /// <summary>
        /// 获取全部用户信息
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="size">页大小</param>
        /// <param name="key">检索关键字(用户名,昵称模糊查询),查询全部时传''</param>
        /// <param name="desc">是否倒序（默认false）</param>
        /// <returns>用户信息</returns>
        [HttpGet]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<UserInfo[]>), 200)]
        public async Task<IActionResult> Get([FromQuery]int index, [FromQuery]int size, [FromQuery]string key, [FromQuery]bool desc = false)
        {
            if (string.IsNullOrEmpty(key))
                return Ok(await _user.GetAnyAsync(size, index, null, keySelector => keySelector.Id, desc));
            else
                return Ok(await _user.GetAnyAsync(size, index,
                    info => info.Name.Contains(key) || info.Username.Contains(key) || info.Phone.Contains(key),
                    keySelector => keySelector.Id, desc));
        }

        /// <summary>
        /// 获取指定用户详细信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<UserInfo>), 200)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            return Ok(await _user.GetAsync(id));
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="add">新增用户数据</param>
        /// <returns>用户信息</returns>
        [HttpPost]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<UserInfo>), 200)]
        public async Task<IActionResult> Add([FromBody]UserAdd add)
        {
            if (add == null) return BadRequest("新增用户信息不能为空");
            add.Password = _config.NewPwd.Get32MD5();
            return Ok(await _user.AddAsync(add));
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> Reset([FromRoute]int id)
        {
            return Ok(await _user.RestPassword(id, _config.DefaultPwd.Get32MD5()));
        }

        /// <summary>
        ///  删除用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> Del([FromRoute]int id)
        {
            return Ok(await _user.DelAsync(id));
        }

        /// <summary>
        /// 修改用户基本信息(当前用户)
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> UpdateInfo([FromBody]UserUpdate update)
        {
            if (int.TryParse(HttpContext.GetCurrentUserId(), out int uid))
            {
                if (update != null)
                {
                    update.Id = uid;
                    return Ok(await _user.UpdateAsync(update));
                }
            }
            return BadRequest("用户ID异常.");
        }

        /// <summary>
        /// 修改用户密码(当前用户)
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [HttpPost("pwd")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> UpdatePwd([FromBody]UserPwdUpdate update)
        {
            if (int.TryParse(HttpContext.GetCurrentUserId(), out int uid))
            {
                if (await _user.UpdatePasswordAsync(uid, update?.OldPwd, update?.NewPwd))
                    return Ok(true);
                return BadRequest("修改失败.");
            }
            return BadRequest("用户ID异常.");
        }
    }
}