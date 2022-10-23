using AntiUAV.Bussiness.Models;
using AntiUAV.DeviceServer.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.DeviceServer.Abstractions.PluginService.ServiceImpl
{
    public abstract class DeviceOptServiceBase : IDeviceOptService
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public abstract int DeviceCategory { get; }


        /// <summary>
        /// 打击下发数据转换
        /// </summary>
        /// <param name="json">下发数据的json字符串，因为这个是从指控端发出，且现在指控端UI会是什么样也没有定数，
        /// 所以这里直接是一个序列化号的json，可以随意发挥的，譬如是我需要的某某某数据，或者直接是一个可以使用的东西</param>
        /// <param name="sw">这个是开关，干扰开启还是关闭的开关switch的简写</param>
        /// <returns></returns>
        public virtual byte[] GetAttackBuff(string json, bool sw) => default;
        /// <summary>
        /// 引导下发数据转换
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual byte[] GetGuidanceBuff(GuidancePositionInfo position) => default;

        /// <summary>
        /// 跟踪下发数据转换
        /// </summary>
        /// <param name="json"></param>
        /// <param name="sw"></param>
        /// <returns></returns>
        public virtual byte[] GetMonitorBuff(string json, bool sw) => default;
        /// <summary>
        /// 位置下发数据转换
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="alt"></param>
        /// <returns></returns>
        public virtual byte[] GetPositionBuff(double lat, double lng, double alt) => default;
        /// <summary>
        /// 纠偏下发数据转换
        /// </summary>
        /// <param name="az">方位</param>
        /// <param name="el">俯仰</param>
        /// <returns></returns>
        public virtual byte[] GetRectifyBuff(double az, double el) => default;

        /// <summary>
        /// 设备操作指令包
        /// </summary>
        /// <param name="oper">1-水平 / 2-俯仰 / 3-归零（转台0位） / 4-指北（修正后0位） / 5-视场角 / 6-焦距</param>
        /// <param name="speed">转台速度 / 视场角增减速度 / 焦距增减速度</param>
        /// <param name="status">true 开始 false 关闭</param>
        /// <returns></returns>
        public virtual byte[] GetDeviceOpBuff(short operationItem, int speed, short operationCode) => default;

    }
}
