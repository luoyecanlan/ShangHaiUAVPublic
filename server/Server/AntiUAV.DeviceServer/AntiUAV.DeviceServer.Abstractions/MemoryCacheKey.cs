using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Abstractions
{
    /// <summary>
    /// 内存缓存键值
    /// </summary>
    public class MemoryCacheKey
    {
        /// <summary>
        /// 设备信息键值
        /// </summary>
        public const string DeviceInfoKey = "DeviceInfoKey";

        /// <summary>
        /// 预警区信息键值
        /// </summary>
        public const string DevicePerWarningZoneKey = "DevicePerWarningZoneKey";

        /// <summary>
        /// 设备状态键值
        /// </summary>
        public const string DeviceStatusKey = "DeviceStatusKey";

        /// <summary>
        /// 关联关系键值
        /// </summary>
        public const string RelationshipsKey = "RelationshipsKey";

        /// <summary>
        /// 目标航迹点集合
        /// </summary>
        public const string TargetPointsKey = "TgPointsKey";

        /// <summary>
        /// 目标威胁度键值
        /// </summary>
        public const string TargetAssessmentKey = "TargetAssessmentKey";

        /// <summary>
        /// 纠偏信息事件键值
        /// </summary>
        public const string BusEventRectifyInfoKey = "RectifyInfoKey";

        /// <summary>
        /// 追踪开关事件键值
        /// </summary>
        public const string BusEventMonitorSwKey = "MonitorSwKey";

        /// <summary>
        /// 打击开关事件键值
        /// </summary>
        public const string BusEventAttackSwKey = "AttackSwKey";

        /// <summary>
        /// 设备纠偏信息键值（实际）
        /// </summary>
        public const string DevRectifyInfoKey = "DevRectifyInfoKey";

        /// <summary>
        /// 设备位置信息键值（实际）
        /// </summary>
        public const string DevPositionInfoKey = "DevPositionInfoKey";
    }
}
