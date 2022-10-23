using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using DbOrm.AntiUAV.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AntiUAV.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Config)]
    [Authorize(Roles = SystemRole.Client)]
    public class ConfigController : ControllerBase
    {
        IMapConfigService _mapService;
        IPersonalizationService _personalizationService;
        ITargetTurnService _targetTurnService;
        ISystemConfigService _systemConfigService;
        public ConfigController(IMapConfigService map,IPersonalizationService personalization,ITargetTurnService targetTurn,ISystemConfigService systemConfig)
        {
            _mapService = map;
            _personalizationService = personalization;
            _targetTurnService = targetTurn;
            _systemConfigService = systemConfig;
        }

        /// <summary>
        /// 获取全部地图配置信息
        /// </summary>
        /// <returns>设备信息</returns>
        [HttpGet("map")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<MapConfigInfo[]>), 200)]
        public async Task<IActionResult> GetMapConfigAll()
        {
            return Ok(await _mapService.GetAnyAsync());
        }

        /// <summary>
        /// 获取单条地图配置详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("map/{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<MapConfigInfo>), 200)]
        public async Task<IActionResult> GetMapConfig([FromRoute]int id)
        {
            return Ok(await _mapService.GetAsync(id));
        }

        /// <summary>
        /// 新增地图配置信息
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpPut("map")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<MapConfigInfo>), 200)]
        public async Task<IActionResult> AddMapConfig([FromBody]MapConfigAdd mode)
        {
            return Ok(await _mapService.AddAsync(mode));
        }

        /// <summary>
        /// 修改地图配置信息
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpPost("map")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<MapConfigInfo>), 200)]
        public async Task<IActionResult> UpdateMapConfig([FromBody]MapConfigUpdate mode)
        {
            return Ok(await _mapService.UpdateAsync(mode));
        }

        /// <summary>
        /// 删除地图配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("map/{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> DeleteMapConfig([FromRoute]int id)
        {
            var count = await _mapService.DelAsync(id);
            return Ok(count > 0);
        }

        /// <summary>
        /// 获取全部转发配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("tgturn")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<TargetTurnInfo[]>), 200)]
        public async Task<IActionResult> GetTargetTurnAll()
        {
            return Ok(await _targetTurnService.GetAnyAsync());
        }

        /// <summary>
        /// 获取单条转发配置详细信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("tgturn/{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<TargetTurnInfo>), 200)]
        public async Task<IActionResult> GetTargetTurn([FromRoute]int id)
        {
            return Ok(await _targetTurnService.GetAsync(id));
        }

        /// <summary>
        /// 新增转发配置
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpPut("tgturn")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<TargetTurnInfo>), 200)]
        public async Task<IActionResult> AddTargetTurn([FromBody]TargetTurnAdd mode)
        {
            return Ok(await _targetTurnService.AddAsync(mode));
        }

        /// <summary>
        /// 修改转发配置
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpPost("tgturn")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<TargetTurnInfo>), 200)]
        public async Task<IActionResult> UpdateTargetTurn([FromBody]TargetTurnUpdate mode)
        {
            return Ok(await _targetTurnService.UpdateAsync(mode));
        }

        /// <summary>
        /// 删除转发配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("tgturn/{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> DeleteTargetTurn([FromRoute]int id)
        {
            var count = await _targetTurnService.DelAsync(id);
            return Ok(count > 0);
        }

        /// <summary>
        /// 获取个性化配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("person")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<PersonalizationInfo[]>), 200)]
        public async Task<IActionResult> GetPersonalizationAll()
        {
            return Ok(await _personalizationService.GetAnyAsync());
        }

        /// <summary>
        /// 获取单条个性化配置详情
        /// </summary>
        /// <returns></returns>
        [HttpGet("person/{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<PersonalizationInfo>), 200)]
        public async Task<IActionResult> GetPersonalization([FromRoute]int id)
        {
            return Ok(await _personalizationService.GetAsync(id));
        }

        /// <summary>
        /// 新增个性化配置
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpPut("person")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<PersonalizationInfo>), 200)]
        public async Task<IActionResult> AddPersonalization([FromBody]PersonalizationAdd mode)
        {
            return Ok(await _personalizationService.AddAsync(mode));
        }

        /// <summary>
        /// 修改个性化配置
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpPost("person")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<PersonalizationInfo>), 200)]
        public async Task<IActionResult> UpdatePersonalization([FromBody]PersonalizationUpdate mode)
        {
            return Ok(await _personalizationService.UpdateAsync(mode));
        }

        /// <summary>
        /// 删除个性化配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("person/{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> DeletePersonalization([FromRoute]int id)
        {
            var count= await _personalizationService.DelAsync(id);
            return Ok(count > 0);
        }

        /// <summary>
        /// 获取系统配置键值对列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("sys")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<SysConfig[]>), 200)]
        public async Task<IActionResult> GetSysConfigAll()
        {
            return Ok(await _systemConfigService.GetAnyAsync());
        }

        /// <summary>
        /// 获取系统配置键值对详情
        /// </summary>
        /// <returns></returns>
        [HttpGet("sys/{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<SysConfig>), 200)]
        public async Task<IActionResult> GetConfig([FromRoute]int id)
        {
            return Ok(await _systemConfigService.GetAsync(id));
        }

        /// <summary>
        /// 根据名称获取系统配置键值对详情
        /// </summary>
        /// <returns></returns>
        [HttpGet("sysconf/{key}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<SysConfig>), 200)]
        public async Task<IActionResult> GetConfig([FromRoute]string key)
        {
            return Ok(await _systemConfigService.GetConfigAsync(key));
        }

        /// <summary>
        /// 新增系统配置信息
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpPut("sys")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<SysConfig>), 200)]
        public async Task<IActionResult> AddSysConfig([FromBody]SysConfigAdd mode)
        {
            return Ok(await _systemConfigService.AddAsync(mode));
        }

        /// <summary>
        /// 修改系统配置信息
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpPost("sys")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<SysConfig>), 200)]
        public async Task<IActionResult> UpdateSysConfig([FromBody]SysConfig mode)
        {
            return Ok(await _systemConfigService.UpdateAsync(mode));
        }

        /// <summary>
        /// 删除系统配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("sys/{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> DeleteSysConfig([FromRoute]int id)
        {
            var count = await _systemConfigService.DelAsync(id);
            return Ok(count > 0);
        }
    }
}