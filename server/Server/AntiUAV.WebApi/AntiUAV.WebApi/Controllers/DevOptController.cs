using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.NoticeModels;
using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Config;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using DbOrm.AntiUAV.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static AntiUAV.Bussiness.GisTool;

namespace AntiUAV.WebApi.Controllers
{
    /// <summary>
    /// 设备操作相关
    /// </summary>
    [Route("api")]
    [ApiController]
    [ApiGroup(ApiGroupNames.DeviceOperation)]
    [Authorize(Roles = SystemRole.Client)]
    public class DevOptController : ControllerBase
    {
        private readonly IDeviceService _device;
        private readonly ILogger<DevOptController> _logger;
        private readonly INoticeDeviceService _notice;
        private readonly DeviceServiceConfig _config;
        private readonly IHistoryTrackService _history;

        public DevOptController(IDeviceService device, ILogger<DevOptController> logger, INoticeDeviceService notice, DeviceServiceConfig config, IHistoryTrackService track)
        {
            _notice = notice;
            _device = device;
            _logger = logger;
            _config = config;
            _history = track;
        }

        /// <summary>
        /// 开始跟踪
        /// </summary>
        /// <param name="tgid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("track/{tgid}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public IActionResult AddTrack([FromRoute] string tgid)
        {
            return Ok(true);
        }
        /// <summary>
        /// 取消跟踪
        /// </summary>
        /// <param name="tgid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("track/{tgid}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public IActionResult RemoveTrack([FromRoute] string tgid)
        {
            return Ok(true);
        }
        /// <summary>
        /// 新增监视
        /// </summary>
        /// <param name="tgid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("monitor/{tgid}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> AddMonitor([FromRoute] string tgid)
        {
            return await AddRelationShip(tgid, f => f.Category.IsMonitorDevice(), RelationshipsType.MonitorGd);
        }
        /// <summary>
        /// 删除监视
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("monitor/{rid}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> DeleteMonitor([FromRoute] string rid)
        {
            return await RemoveRelationShip(rid);
        }

        /// <summary>
        /// 开启打击
        /// </summary>
        /// <param name="tgid"></param>
        /// <param name="hitId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("hit/{tgid}")]
        //[Authorize(Roles = SystemRole.Client)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> AddHit([FromRoute] string tgid)
        {
            _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}开启打击。");
            return await AddRelationShip(tgid, f => f.Category.IsHitDevice(), RelationshipsType.AttackGd);
        }
        /// <summary>
        /// 取消打击
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="hitId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("hit/{rid}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> RemoveHit([FromRoute] string rid)
        {
            //RpcRequestModel rpcRequestModel = new RpcRequestModel()
            //{
            //    Data = null,
            //    ReqCode = RpcRequestEnum.AttackClose,
            //    TimeOut = 5000,
            //};
            //await _notice.NoticeRpcRequest(rpcRequestModel);
            _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}关闭打击。");
            return await RemoveRelationShip(rid);
        }
        // <summary>
        // 开启打击
        // /</summary>
        // <param name = "tgid" ></ param >
        // < param name="hitId"></param>
        // <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("hit/{tgid}/{hitId}/{hitFreq}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> AddHit([FromRoute] string tgid, [FromRoute] string hitId, [FromRoute] HitFreqModel hitFreq)
        {
            _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}开启打击。");
            return await AddHitRelationShip(tgid, f => f.Category.IsHitDevice(), RelationshipsType.AttackGd, hitId, hitFreq);
        }
        /// <summary>
        /// 取消打击
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="hitId"></param>
        /// <returns></returns>
        [HttpDelete]
        [AllowAnonymous]
        [Route("hit/{rid}/{hitId}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> RemoveHit([FromRoute] string rid, [FromRoute] string hitId)
        {
            //RpcRequestModel rpcRequestModel = new RpcRequestModel()
            //{
            //    Data = null,
            //    ReqCode = RpcRequestEnum.AttackClose,
            //    TimeOut = 5000,
            //};
            //await _notice.NoticeRpcRequest(rpcRequestModel);
            _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}关闭打击。");
            return await RemoveHItRelationShip(rid, hitId);
        }
        /// <summary>
        /// 开启诱骗
        /// </summary>
        /// <param name="tgid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("decoy/{tgid}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public IActionResult AddDecoy([FromRoute] string tgid)
        {
            return Ok(true);
        }
        /// <summary>
        /// 取消诱骗
        /// </summary>
        /// <param name="tgid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("decoy/{tgid}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public IActionResult RemoveDecoy([FromRoute] string tgid)
        {
            return Ok(true);
        }

        /// <summary>
        /// 设备开关
        /// </summary>
        /// <param name="id">设备Id</param>
        /// <param name="open">指定设备开关</param>
        /// <param name="param">参数</param>
        /// <returns>结果仅代表开关命令下发结果，不代表设备服务真实的状态</returns>      
        [HttpPost]
        [Route("sw")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> DevSwicth([FromQuery] int id, [FromQuery] bool open, [FromBody] string param)
        {
            try
            {
                var _devInfo = await _device.GetHost(id);
                if (open)
                {
                    _logger.LogInformation($"关闭设备{id}服务");
                }
                else
                {
                    _logger.LogInformation($"开启设备{id}服务");
                }
                return Ok(true);
            }
            catch (Exception es)
            {
                _logger.LogError($"通知异常：{es.Message}");
                return BadRequest(es.Message);
            }
        }

        /// <summary>
        /// 设备服务开关
        /// </summary>
        /// <param name="id">设备Id</param>
        /// <param name="open">指定设备开关</param>
        /// <returns>结果仅代表开关命令下发结果，不代表设备服务真实的状态</returns>
        [HttpPost]
        [Route("ssw")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> DevServerSwicth([FromQuery] int id, [FromQuery] bool open)
        {
            string ProcessName = _config.FileName;
            try
            {
                var devInfo = await _device.GetAsync(id);
                if (open)
                {
                    string _dsPath = @$"{_config.Path}/{ProcessName}{_config.Extension}";
                    if (!System.IO.File.Exists(_dsPath))
                        return BadRequest(new BussinessException(BussinessExceptionCode.NotFindDeviceServerFile));
                    ProcessStartInfo info = new ProcessStartInfo
                    {
                        FileName = _dsPath,
                        Arguments = $" {devInfo.Id} {devInfo.Category}"
                    };
                    Process pro = Process.Start(info);
                    pro.WaitForExit();
                }
                else
                {
                    var ps = Process.GetProcessesByName(ProcessName);
                    ps.ToList().ForEach(f =>
                    {
                        if (f.ProcessName.Equals(ProcessName))
                        {
                            f.Kill();
                            f.WaitForExit();
                        }
                    });
                }
                return Ok(true);
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }

        /// <summary>
        /// 新增转发
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("transmit")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> TransmitToDev([FromBody] TurnTargetInfo info)
        {
            try
            {
                var add = new Relationships()
                {
                    FromDeviceId = info.DeviceId,
                    TargetId = info.TargetId,
                    RType = RelationshipsType.PositionTurn,
                    ToAddressIp = info.Ip,
                    ToAddressPort = info.Port,
                    UpdateTime = DateTime.Now
                };
                var _addResult = await _device.AddRelationships(add);
                if (_addResult)
                {
                    var _fromHost = await _device.GetStatusOne(info.DeviceId);
                    _ = _notice.NoticeRelationAdd(add);
                    return Ok(true);
                }
                else
                    return BadRequest(new BussinessException(BussinessExceptionCode.RelationShipWriteError));
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }

        /// <summary>
        /// 纠偏设置
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Rectify")]
        [Authorize(Roles = SystemRole.Client)]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> SetRectifyToDev([FromBody] DeviceRectify info)
        {
            try
            {
                var add = new DeviceUpdatePosition()
                {
                    Lat = info.Lat,
                    Lng = info.Lng,
                    Alt = info.Alt,
                    RectifyAz = info.RectifyAz,
                    RectifyEl = info.RectifyEl
                };

                //var _addResult = await _device.Update(add);
                //if (_addResult!=null)
                {
                    RpcRequestModel rpcRequestModel = new RpcRequestModel()
                    {
                        Data = JsonConvert.SerializeObject(add),
                        ReqCode = RpcRequestEnum.RectifySetting,
                        TimeOut = 5000,
                    };
                    _ = _notice.NoticeRpcRequest(rpcRequestModel);
                    return Ok(true);
                }
                //else
                //    return BadRequest(new BussinessException(BussinessExceptionCode.RelationShipWriteError));
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }

        /// <summary>
        /// 云台控制（水平俯仰 指北归零 视场焦距）
        /// </summary>
        /// <param name="devicePTZInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PTZ")]
        [Authorize(Roles = SystemRole.Client)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> PTZToMonitor([FromBody] DevicePTZInfo devicePTZInfo)
        {
            try
            {
                if (devicePTZInfo != null)
                {
                    RpcRequestModel rpcRequestModel = new RpcRequestModel()
                    {
                        Data = JsonConvert.SerializeObject(devicePTZInfo),
                        ReqCode = RpcRequestEnum.PTZ,
                        TimeOut = 5000,
                    };
                    _ = _notice.NoticeRpcRequest(rpcRequestModel);
                    return Ok(true);
                }
                else
                    return BadRequest(new BussinessException(BussinessExceptionCode.RelationShipWriteError));
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }

        /// <summary>
        /// 手动跟踪启停
        /// </summary>
        /// <param name="deviceFollow"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Follow")]
        [AllowAnonymous]
        //[Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> FollowToMonitor([FromBody] DeviceFollow deviceFollow)
        {
            try
            {
                if (deviceFollow != null)
                {
                    RpcRequestModel rpcRequestModel = new RpcRequestModel()
                    {
                        Data = JsonConvert.SerializeObject(deviceFollow),
                        ReqCode = RpcRequestEnum.MonitorOpen,
                        TimeOut = 5000,
                    };
                    _ = _notice.NoticeRpcRequest(rpcRequestModel);
                    return Ok(true);
                }
                else
                    return BadRequest(new BussinessException(BussinessExceptionCode.RelationShipWriteError));
            }
            catch (Exception es)
            {
                return BadRequest(es.Message);
            }
        }

        /// <summary>
        /// 删除转发
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("transmit/{rid}")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public async Task<IActionResult> DeleteTransmit([FromRoute] string rid)
        {
            return await RemoveRelationShip(rid);
        }


        #region helper

        /// <summary>
        /// 建立关联关系
        /// </summary>
        /// <param name="tgid"></param>
        /// <param name="func"></param>
        /// <param name="_rtype"></param>
        /// <returns></returns>
        private async Task<IActionResult> AddRelationShip(string tgid, Func<DeviceInfo, bool> func, RelationshipsType _rtype)
        {
            _logger.LogInformation($"daji:1");
            var bastDev = await _device.GetBastDevice(tgid, f => func(f), _rtype);
            if (bastDev == null)
                return BadRequest(new BussinessException(BussinessExceptionCode.NoEquipmentAvailable));
            _logger.LogInformation($"daji:2");
            try
            {
                var _fromId = tgid.ToDeviceId();
                //写入关联关系
                var add = new Relationships()
                {
                    FromDeviceId = _fromId,
                    ToDeviceId = bastDev.Id,
                    ToAddressIp = bastDev.Lip,
                    ToAddressPort = bastDev.Lport,
                    TargetId = tgid,
                    RType = _rtype,
                    UpdateTime = DateTime.Now
                };
                _logger.LogInformation($"daji:{tgid}");
                var writeResult = await _device.AddRelationships(add);
                _logger.LogInformation($"daji:{writeResult}");
                if (writeResult)
                {
                    //增加目标打击开启记录
                    _ = _history.AddHitMark(tgid);
                    //通知设备服务Redis发生改变
                    _ = _notice.NoticeRelationAdd(add);
                    //if (add.RType == RelationshipsType.AttackGd)
                    //{
                    //    RpcRequestModel rpcRequestModel = new RpcRequestModel()
                    //    {
                    //        Data = JsonConvert.SerializeObject(add),
                    //        ReqCode= RpcRequestEnum.AttackOpen,
                    //        TimeOut=5000,
                    //    };
                    //    await _notice.NoticeRpcRequest(rpcRequestModel);
                    //}
                    return Ok(true);
                }
                else
                    return BadRequest(new BussinessException(BussinessExceptionCode.RelationShipWriteError));
            }
            catch (Exception es)
            {
                return BadRequest(es);
            }
        }

        /// <summary>
        /// 移除关联关系
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        private async Task<IActionResult> RemoveRelationShip(string rid)
        {
            try
            {
                //移除关联关系
                var removeResult = await _device.RemoveRelationships(rid);
                if (removeResult)
                {
                    //通知设备服务Redis发生改变
                    _ = _notice.NoticeRelationRemove(rid);

                    return Ok(true);
                }
                else
                    return BadRequest(new BussinessException(BussinessExceptionCode.RemoveShipWriteError));
            }
            catch (Exception es)
            {
                return BadRequest(es);
            }
        }


        #endregion


        #region Hithelper

        /// <summary>
        /// 建立关联关系
        /// </summary>
        /// <param name="tgid"></param>
        /// <param name="func"></param>
        /// <param name="_rtype"></param>
        /// <returns></returns>
        private async Task<IActionResult> AddHitRelationShip(string tgid, Func<DeviceInfo, bool> func, RelationshipsType _rtype, string Hitid, HitFreqModel hitFreq)
        {
            var bastDev = await _device.GetBastDevice(tgid, f => func(f), _rtype);
            if (bastDev == null)
                return BadRequest(new BussinessException(BussinessExceptionCode.NoEquipmentAvailable));
            try
            {
                var _fromId = tgid.ToDeviceId();
                //写入关联关系
                var add = new Relationships()
                {
                    FromDeviceId = _fromId,
                    hitFreq = hitFreq,
                    ToDeviceId = Convert.ToInt32(bastDev.Id),
                    ToAddressIp = bastDev.Lip,
                    ToAddressPort = bastDev.Lport,
                    TargetId = tgid,
                    RType = _rtype,
                    UpdateTime = DateTime.Now
                };
                var writeResult = await _device.AddRelationships(add);
                if (writeResult)
                {
                    //增加目标打击开启记录
                    _ = _history.AddHitMark(tgid);
                    //通知设备服务Redis发生改变
                    _ = _notice.NoticeRelationAdd(add);
                    //if (add.RType == RelationshipsType.AttackGd)
                    //{
                    //    RpcRequestModel rpcRequestModel = new RpcRequestModel()
                    //    {
                    //        Data = JsonConvert.SerializeObject(add),
                    //        ReqCode = RpcRequestEnum.AttackOpen,
                    //        TimeOut = 5000,
                    //    };
                    //    await _notice.NoticeRpcRequest(rpcRequestModel);
                    //}
                    return Ok(true);
                }
                else
                    return BadRequest(new BussinessException(BussinessExceptionCode.RelationShipWriteError));
            }
            catch (Exception es)
            {
                return BadRequest(es);
            }
        }

        /// <summary>
        /// 移除关联关系
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        private async Task<IActionResult> RemoveHItRelationShip(string rid, string hitId)
        {
            try
            {
                //移除关联关系
                var removeResult = await _device.RemoveRelationships(rid);
                if (removeResult)
                {
                    //通知设备服务Redis发生改变
                    _ = _notice.NoticeRelationRemove(rid);

                    return Ok(true);
                }
                else
                    return BadRequest(new BussinessException(BussinessExceptionCode.RemoveShipWriteError));
            }
            catch (Exception es)
            {
                return BadRequest(es);
            }
        }


        #endregion
    }
}