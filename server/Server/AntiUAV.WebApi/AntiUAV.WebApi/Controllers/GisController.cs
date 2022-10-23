using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiUAV.Bussiness;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static AntiUAV.Bussiness.GisTool;

namespace AntiUAV.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiGroup(ApiGroupNames.Gis)]
    public class GisController : ControllerBase
    {
        ILogger<GisController> _logger;
        GisTool _gisTool;
        public GisController(ILogger<GisController> logger,GisTool gis)
        {
            _logger = logger;
            _gisTool = gis;
        }

        /// <summary>
        /// 根据两点的经纬高计算目标对应设备的方位、俯仰、距离、高度
        /// </summary>
        /// <param name="gis"></param>
        /// <returns></returns>
        [HttpPost("3d2info")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<RelativeSpatialPosition>), 200)]
        public IActionResult Convert3DPositionAzimuthAndPitchInfo([FromBody]GisToolModel gis)
        {
            var ret = _gisTool.Convert3DPositionAzimuthAndPitchInfo(gis.Target, gis.Center);
            return Ok(ret);
        }

        /// <summary>
        /// 已知一个点经纬度，方位角，距离，计算另一个点经纬度   (二维坐标)
        /// </summary>
        /// <param name="mode">仅需要传递中心点经纬度及目标的距离和方位即可</param>
        /// <returns></returns>
        [HttpPost("2latlng")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<Position>), 200)]
        public IActionResult ConvertLonLat([FromBody]PositionModel mode)
        {
            var ret = _gisTool.ConvertLonLat(mode.Center, mode.AZ, mode.Dis);
            return Ok(ret);
        }

        /// <summary>
        /// 根据俯仰和距离计算真是高度
        /// </summary>
        /// <param name="pitch">俯仰</param>
        /// <param name="dis">距离</param>
        /// <returns></returns>
        [HttpPost("2height")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<double>), 200)]
        public IActionResult ConvertRealHeight([FromQuery]double pitch,[FromQuery]double dis)
        {
            var ret = _gisTool.ConvertRealHeight(pitch, dis);
            return Ok(ret);
        }


        /// <summary>
        /// 根据设备点的经纬度和相对目标的方位、距离、俯仰，计算目标点经纬高
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [HttpPost("tgpos")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<Position>), 200)]
        public IActionResult GetTargetPosition([FromBody]PositionModel position)
        {
            var ret = _gisTool.GetTargetPosition(position.Center, position.AZ, position.Dis, position.Pitch, position.Alt);
            return Ok(ret);
        }

        /// <summary>
        /// 度转度分秒
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        [HttpPost("totuple")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<Tuple<double, double, double>>), 200)]
        public IActionResult ToTuple([FromQuery]double deg)
        {
            return Ok(_gisTool.ConverToDegAndMinAndSed(deg));
        }


        /// <summary>
        /// 度分秒转度
        /// </summary>
        /// <param name="deg">度</param>
        /// <param name="min">分</param>
        /// <param name="sed">秒</param>
        /// <returns></returns>
        [HttpPost("todeg")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<double>), 200)]
        public IActionResult ToDegrade([FromQuery]double deg,[FromQuery]double min,[FromQuery]double sed)
        {
            return Ok(_gisTool.ConverToDegrade(deg, min, sed));
        }

        /// <summary>
        /// 两点校北
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("correct/point")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<CorrectModel>), 200)]
        public IActionResult CorrectByPoints([FromBody]PointsCorrectNorthModel model)
        {
            return Ok(_gisTool.CalcCorrectByPoints(model));
        }


        /// <summary>
        /// 相对校北
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("correct/relative")]
        [Authorize(Roles = SystemRole.Client)]
        [ProducesResponseType(typeof(ServiceResponse<RelativeSpatialPosition>), 200)]
        public IActionResult CorrectByRelative([FromBody]RelativeCorrectNorthModel model)
        {
            return Ok(_gisTool.CalcCorrectByRelative(model));
        }

    }
}