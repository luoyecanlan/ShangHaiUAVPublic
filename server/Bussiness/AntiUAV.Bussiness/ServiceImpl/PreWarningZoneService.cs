using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AntiUAV.Bussiness.GisTool;

namespace AntiUAV.Bussiness.ServiceImpl
{
    /// <summary>
    /// 预警区服务业务
    /// </summary>
    public class PreWarningZoneService : MetadataService<PerWarningZoneInfo, PerWarningZoneUpdate, PerWarningZoneDel, PerWarningZoneAdd>, IPreWarningZoneService
    {
        public PreWarningZoneService(IEntityCrudService orm, GisTool tool) : base(orm)
        {
            _tool = tool;
        }

        private readonly GisTool _tool;

        /// <summary>
        /// 更新预警区几何图形
        /// </summary>
        /// <param name="zone">预警区几何图形</param>
        /// <returns></returns>
        public async Task<PerWarningZoneInfo> UpdateGeoAsync(PerWarningZoneGeoUpdate zone)
        {
            if (zone == null)
                throw new BussinessException(BussinessExceptionCode.ParamNull);
            bool res;
            try
            {
                var cacl = Cacl(zone);
                if (cacl == null)
                    throw new BussinessException(BussinessExceptionCode.CaclDataNull);
                res = await _orm.UpdateAsync(new PerWarningZoneGeoUpdate()
                {
                    Id = zone.Id,
                    ZonePoints = zone.ZonePoints,
                    ZonePointsPosition = zone.ZonePointsPosition,
                    ADistance = cacl.ADistance,
                    BDistance = cacl.BDistance,
                    CircumcircleLat = cacl.CircumcircleLat,
                    CircumcircleLng = cacl.CircumcircleLng,
                    CircumcircleR = cacl.CircumcircleR,
                    CircumcircleRadiationR = cacl.CircumcircleRadiationR,
                    NormalDistance = cacl.NormalDistance
                });
            }
            catch (Exception ex)
            {
                throw new BussinessException(BussinessExceptionCode.OptUpdateFail, ex, message: $"Id:{zone.Id}");
            }
            return res ? await GetAsync(zone.Id) : default;
        }

        private CaclZone Cacl<T>(T zone) where T : PerWarningZoneGeoUpdate
        {
            var model = zone.ZonePoints.ToObject<WarningZone>();//从Zonepoints中提取json
            if (model.type == "Circle")
                return CalcCircle(zone.ZonePoints);
            else if (model.type == "Polygon")
                return CalcPolygon(zone.ZonePoints);
            else
                return default;
        }

        /// <summary>
        /// 计算多边形告警区
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
        private CaclZone CalcPolygon(string jsonObj)
        {
            //需要计算zone到cacl的值
            var area = new List<PositionSimple>();
            var polygonModel = jsonObj.ToObject<PolygonWarningZone>();
            if (polygonModel != null && polygonModel.coordinates?.Count() > 0)
            {
                polygonModel.coordinates.First()?.ToList().ForEach(p =>
                {
                    area.Add(_tool.PointToLatLng(p));
                });
                var extent = _tool.GetExtent(area);
                var centerP = _tool.GetCenterPoint(extent);
                var R = _tool.GetRaduis(extent);
                var RR = R * 0.3;//30%辐射区域
                if (RR > 1500)
                    RR = 1500;
                var RadiationR = R + RR;

                return new CaclZone()
                {
                    CircumcircleR = R.Format(1),
                    CircumcircleRadiationR = RadiationR.Format(1),
                    CircumcircleLat = centerP.Lat.Format(7),
                    CircumcircleLng = centerP.Lng.Format(7),
                    ADistance = RadiationR.Format(1),
                    BDistance = (RadiationR / R).Format(),
                    NormalDistance = R.Format(1)
                };
            }
            return default;
        }

        /// <summary>
        /// 计算圆形告警区
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
        private CaclZone CalcCircle(string jsonObj)
        {
            var circleModel = jsonObj.ToObject<CircleWarningZone>();
            if (circleModel != null)
            {
                var centerP = _tool.PointToLatLng(circleModel.center);
                var R = circleModel.radius;
                var RR = R * 0.3;//30%辐射区域
                if (RR > 1500)
                    RR = 1500;
                var RadiationR = R + RR;

                return new CaclZone()
                {
                    CircumcircleR = R.Format(1),
                    CircumcircleRadiationR = RadiationR.Format(1),
                    CircumcircleLat = centerP.Lat.Format(7),
                    CircumcircleLng = centerP.Lng.Format(7),
                    ADistance = RadiationR.Format(1),
                    BDistance = (RadiationR / R).Format(),
                    NormalDistance = R.Format(1)
                };
            }
            return default;
        }

    }
}
