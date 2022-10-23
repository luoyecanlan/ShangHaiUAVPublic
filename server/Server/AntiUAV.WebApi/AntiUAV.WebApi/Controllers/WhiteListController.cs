using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Controllers
{

    /// <summary>
    /// 白名单管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Meatdata)]
    public class WhiteListController : ControllerBase
    {
        public WhiteListController(IWhiteListService white, ILogger<WhiteListController> logger)
        {
            _logger = logger;
            _white = white;
        }
        private readonly ILogger<WhiteListController> _logger;
        private readonly IWhiteListService _white;

        /// <summary>
        /// 获取全部白名单
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="size">页大小</param>
        /// <param name="key">检索关键字,按照设备名称模糊查询</param>
        /// <param name="desc">是否倒序（默认false）</param>
        /// <returns>白名单信息</returns>
        [HttpGet]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<PagingModel<WhiteListInfo>>), 200)]
        public async Task<IActionResult> Get([FromQuery] int index, [FromQuery] int size, [FromQuery] string key = "", [FromQuery] bool desc = false)
        {
            return Ok(await _white.GetAnyAsync(size, index, string.IsNullOrWhiteSpace(key) ? null :
                                        info => info.Sn.Contains(key),
                                        keySelector => keySelector.Id, desc));
        }

        /// <summary>
        /// 获取全部白名单
        /// </summary>
        /// <returns>预警区信息</returns>
        [HttpGet("all")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<WhiteListInfo[]>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _white.GetAnyAsync());
        }

        /// <summary>
        /// 获取单个白名单详细信息
        /// </summary>
        /// <param name="id">预警区ID</param>
        /// <returns>白名单信息</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<WhiteListInfo>), 200)]
        public async Task<IActionResult> GetInfo([FromRoute] int id)
        {
            return Ok(await _white.GetAsync(id));
        }
        /// <summary>
        /// 新增白名单
        /// </summary>
        /// <param name="add">白名单数据</param>
        /// <returns>白名单信息</returns>
        [HttpPost]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<WhiteListInfo>), 200)]
        public async Task<IActionResult> Add([FromBody] WhiteListAdd add)
        {
            var _add = await _white.AddAsync(add);
            if (_add != null)
            {
                _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}新增白名单 sn:{add.Sn}({add.StarTime.ToString("yyyy-MM-dd HH:mm:ss")}~{add.EndTime.ToString("yyyy-MM-dd HH:mm:ss")})。");
                //_ = _notice.noticeperzoneschange(_add.id, bussiness.noticemodels.perzonesnoticecode.add);
            }

            return Ok(_add);
        }

        /// <summary>
        /// 修改白名单信息
        /// </summary>
        /// <param name="update">白名单信息数据</param>
        /// <returns>白名单信息</returns>
        [HttpPut]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<WhiteListInfo>), 200)]
        public async Task<IActionResult> Update([FromBody] WhiteListUpdate update)
        {
            var _info = await _white.UpdateAsync(update);
            //if (_info != null)
            //    _ = _notice.NoticePerZonesChange(_info.Id, Bussiness.NoticeModels.PerZonesNoticeCode.Update);
            return Ok(_info);
        }

        /// <summary>
        /// 删除白名单
        /// </summary>
        /// <param name="id">白名单ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> Del([FromRoute] int id)
        {
            var count = await _white.DelAsync(id);
            if (count > 0)
                _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}删除白名单id：{id}。");
            //    _ = _notice.NoticePerZonesChange(id, Bussiness.NoticeModels.PerZonesNoticeCode.Delete);
            return Ok(count >= 0);
        }
    }
}
