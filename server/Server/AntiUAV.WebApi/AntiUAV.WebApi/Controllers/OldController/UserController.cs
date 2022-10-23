using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using Bussiness.Metadata.Model;
using Bussiness.Metadata.Service;
using DbOrm.AntiUAV.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Controllers
{
    /// <summary>
    /// 当前用户操作
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.User)]
    [Authorize(Roles = SystemRole.All)]
    //[EnableCors("antiuav")]
    public class UserOldController : ControllerBase
    {
        public UserOldController(IUserManagerService user)
        {
            _user = user;
        }

        private readonly IUserManagerService _user;

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<UserInfoModel>), 200)]
        public async Task<IActionResult> Get()
        {
            if (int.TryParse(HttpContext.GetCurrentUserId(), out int uid))
            {
                return Ok(await _user.Get(uid));
            }
            return BadRequest("用户ID异常.");
        }

        /// <summary>
        /// 更新当前用户信息
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(ServiceResponse<UserInfoModel>), 200)]
        public async Task<IActionResult> UpdateInfo([FromBody]UserUpdate update)
        {
            if (int.TryParse(HttpContext.GetCurrentUserId(), out int uid))
            {
                if (update != null)
                {
                    update.Id = uid;
                    return Ok(await _user.Update(update));
                }
            }
            return BadRequest("用户ID异常.");
        }

        /// <summary>
        /// 修改当前用户密码
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> UpdatePwd([FromBody]UserPasswordUpdateModel update)
        {
            if (int.TryParse(HttpContext.GetCurrentUserId(), out int uid))
            {
                if (update != null)
                {
                    update.Uid = uid;
                }

                if (await _user.Update(update))
                    return Ok(true);
                return BadRequest("修改失败.");
            }
            return BadRequest("用户ID异常.");
        }
    }
}