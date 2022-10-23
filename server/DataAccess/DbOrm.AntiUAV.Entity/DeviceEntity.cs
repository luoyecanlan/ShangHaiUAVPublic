using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    /// <summary>
    /// 设备POCO实体基类
    /// </summary>
    [Table("device")]
    public abstract class DeviceBase { }

    /// <summary>
    /// 设备POCO实体基类（带主键）
    /// </summary>
    public abstract class DeviceKeyBase : DeviceBase, IEntityKeyProperty
    {
        [Column("id"), PrimaryKey, Identity]
        public int Id { get; set; }
    }

    /// <summary>
    /// 设备ID类
    /// </summary>
    public class DeviceKey: DeviceKeyBase
    {

    }

    /// <summary>
    /// 设备新建POCO实体
    /// </summary>
    public class DeviceAdd : DeviceBase
    {
        [Column("name"), NotNull]
        public string Name { get; set; }//设备名称
        [Column("display"), Nullable]
        public string Display { get; set; }//设备描述
        [Column("category"), NotNull] 
        public int Category { get; set; } // 设备类型
        [Column("ip"), NotNull]
        public string Ip { get; set; }//设备ip
        [Column("port"), NotNull]
        public int Port { get; set; }//设备端口
        [Column("lip"), NotNull]
        public string Lip { get; set; } // 监听IP
        [Column("lport"), NotNull]
        public int Lport { get; set; } // 监听端口
        [Column("lat"), NotNull]
        public double Lat { get; set; }//位置纬度
        [Column("lng"), NotNull]
        public double Lng { get; set; }//位置经度
        [Column("alt"), NotNull]
        public double Alt { get; set; }//位置海拔
        [Column("coverR"), NotNull]
        public double CoverR { get; set; }//覆盖半径

        [Column("coverS"), NotNull]
        public double CoverS { get; set; }//覆盖起始角

        [Column("coverE"), NotNull]
        public double CoverE { get; set; }//覆盖结束角

        [Column("rectifyAz"), NotNull]
        public double RectifyAz { get; set; }//方位纠偏

        [Column("rectifyEl"), NotNull]
        public double RectifyEl { get; set; }//俯仰纠偏
        [Column("threadAssessmentCount"), NotNull]
        public int ThreadAssessmentCount { get; set; }//威胁判定N个点判定一次
        [Column("targetTimeOut"), NotNull]
        public int TargetTimeOut { get; set; }//目标超时时间（秒）
        [Column("statusReportingInterval"), NotNull] 
        public int StatusReportingInterval { get; set; } // 状态上报时间间隔
        [Column("probeReportingInterval"), NotNull] 
        public int ProbeReportingInterval { get; set; } // 航迹上报时间间隔
    }

    /// <summary>
    /// 设备删除POCO实体
    /// </summary>
    public class DeviceDel : DeviceKeyBase { }

    /// <summary>
    /// 设备类型POCO实体
    /// </summary>
    [Table("dic_device_category")]
    public class DeviceCategoryInfo: IEntityKeyProperty
    {
        [Column("id"), PrimaryKey, Identity]
        public int Id { get; set; }
        [Column("name"), NotNull]
        public string Name { get; set; }//设备名称
    }

    public class DeviceSimpleInfo : DeviceKeyBase
    {
        [Column("name"), NotNull]
        public string Name { get; set; }//设备名称
        [Column("category"), NotNull]
        public int Category { get; set; } // 设备类型
    }

    /// <summary>
    /// 设备信息POCO实体
    /// </summary>
    public class DeviceInfo : DeviceKeyBase
    {
        [Column("name"), NotNull]
        public string Name { get; set; }//设备名称
        [Column("display"), Nullable]
        public string Display { get; set; }//设备描述
        [Column("category"), NotNull]
        public int Category { get; set; } // 设备类型
        [Column("ip"), NotNull]
        public string Ip { get; set; }//设备ip
        [Column("port"), NotNull]
        public int Port { get; set; }//设备端口
        [Column("lip"), NotNull]
        public string Lip { get; set; } // 监听IP
        [Column("lport"), NotNull]
        public int Lport { get; set; } // 监听端口
        [Column("lat"), NotNull]
        public double Lat { get; set; }//位置纬度
        [Column("lng"), NotNull]
        public double Lng { get; set; }//位置经度
        [Column("alt"), NotNull]
        public double Alt { get; set; }//位置海拔
        [Column("coverR"), NotNull]
        public double CoverR { get; set; }//覆盖半径

        [Column("coverS"), NotNull]
        public double CoverS { get; set; }//覆盖起始角

        [Column("coverE"), NotNull]
        public double CoverE { get; set; }//覆盖结束角

        [Column("rectifyAz"), NotNull]
        public double RectifyAz { get; set; }//方位纠偏

        [Column("rectifyEl"), NotNull]
        public double RectifyEl { get; set; }//俯仰纠偏
        [Column("threadAssessmentCount"), NotNull]
        public int ThreadAssessmentCount { get; set; }//威胁判定N个点判定一次
        [Column("targetTimeOut"), NotNull]
        public int TargetTimeOut { get; set; }//目标超时时间（秒）
        [Column("statusReportingInterval"), NotNull]
        public int StatusReportingInterval { get; set; } // 状态上报时间间隔
        [Column("probeReportingInterval"), NotNull]
        public int ProbeReportingInterval { get; set; } // 航迹上报时间间隔
        [Column("createtime"), NotNull]
        public DateTime CreateTime { get; set; } // 设备创建时间
        [Column("updatetime"), NotNull]
        public DateTime UpdateTime { get; set; } // 设备更新时间
    }

    /// <summary>
    /// 设备信息更新POCO实体
    /// </summary>
    public class DeviceUpdate : DeviceKeyBase
    {
        [Column("name"), NotNull]
        public string Name { get; set; }//设备名称
        [Column("display"), Nullable]
        public string Display { get; set; }//设备描述
        [Column("ip"), NotNull]
        public string Ip { get; set; }//设备ip
        [Column("port"), NotNull]
        public int Port { get; set; }//设备端口
        [Column("lip"), NotNull] 
        public string Lip { get; set; } // 监听IP
        [Column("lport"), NotNull] 
        public int Lport { get; set; } // 监听端口
        [Column("lat"), NotNull]
        public double Lat { get; set; }//位置纬度
        [Column("lng"), NotNull]
        public double Lng { get; set; }//位置经度
        [Column("alt"), NotNull]
        public double Alt { get; set; }//位置海拔
        [Column("coverR"), NotNull]
        public double CoverR { get; set; }//覆盖半径

        [Column("coverS"), NotNull]
        public double CoverS { get; set; }//覆盖起始角

        [Column("coverE"), NotNull]
        public double CoverE { get; set; }//覆盖结束角
        [Column("rectifyAz"), NotNull]
        public double RectifyAz { get; set; }//方位纠偏

        [Column("rectifyEl"), NotNull]
        public double RectifyEl { get; set; }//俯仰纠偏
        [Column("threadAssessmentCount"), NotNull]
        public int ThreadAssessmentCount { get; set; }//威胁判定N个点判定一次
        [Column("targetTimeOut"), NotNull]
        public int TargetTimeOut { get; set; }//目标超时时间（秒）
        [Column("statusReportingInterval"), NotNull]
        public int StatusReportingInterval { get; set; } // 状态上报时间间隔
        [Column("probeReportingInterval"), NotNull]
        public int ProbeReportingInterval { get; set; } // 航迹上报时间间隔
    }

    /// <summary>
    /// 设备位置更新POCO实体
    /// </summary>
    public class DeviceUpdatePosition : DeviceKeyBase
    {
        [Column("lat"), NotNull]
        public double Lat { get; set; }//位置纬度
        [Column("lng"), NotNull]
        public double Lng { get; set; }//位置经度
        [Column("alt"), NotNull]
        public double Alt { get; set; }//位置海拔
        [Column("rectifyAz"), NotNull]
        public double RectifyAz { get; set; }//方位纠偏

        [Column("rectifyEl"), NotNull]
        public double RectifyEl { get; set; }//俯仰纠偏
    }

    /// <summary>
    /// 设备基础信息
    /// </summary>
    public class DeviceBaseInfo : DeviceUpdatePosition
    {
        [Column("name"),NotNull]
        public string Name { get; set; }    //设备名称
    }
}
