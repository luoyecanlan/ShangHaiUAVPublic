using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AntiUAV.Bussiness.Service;
using DbOrm.AntiUAV.Entity;
using Microsoft.Extensions.Logging;
using DbOrm.CRUD;

namespace AntiUAV.WebApi.Controllers
{
    /// <summary>
    /// 设备管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Meatdata)]
    [Authorize(Roles = SystemRole.Client)]
    public class DevController : ControllerBase
    {
        public IDeviceService _devService;
        private readonly ILogger<DevController> _logger;
        private readonly INoticeDeviceService _notice;
        public DevController(IDeviceService service, ILogger<DevController> logger, INoticeDeviceService notice)
        {
            _devService = service;
            _logger = logger;
            _notice = notice;
        }
        /// <summary>
        /// 获取全部设备信息
        /// </summary>
        /// <returns>设备信息</returns>
        [HttpGet("all")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<DeviceInfo[]>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _devService.GetAnyAsync());
        }

        /// <summary>
        /// 获取设备基础信息  位置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("devs")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<DeviceBaseInfo[]>), 200)]
        public async Task<IActionResult> GetAllBaseInfo()
        {
            return Ok(await _devService.GetAnyAsync<DeviceBaseInfo>());
        }

        /// <summary>
        /// 获取全部设备信息(按ID排序)
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="size">页大小</param>
        /// <param name="key">检索关键字,按照设备名称模糊查询</param>
        /// <param name="desc">是否倒叙</param>
        /// <returns>设备信息集合</returns>
        /// <remarks>
        ///  关键字为 名称或者ID
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(PagingModel<DeviceInfo[]>), 200)]
        public async Task<IActionResult> Get([FromQuery] int index, [FromQuery] int size, [FromQuery] string key, [FromQuery] bool desc = false)
        {
            if (string.IsNullOrEmpty(key))
                return Ok(await _devService.GetAnyAsync(size, index, null, selector => selector.Id, desc));
            else
                return Ok(await _devService.GetAnyAsync(size, index,
                    info => info.Name.Contains(key) || info.Id.ToString().Contains(key),
                    selector => selector.Id, desc));

        }

        /// <summary>
        /// 获取单个设备详细信息
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<DeviceInfo>), 200)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            return Ok(await _devService.GetAsync(id));
        }

        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="add">新增设备数据</param>
        /// <returns>设备信息</returns>
        [HttpPost]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<DeviceInfo>), 200)]
        public async Task<IActionResult> Add([FromBody] DeviceAdd add)
        {
            var n = await _devService.AddAsync(add);
            if (n != null)
                _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}新增设备{n.Name}({n.Id})。");
            return Ok(n);
        }

        /// <summary>
        /// 修改设备信息
        /// </summary>
        /// <param name="update">修改设备信息数据</param>
        /// <returns>设备信息</returns>
        [HttpPut]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<DeviceInfo>), 200)]
        public async Task<IActionResult> Update([FromBody] DeviceUpdate update)
        {
            var info = await _devService.UpdateAsync(update);
            if (info != null)
            {
                _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}修改设备{info.Name}({info.Id})信息数据。");
                try
                {
                    _ = _notice.NoticeDeviceChange(info.Id, Bussiness.NoticeModels.DeviceInfoNoticeCode.InfoChange);
                }
                catch (Exception es)
                {
                    _logger.LogError($"通知异常：{es.Message}");
                }
            }
            return Ok(info);
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="id">删除的设备ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> Del([FromRoute] int id)
        {
            var count = await _devService.DelAsync(id);
            if (count >= 0)
                _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}删除设备{id}。");
            return Ok(count > 0);
        }
        /// <summary>
        /// 获取设备类型列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("ct")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<DeviceCategoryInfo[]>), 200)]
        public async Task<IActionResult> GetCategory()
        {
            return Ok(await _devService.GetCategory());
        }
    }
}