using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AntiUAV.WebApi.Controllers
{
    /// <summary>
    /// 预警区域管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Meatdata)]
    public class ZoneController : ControllerBase
    {
        public ZoneController(IPreWarningZoneService zone, INoticeDeviceService notice, ILogger<ZoneController> logger)
        {
            _logger = logger;
            _zone = zone;
            _notice = notice;
        }
        private readonly ILogger<ZoneController> _logger;
        private readonly INoticeDeviceService _notice;
        private readonly IPreWarningZoneService _zone;
        /// <summary>
        /// 获取全部预警区
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="size">页大小</param>
        /// <param name="key">检索关键字,按照设备名称模糊查询</param>
        /// <param name="desc">是否倒序（默认false）</param>
        /// <returns>预警区信息</returns>
        [HttpGet]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<PagingModel<PerWarningZoneInfo>>), 200)]
        public async Task<IActionResult> Get([FromQuery] int index, [FromQuery] int size, [FromQuery] string key = "", [FromQuery] bool desc = false)
        {
            return Ok(await _zone.GetAnyAsync(size, index,
                                        info => info.Name.Contains(key) || info.Id.ToString().Contains(key),
                                        keySelector => keySelector.Id, desc));
        }

        /// <summary>
        /// 获取全部预警区
        /// </summary>
        /// <returns>预警区信息</returns>
        [HttpGet("all")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<PerWarningZoneInfo[]>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _zone.GetAnyAsync());
        }

        /// <summary>
        /// 获取单个告警区详细信息
        /// </summary>
        /// <param name="id">预警区ID</param>
        /// <returns>预警区信息</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<PerWarningZoneInfo>), 200)]
        public async Task<IActionResult> GetInfo([FromRoute] int id)
        {
            return Ok(await _zone.GetAsync(id));
        }
        /// <summary>
        /// 新增预警区
        /// </summary>
        /// <param name="add">预警区数据</param>
        /// <returns>预警区信息</returns>
        [HttpPost]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<PerWarningZoneInfo>), 200)]
        public async Task<IActionResult> Add([FromBody] PerWarningZoneAdd add)
        {
            var _add = await _zone.AddAsync(add);
            if (_add != null)
            {
                _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}新增预警区{_add.Name}({_add.Id})。");
                _ = _notice.NoticePerZonesChange(_add.Id, Bussiness.NoticeModels.PerZonesNoticeCode.Add);
            }

            return Ok(_add);
        }

        /// <summary>
        /// 修改预警区信息
        /// </summary>
        /// <param name="update">预警区信息数据</param>
        /// <returns>预警区信息</returns>
        [HttpPut]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<PerWarningZoneInfo>), 200)]
        public async Task<IActionResult> Update([FromBody] PerWarningZoneUpdate update)
        {
            var _info = await _zone.UpdateAsync(update);
            if (_info != null)
            {
                _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}修改预警区{_info.Name}({_info.Id})的信息数据。");
                _ = _notice.NoticePerZonesChange(_info.Id, Bussiness.NoticeModels.PerZonesNoticeCode.Update);
            }
            return Ok(_info);
        }

        /// <summary>
        /// 修改预警区信息
        /// </summary>
        /// <param name="update">预警区信息数据</param>
        /// <returns>预警区信息</returns>
        [HttpPut("geo")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<PerWarningZoneInfo>), 200)]
        public async Task<IActionResult> Update([FromBody] PerWarningZoneGeoUpdate update)
        {
            if (string.IsNullOrWhiteSpace(update.ZonePointsPosition))
                update.ZonePointsPosition = "[]";
            var _info = await _zone.UpdateGeoAsync(update);
            if (_info != null)
            {
                _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}修改预警区{_info.Name}({_info.Id})的绘制数据。");
                _ = _notice.NoticePerZonesChange(_info.Id, Bussiness.NoticeModels.PerZonesNoticeCode.Update);
            }
            return Ok(_info);
        }

        /// <summary>
        /// 删除预警区
        /// </summary>
        /// <param name="id">预警区ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> Del([FromRoute] int id)
        {
            var count = await _zone.DelAsync(id);
            if (count > 0)
            {
                _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}删除预警区id：{id}。");
                _ = _notice.NoticePerZonesChange(id, Bussiness.NoticeModels.PerZonesNoticeCode.Delete);
            }
            return Ok(count >= 0);
        }
    }
}