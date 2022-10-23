using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AntiUAV.Bussiness
{
    public class GisTool
    {

        /// <summary>
        /// 坐标点是否在多边形内判断
        /// </summary>
        /// <param name="point"></param>
        /// <param name="pts"></param>
        /// <returns></returns>
        public static bool isPointInPolygon(Position point, List<Position> pts)
        {

            //检查类型
            if (point == null || pts?.Count <= 0)
                return false;

            var N = pts.Count;
            var boundOrVertex = true; //如果点位于多边形的顶点或边上，也算做点在多边形内，直接返回true
            var intersectCount = 0; //cross points count of x 
            var precision = 2e-10; //浮点类型计算时候与0比较时候的容差
            Position p1, p2; //neighbour bound vertices
            var p = point; //测试点
            p1 = pts[0]; //left vertex        
            for (var i = 1; i <= N; ++i)
            {
                //check all rays            
                if (p.Lat.Equals(p1.Lat) && p.Lng.Equals(p1.Lng))
                {
                    return boundOrVertex; //p is an vertex
                }

                p2 = pts[i % N]; //right vertex            
                if (p.Lat < Math.Min(p1.Lat, p2.Lat) || p.Lat > Math.Max(p1.Lat, p2.Lat))
                {
                    //ray is outside of our interests                
                    p1 = p2;
                    continue; //next ray left point
                }

                if (p.Lat > Math.Min(p1.Lat, p2.Lat) && p.Lat < Math.Max(p1.Lat, p2.Lat))
                {
                    //ray is crossing over by the algorithm (common part of)
                    if (p.Lng <= Math.Max(p1.Lng, p2.Lng))
                    {
                        //x is before of ray                    
                        if (p1.Lat == p2.Lat && p.Lng >= Math.Min(p1.Lng, p2.Lng))
                        {
                            //overlies on a horizontal ray
                            return boundOrVertex;
                        }

                        if (p1.Lng == p2.Lng)
                        {
                            //ray is vertical                        
                            if (p1.Lng == p.Lng)
                            {
                                //overlies on a vertical ray
                                return boundOrVertex;
                            }
                            else
                            {
                                //before ray
                                ++intersectCount;
                            }
                        }
                        else
                        {
                            //cross point on the left side                        
                            var xinters =
                                (p.Lat - p1.Lat) * (p2.Lng - p1.Lng) / (p2.Lat - p1.Lat) +
                                p1.Lng; //cross point of lng                        
                            if (Math.Abs(p.Lng - xinters) < precision)
                            {
                                //overlies on a ray
                                return boundOrVertex;
                            }

                            if (p.Lng < xinters)
                            {
                                //before ray
                                ++intersectCount;
                            }
                        }
                    }
                }
                else
                {
                    //special case when ray is crossing through the vertex                
                    if (p.Lat == p2.Lat && p.Lng <= p2.Lng)
                    {
                        //p crossing over p2                    
                        var p3 = pts[(i + 1) % N]; //next vertex                    
                        if (p.Lat >= Math.Min(p1.Lat, p3.Lat) && p.Lat <= Math.Max(p1.Lat, p3.Lat))
                        {
                            //p.lat lies between p1.lat & p3.lat
                            ++intersectCount;
                        }
                        else
                        {
                            intersectCount += 2;
                        }
                    }
                }
                p1 = p2; //next ray left point
            }

            if (intersectCount % 2 == 0)
            {
                //偶数在多边形外
                return false;
            }
            else
            {
                //奇数在多边形内
                return true;
            }

        }


        #region 0~360度与-180~180度相互换转
        /// <summary>
        /// 0~360度与-180~180度相互换转
        /// </summary>
        /// <param name="angle">转换角度</param>
        /// <returns>如果参数大于360或者小于-180，则会返回错误代码1001</returns>
        public double MutualConvert180And360(double angle)
        {
            if (angle > 360 || angle < -180) return 1001;

            if (angle > 180)
                return Convert360With180(angle);
            else
                return Convert180With360(angle);
        }
        #endregion

        #region 度转度、分、秒
        /// <summary>
        /// 度转度、分、秒
        /// </summary>
        /// <param name="Angle">角度</param>
        /// <returns>度；分；秒</returns>
        public Tuple<double, double, double> ConverToDegAndMinAndSed(double Angle)
        {
            var tmp = ConverToDegAndMin(Angle);
            var sed = Math.Round((tmp.Item2 - Math.Truncate(tmp.Item2)) * 60, 2);
            return new Tuple<double, double, double>(tmp.Item1, tmp.Item2, sed);
        }
        #endregion

        #region 度、分、秒转度
        /// <summary>
        /// 度、分、秒转度
        /// </summary>
        /// <param name="deg">度</param>
        /// <param name="min">分</param>
        /// <param name="sed">秒</param>
        /// <returns>度</returns>
        public double ConverToDegrade(double deg, double min, double sed)
        {
            //return Math.Round(ConverToDegrade(deg , min) + sed/3600 , 7);
            return Math.Round(deg + min / 60 + sed / 3600, 7);
        }
        #endregion

        #region 根据设备点的经纬度和相对目标的方位、距离、俯仰，计算目标点经纬高
        /// <summary>
        /// 根据设备经纬度和目标的方位、距离、俯仰，计算目标点经纬高
        /// </summary>
        /// <param name="centerPoint">设备经纬度</param>
        /// <param name="az">方位角（度）</param>
        /// <param name="dis">距离（米）</param>
        /// <param name="pitch">俯仰（度）</param>
        /// <param name="height">高度（米）</param>
        /// <returns>目标点经纬高（高程）</returns>
        public Position GetTargetPosition(Position centerPoint, double az, double dis, double pitch = 0, double height = 0)
        {
            var postion = ConvertLonLat(centerPoint, az, dis);
            if (pitch != 0)
                postion.Altitude = centerPoint.Altitude + ConvertRealHeight(pitch, dis);
            else
                //高度和距离转成俯仰角都，在计算目标的高度
                postion.Altitude = centerPoint.Altitude + ConvertRealHeight(ConvertPich(height, dis), dis);
            return postion;
        }
        #endregion

        #region 根据两点的经纬高（高程）计算目标对应设备的方位、俯仰、距离、高度
        /// <summary>
        /// 根据两点的经纬高（高程）计算目标对应设备的方位、俯仰、距离、高度差值
        /// </summary>
        /// /// <param name="TargetPoint">目标点3d坐标</param>
        /// <param name="CenterPoint">中心点3d坐标</param>
        /// <returns>方位（度）、俯仰（度）、距离（米）、高度差（米）</returns>
        public RelativeSpatialPosition Convert3DPositionAzimuthAndPitchInfo(Position TargetPoint, Position CenterPoint)
        {
            var AZEL = ConvertAzimutInfo(CenterPoint, TargetPoint);
            if (AZEL == null) return null;
            AZEL.AD = TargetPoint.Altitude - CenterPoint.Altitude;
            AZEL.El = ConvertPich(AZEL.AD, AZEL.Dis);
            return AZEL;
        }
        #endregion

        #region 计算目标航速（绝对速度+航向）考虑二维
        /// <summary>
        ///  计算目标航速（绝对速度+航向）考虑二维
        /// </summary>
        /// <param name="CenterPoint">中心点经纬度、时刻</param>
        /// <param name="TargetPoint">目标点点经纬度、时刻</param>
        /// <returns>速度（米/秒）、航向（度）</returns>
        public VelocityVector GetTargetAbsoluteSpeed(Position CenterPoint, Position TargetPoint, double s_time)
        {
            var AzDis = ConvertAzimutInfo(CenterPoint, TargetPoint);
            //var time = (CenterPoint.UpdateTime - TargetPoint.UpdateTime).TotalSeconds;
            var speed = (AzDis?.Dis ?? 0) / s_time;
            return new VelocityVector() { Speed = speed, Course = AzDis?.Az ?? 0 };
        }
        #endregion

        #region 判断目标点是否在设备覆盖范围之内
        /// <summary>
        /// 判断目标点是否在设备覆盖范围之内(注：以centerPoint为圆心，startAngle为起始方位，顺时针方向，endAngle为结束方位，这个范围内进行判断)
        /// </summary>
        /// <param name="centerPoint">设备中心点经纬度</param>
        /// <param name="startAngle">覆盖范围起始角度</param>
        /// <param name="endAngle">覆盖范围结束角度</param>
        /// <param name="radius">覆盖半径，单位米</param>
        /// <param name="TargetPoint">需要判断的目标点经纬度</param>
        /// <returns>存在范围内返回距离，反之返回-1</returns>
        public double JudgeCoverTarget(Position centerPoint, double startAngle, double endAngle, double radius, Position TargetPoint)
        {
            var tmp = ConvertAzimutInfo(centerPoint, TargetPoint);
            //如果距离大于覆盖半径，返回false
            if (tmp.Dis > radius)
                return -1;
            //如果是个圆则返回true
            if (startAngle == 0 && endAngle == 360)
                return tmp.Dis;

            //将起始角度强制归到0位，然后将目标方位也减去等量的差值在做判断
            var differenceValue = startAngle;
            var newEndAngle = (endAngle - differenceValue) > 0 ? (endAngle - differenceValue) : 360 + (endAngle - differenceValue);
            var newAz = (tmp.Az - differenceValue) > 0 ? (tmp.Az - differenceValue) : 360 + (tmp.Az - differenceValue);
            if (newAz >= 0 && newAz <= newEndAngle)
                return tmp.Dis;
            else
                return -1;
        }
        #endregion

        /// <summary>
        /// 计算两点间距离
        /// </summary>
        /// <param name="CenterPoint"></param>
        /// <param name="TargetPoint"></param>
        /// <returns></returns>
        public double CalculatingDistance(Position CenterPoint, Position TargetPoint)
        {
            return ConvertAzimutInfo(CenterPoint, TargetPoint).Dis;
        }

        public PoylgonExtant GetExtent(IEnumerable<PositionSimple> poylgon)
        {
            return new PoylgonExtant()
            {
                MaxLat = poylgon.Max(x => x.Lat),
                MinLat = poylgon.Min(x => x.Lat),
                MaxLng = poylgon.Max(x => x.Lng),
                MinLng = poylgon.Min(x => x.Lng)
            };
        }

        public Position GetCenterPoint(PoylgonExtant extant)
        {
            var point = new Position { Lat = extant.MaxLat, Lng = extant.MinLng };
            var br = Convert3DPositionAzimuthAndPitchInfo(new Position { Lat = extant.MinLat, Lng = extant.MaxLng }, point);
            return ConvertLonLat(point, br.Az, br.Dis / 2);
        }
        public double GetRaduis(PoylgonExtant extant)
        {
            return Convert3DPositionAzimuthAndPitchInfo(new Position { Lat = extant.MinLat, Lng = extant.MaxLng }, new Position { Lat = extant.MaxLat, Lng = extant.MinLng }).Dis;
        }

        #region 固定参数
        //大地坐标系资料WGS-84 长半径a=6378137 短半径b=6356752.3142 扁率f=1/298.2572236 
        private const double a = 6378137;
        private const double b = 6356752.3142;
        private const double f = 1 / 298.2572236;
        #endregion

        #region models
        public class PoylgonExtant
        {
            public double MinLat { set; get; }
            public double MaxLat { set; get; }
            public double MinLng { get; set; }
            public double MaxLng { get; set; }
        }
        /// <summary>
        /// 简单位置信息
        /// </summary>
        public class PositionSimple
        {
            /// <summary>
            /// 纬度
            /// </summary>
            public double Lat { get; set; }
            /// <summary>
            /// 经度
            /// </summary>
            public double Lng { get; set; }
        }

        /// <summary>
        /// 经度、纬度信息
        /// </summary>
        public class Position
        {
            /// <summary>
            /// 纬度
            /// </summary>
            public double Lat { get; set; }
            /// <summary>
            /// 经度
            /// </summary>
            public double Lng { get; set; }
            /// <summary>
            /// 海拔
            /// </summary>
            public double Altitude { get; set; }
            /// <summary>
            /// 更新时间
            /// </summary>
            public DateTime UpdateTime { get; set; }

        }

        /// <summary>
        /// 方位、俯仰、距离信息
        /// </summary>
        public class RelativeSpatialPosition
        {
            /// <summary>
            /// 方位角{度}
            /// </summary>
            public double Az { get; set; }
            /// <summary>
            /// 俯仰角（度）
            /// </summary>
            public double El { get; set; }
            /// <summary>
            /// 距离（米）
            /// </summary>
            public double Dis { get; set; }
            /// <summary>
            /// 高度差
            /// </summary>
            public double AD { get; set; }
        }

        public class VelocityVector
        {
            /// <summary>
            /// 速度（米/秒）
            /// </summary>
            public double Speed { get; set; }

            /// <summary>
            /// 方位（度）
            /// </summary>
            public double Course { get; set; }
        }


        /// <summary>
        /// 纠偏信息
        /// </summary>
        public class CorrectModel
        {
            public double Az { get; set; }
            public double El { get; set; }
        }


        /// <summary>
        /// 两点校北数据
        /// </summary>
        public class PointsCorrectNorthModel : GisToolModel
        {
            public double CurrentAz { get; set; }
            public double CurrentEl { get; set; }
        }

        /// <summary>
        /// 相对校北数据
        /// </summary>
        public class RelativeCorrectNorthModel : GisToolModel
        {
            public double Dis { get; set; }
            public double Az { get; set; }
        }
        #endregion

        #region 0~360度换转-180~180度
        /// <summary>
        /// 0~360度换转-180~180度
        /// </summary>
        /// <param name="angle">转换角度</param>
        /// <returns>-180~180</returns>
        public double Convert360With180(double angle)
        {
            //判断参数是否无效
            if (angle > 360 || angle < 0) return default;

            //小于等于180返回原值
            if (angle <= 180)
                return angle;
            //大于180返回负值
            else
                return 0 - (360 - angle);
        }
        #endregion

        #region -180~180度换转0~360度
        /// <summary>
        /// -180~180度换转0~360度
        /// </summary>
        /// <param name="angle">转换角度</param>
        /// <returns>0~360</returns>
        public double Convert180With360(double angle)
        {

            //判断参数是否无效
            if (angle > 180 || angle < -180) return default;
            //大于零返回原值
            if (angle >= 0)
                return angle;
            //返回一个大于180的正值
            else
                return Math.Abs(0 - 360 - angle);
        }
        #endregion

        #region 度转度、分
        /// <summary>
        /// 度转度、分
        /// </summary>
        /// <param name="Angle">角度</param>
        /// <returns>度；分</returns>
        public Tuple<double, double> ConverToDegAndMin(double Angle)
        {
            var angles = Math.Truncate(Angle);
            var minutes = Math.Round((Angle - angles) * 60, 5);
            return new Tuple<double, double>(angles, minutes);
        }
        #endregion

        #region 度、分转度
        /// <summary>
        /// 度、分转度
        /// </summary>
        /// <param name="deg">度</param>
        /// <param name="min">分</param>
        /// <returns></returns>
        public double ConverToDegrade(double deg, double min)
        {
            return Math.Round(deg + min / 60, 7);
        }
        #endregion

        #region 根据两点经纬度计算方位、距离
        /// <summary>
        /// 根据两点经纬度计算方位、距离
        /// </summary>
        /// <param name="CenterPoint">中心点经纬度</param>
        /// <param name="TargetPoint">目标点经纬度</param>
        /// <returns>方位（度）、距离信息（米）</returns>
        public RelativeSpatialPosition ConvertAzimutInfo(Position CenterPoint, Position TargetPoint)
        {
            var L = ConverToRad(TargetPoint.Lng - CenterPoint.Lng);
            var tanU1 = (1 - f) * Math.Tan(ConverToRad(CenterPoint.Lat));
            var CosU1 = 1 / Math.Sqrt((1 + tanU1 * tanU1));
            var sinU1 = tanU1 * CosU1;
            var tanU2 = (1 - f) * Math.Tan(ConverToRad(TargetPoint.Lat));
            var CosU2 = 1 / Math.Sqrt((1 + tanU2 * tanU2));
            var sinU2 = tanU2 * CosU2;

            var lngintial = L;
            double lngtemp;
            var iterationLimit = 100;
            double CosSqα;
            double Cos2σM;
            double sinσ;
            double Cosσ;
            double σ;
            double sinλ;
            double Cosλ;
            do
            {
                sinλ = Math.Sin(lngintial);
                Cosλ = Math.Cos(lngintial);
                var sinSqσ = (CosU2 * sinλ) * (CosU2 * sinλ) + (CosU1 * sinU2 - sinU1 * CosU2 * Cosλ) * (CosU1 * sinU2 - sinU1 * CosU2 * Cosλ);
                sinσ = Math.Sqrt(sinSqσ);
                if (sinσ == 0)
                    return default;  // co-incident points
                Cosσ = sinU1 * sinU2 + CosU1 * CosU2 * Cosλ;
                σ = Math.Atan2(sinσ, Cosσ);
                var sinα = CosU1 * CosU2 * sinλ / sinσ;
                CosSqα = 1 - sinα * sinα;
                Cos2σM = Cosσ - 2 * sinU1 * sinU2 / CosSqα;
                if (double.IsNaN(Cos2σM))
                    Cos2σM = 0;  // equatorial line: CosSqα=0 (§6)
                var C = f / 16 * CosSqα * (4 + f * (4 - 3 * CosSqα));
                lngtemp = lngintial;
                lngintial = L + (1 - C) * f * sinα * (σ + C * sinσ * (Cos2σM + C * Cosσ * (-1 + 2 * Cos2σM * Cos2σM)));
            } while (Math.Abs(lngintial - lngtemp) > 1e-12 && --iterationLimit > 0);
            if (iterationLimit == 0)
                return default;

            var uSq = CosSqα * (a * a - b * b) / (b * b);
            var A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
            var B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
            var Δσ = B * sinσ * (Cos2σM + B / 4 * (Cosσ * (-1 + 2 * Cos2σM * Cos2σM) -
                B / 6 * Cos2σM * (-3 + 4 * sinσ * sinσ) * (-3 + 4 * Cos2σM * Cos2σM)));

            var s = b * A * (σ - Δσ);

            //var fwdAz = Math.Atan2(CosU2 * sinλ, CosU1 * sinU2 - sinU1 * CosU2 * Cosλ);
            var revAz = Math.Atan2(CosU1 * sinλ, -sinU1 * CosU2 + CosU1 * sinU2 * Cosλ);
            return new RelativeSpatialPosition()
            {
                Dis = b * A * (σ - Δσ),
                Az = ConverToDeg(revAz) > 0 ? ConverToDeg(revAz) : ConverToDeg(revAz) + 360
            };
        }
        #endregion

        #region 根据中心点经纬高、方位，距离计算目标点经纬高
        /// <summary>
        /// 根据中心点经纬高、方位，距离计算目标点经纬高
        /// </summary>
        /// <param name="centerPoint">中心点经纬高（高程）</param>
        /// <param name="az">方位角（度）</param>
        /// <param name="dis">距离（米）</param>
        /// <param name="realHeight">绝对高度（米）</param>
        /// <returns>目标点经纬高（高程）</returns>
        public Position Convert3DPosition(Position centerPoint, double az, double dis, double realHeight)
        {
            var postion = ConvertLonLat(centerPoint, az, dis);
            postion.Altitude = centerPoint.Altitude + realHeight;
            return postion;
        }

        private double ConverToRad(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }

        private double ConverToDeg(double x)
        {
            return x * 180 / Math.PI;
        }

        #endregion

        #region  已知一个点经纬度，方位角，距离，计算另一个点经纬度
        /// <summary>
        /// 已知一个点经纬度，方位角，距离，计算另一个点经纬度
        /// </summary>
        /// <param name="centerPoint">中心点经纬度</param>
        /// <param name="az">方位角（度）</param>
        /// <param name="dis">距离（米）</param>
        /// <returns>目标点经纬度</returns>
        public Position ConvertLonLat(Position centerPoint, double az, double dis)
        {
            double alpha1 = ConverToRad(az);
            double sinAlpha1 = Math.Sin(alpha1);
            double CosAlpha1 = Math.Cos(alpha1);

            double tanU1 = (1 - f) * Math.Tan(ConverToRad(centerPoint.Lat));
            double CosU1 = 1 / Math.Sqrt((1 + tanU1 * tanU1));
            double sinU1 = tanU1 * CosU1;
            double sigma1 = Math.Atan2(tanU1, CosAlpha1);
            double sinAlpha = CosU1 * sinAlpha1;
            double CosSqAlpha = 1 - sinAlpha * sinAlpha;
            double uSq = CosSqAlpha * (a * a - b * b) / (b * b);
            double A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
            double B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));

            double Cos2SigmaM = 0;
            double sinSigma = 0;
            double CosSigma = 0;
            double sigma = dis / (b * A), sigmaP = 2 * Math.PI;
            while (Math.Abs(sigma - sigmaP) > 1e-12)
            {
                Cos2SigmaM = Math.Cos(2 * sigma1 + sigma);
                sinSigma = Math.Sin(sigma);
                CosSigma = Math.Cos(sigma);
                double deltaSigma = B * sinSigma * (Cos2SigmaM + B / 4 * (CosSigma * (-1 + 2 * Cos2SigmaM * Cos2SigmaM)
                        - B / 6 * Cos2SigmaM * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * Cos2SigmaM * Cos2SigmaM)));
                sigmaP = sigma;
                sigma = dis / (b * A) + deltaSigma;
            }

            double tmp = sinU1 * sinSigma - CosU1 * CosSigma * CosAlpha1;
            double lat2 = Math.Atan2(sinU1 * CosSigma + CosU1 * sinSigma * CosAlpha1,
                    (1 - f) * Math.Sqrt(sinAlpha * sinAlpha + tmp * tmp));
            double lambda = Math.Atan2(sinSigma * sinAlpha1, CosU1 * CosSigma - sinU1 * sinSigma * CosAlpha1);
            double C = f / 16 * CosSqAlpha * (4 + f * (4 - 3 * CosSqAlpha));
            double L = lambda - (1 - C) * f * sinAlpha
                    * (sigma + C * sinSigma * (Cos2SigmaM + C * CosSigma * (-1 + 2 * Cos2SigmaM * Cos2SigmaM)));

            double revAz = Math.Atan2(sinAlpha, -tmp); // final bearing  
            return new Position()
            {
                Lat = ConverToDeg(lat2),
                Lng = centerPoint.Lng + ConverToDeg(L)
            };
        }
        #endregion

        #region 度与弧度互转
        /// <summary>
        /// 度与弧度互转
        /// </summary>
        /// <param name="angle">转换角度</param>
        /// <param name="ConverTodeg">true： 弧度转度；false：度转弧度</param>
        /// <returns></returns>
        public double ConverToDeg(double angle, bool ConverTodeg = true)
        {
            if (ConverTodeg)
                return angle * 180 / Math.PI;
            else
                return (Math.PI / 180) * angle;
        }
        #endregion

        #region 根据绝对高度、距离转俯仰度
        /// <summary>
        /// 根据绝对高度、距离装俯仰(距离小于10Km情况下不用考虑地区曲率的问题)
        /// </summary>
        /// <param name="realHeight">绝对高度（米）</param>
        /// <param name="dis">距离（米）</param>
        /// <returns>俯仰角（度）</returns>
        public double ConvertPich(double realHeight, double dis)
        {
            //return  Math.Atan(realHeight / dis) * 180 / Math.PI;//高度除以距离得到tanA

            if (dis >= 3000/*3km之后使用地球曲率*/)
            {
                double earthRadis = 6371004;
                double fA = earthRadis;
                double fB = dis;
                double fC = realHeight + earthRadis;
                return Math.Acos((fA * fA + fB * fB - fC * fC) / (2.0 * fA * fB)) * 180 / Math.PI - 90; //转换为伺服角度	
            }
            else
                return Math.Atan(realHeight / dis) * 180 / Math.PI;
        }
        #endregion

        #region 根据俯仰、距离转绝对高度
        /// <summary>
        /// 根据俯仰、距离转绝对高度(距离小于10Km情况下不用考虑地区曲率的问题)
        /// </summary>
        /// <param name="pitch">俯仰角（度）</param>
        /// <param name="dis">距离（米）</param>
        /// <returns>绝对高度（米）</returns>
        public double ConvertRealHeight(double pitch, double dis)
        {
            //return dis / Math.Tan((Math.PI / 180) * pitch);//距离除以正切值得到高度
            if (dis >= 3000/*3km之后使用地球曲率*/)
            {
                var earthRadis = 6371004.0;
                var fA = earthRadis;
                var fB = dis;
                var fC = Math.Sqrt(fA * fA + fB * fB - 2 * fA * fB * Math.Cos(Math.PI * (90 + pitch) / 180));
                return fC - earthRadis;
            }
            else
                return dis / Math.Tan((Math.PI / 180) * pitch);
        }
        #endregion



        /// <summary>
        /// 两点校北
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CorrectModel CalcCorrectByPoints(PointsCorrectNorthModel model)
        {
            try
            {
                //计算两点的方位俯仰和距离
                var _position = Convert3DPositionAzimuthAndPitchInfo(model.Target, model.Center);
                //当前方位指向-方位=方位纠偏[-180,180]
                CorrectModel mode = new CorrectModel();
                mode.Az = FormatNumber(model.CurrentAz - _position.Az);
                //当前俯仰指向-俯仰=俯仰纠偏[-90,90]
                mode.El = model.CurrentEl - _position.El;
                return mode;
            }
            catch (Exception es)
            {
                throw es;
            }
        }

        /// <summary>
        /// 相对校北
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RelativeSpatialPosition CalcCorrectByRelative(RelativeCorrectNorthModel model)
        {
            //根据设备A位置信息和方位距离计算目标点位置信息
            var target = Convert3DPosition(model.Center, model.Az, model.Dis, model.Center.Altitude);
            //根据设备B和目标点计算两点间的方位俯仰和距离
            var _position = Convert3DPositionAzimuthAndPitchInfo(model.Target, target);
            return _position;
        }

        /// <summary>
        /// 数据转换到区间[-180,180]
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double FormatNumber(double x)
        {
            if (x > 180 || x < -180)
            {
                var y = x % 360;
                if (y < -180)
                    return y + 360;
                else if (y > 180)
                    return y - 360;
                else
                    return y;
            }
            else
                return x;
        }

        /// <summary>
        /// 坐标转经纬度
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public PositionSimple PointToLatLng(double[] point)
        {
            if (point?.Length < 2) return default;
            double X = point[0], Y = point[1];
            double x = X / 20037508.34 * 180;
            double y = Y / 20037508.34 * 180;
            y = 180 / Math.PI * (2 * Math.Atan(Math.Exp(y * Math.PI / 180)) - Math.PI / 2);
            return new PositionSimple()
            {
                Lat = y,
                Lng = x
            };
        }
    }
}
