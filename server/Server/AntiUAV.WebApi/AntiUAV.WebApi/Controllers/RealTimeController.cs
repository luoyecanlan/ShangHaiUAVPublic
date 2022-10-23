using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using DbOrm.AntiUAV.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiUAV.WebApi.Controllers
{
    /// <summary>
    /// 实时数据管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.RealTime)]
    [Authorize(Roles = SystemRole.Client)]
    public class RealTimeController : ControllerBase
    {
        public RealTimeController(ITargetService target, IDeviceService device)
        {
            _target = target;
            _device = device;
        }

        private readonly ITargetService _target;
        private readonly IDeviceService _device;

        /// <summary>
        /// 获取全部目标信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tg")]
        [Authorize(Roles = SystemRole.Client)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<TargetPosition[]>), 200)]
        public async Task<IActionResult> GetSHTarget()
        {
            int devid = 3;
            int sec = 3;
            List<SHReportModel> Reports = new List<SHReportModel>();
            IEnumerable<TargetPosition> lists= await _target.GetLastUpdateTargetsPositionAsync(devid, DateTime.Now.AddSeconds(-1 * sec));
            foreach(TargetPosition temp in lists)
            {
                SHReportModel sHReportModel = new SHReportModel()
                {
                    Id = temp.Id,
                    DeviceId = temp.DeviceId,
                    Lat = temp.Lat,
                    Lng = temp.Lng,
                    Alt = temp.Alt,
                    FlyDirection = temp.FlyDirection,
                    AppAddr = temp.AppAddr,
                    AppLat = temp.AppLat,
                    AppLng = temp.AppLng,
                    HomeLat = temp.HomeLat,
                    HomeLng = temp.HomeLng,
                    UAVSn = temp.UAVSn,
                    UAVType = temp.UAVType,
                    BeginAt = temp.BeginAt,
                    TrackTime = temp.TrackTime,
                    V = temp.Vr,
                };
                Reports.Add(sHReportModel);
            }
            return Ok(Reports);
            
        }

        /// <summary>
        /// 获取全部目标信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tg/{devid}/{sec}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<TargetPosition[]>), 200)]
        public async Task<IActionResult> GetTarget([FromRoute]int devid,[FromRoute] int sec)
        {
            if (sec <= 5 && sec >= 1)
                return Ok(await _target.GetLastUpdateTargetsPositionAsync(devid, DateTime.Now.AddSeconds(-1 * sec)));
            else
            {
                throw new BussinessException(BussinessExceptionCode.ParamInvalidId, "参数无效,sec的有效值为1~5");
            }
        }

        /// <summary>
        /// 获取目标详细信息
        /// </summary>
        /// <param name="id">目标编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("tg/{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<TargetInfo>), 200)]
        public async Task<IActionResult> GetTargetInfo([FromRoute]string id)
        {
            //ID：P10102.7.SFml-18267
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id.Split(".")[1], out int devId))
                    return Ok(await _target.GetLastUpdateTargetInfo(id, devId));
            }
            throw new BussinessException(BussinessExceptionCode.ParamInvalidId,"参数无效");
        }

        /// <summary>
        /// 获取设备状态信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("dev")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<DeviceStatus[]>), 200)]
        public async Task<IActionResult> GetDevStatus()
        {
            var ids = await _device.GetAnyAsync<DeviceKey>();
            return Ok(await _device.GetStatus(ids.Select(x => x.Id).ToArray()));
        }

        /// <summary>
        /// 获取设备状态信息
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("dev/{id}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<DeviceStatus>), 200)]
        public async Task<IActionResult> GetDevStatus([FromRoute] int id)
        {
            return Ok(await _device.GetStatus(id));
        }
    }
}