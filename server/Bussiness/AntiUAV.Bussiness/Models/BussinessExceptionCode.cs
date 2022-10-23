using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 异常错误码
    /// </summary>
    public enum BussinessExceptionCode
    {
        /// <summary>
        /// 参数信息无效
        /// </summary>
        [Description("参数信息无效")]
        ParamNull = 10000,
        /// <summary>
        /// 无效ID
        /// </summary>
        [Description("无效ID")]
        ParamInvalidId = 10001,
        /// <summary>
        /// 分页参数无效
        /// </summary>
        [Description("分页参数无效")]
        ParamPagingInvalid = 10002,
        /// <summary>
        /// 时间区间无效
        /// </summary>
        [Description("时间区间无效")]
        ParamTimeIntervalInvalid = 10003,
        /// <summary>
        /// 密码无效
        /// </summary>
        [Description("密码无效")]
        PasswordInvalid = 10004,
        /// <summary>
        /// 用户名无效
        /// </summary>
        [Description("用户名无效")]
        UserNameInvalid = 10005,
        /// <summary>
        /// 无效Code
        /// </summary>
        [Description("无效Code")]
        ParamInvalidCode = 10006,
        /// <summary>
        /// 无效的权限分配
        /// </summary>
        [Description("无效的权限分配")]
        InvalidRoleAssignment = 10007,
        /// <summary>
        /// 新建操作失败
        /// </summary>
        [Description("新建操作失败")]
        OptAddFail = 20000,
        /// <summary>
        /// 删除操作失败
        /// </summary>
        [Description("删除操作失败")]
        OptDelFail = 20001,
        /// <summary>
        /// 更新操作失败
        /// </summary>
        [Description("更新操作失败")]
        OptUpdateFail = 20002,
        /// <summary>
        /// 读取操作失败
        /// </summary>
        [Description("读取操作失败")]
        OptGetFail = 20003,
        /// <summary>
        /// 关联操作失败
        /// </summary>
        [Description("关联操作失败")]
        OptMapFail = 20004,
        /// <summary>
        /// 修改密码失败
        /// </summary>
        [Description("修改密码失败")]
        OptUpdatPwdFail = 20005,
        /// <summary>
        /// 重置密码失败
        /// </summary>
        [Description("重置密码失败")]
        OptRestPwdFail = 20006,
        /// <summary>
        /// 数据计算失败
        /// </summary>
        [Description("数据计算失败")]
        CaclDataNull = 30000,
        /// <summary>
        /// 未找到可用服务
        /// </summary>
        [Description("未找到可用服务")]
        ServiceNotFound = 30001,
        /// <summary>
        /// 未知的调用方式
        /// </summary>
        [Description("未知的调用方式")]
        UnKnownCallMode = 30002,
        /// <summary>
        /// 远程执行命令失败
        /// </summary>
        [Description("远程执行命令失败")]
        RemoteCallFail = 30003,
        /// <summary>
        /// 目标附近没有可用设备
        /// </summary>
        [Description("目标附近没有可用设备")]
        NoEquipmentAvailable = 40000,
        /// <summary>
        /// 未找到设备服务启动文件
        /// </summary>
        [Description("未找到设备服务启动文件")]
        NotFindDeviceServerFile = 40001,
        /// <summary>
        /// 关联关系写入Redis失败
        /// </summary>
        [Description("关联关系写入Redis失败")]
        RelationShipWriteError = 50000,
        /// <summary>
        /// 移除关联关系失败
        /// </summary>
        [Description("移除关联关系失败")]
        RemoveShipWriteError = 50001,
        /// <summary>
        /// Excle模板文件未找到
        /// </summary>
        [Description("模板文件未找到")]
        TemplateNotFound = 9000,
        /// <summary>
        /// 文件生成失败
        /// </summary>
        [Description("文件生成失败")]
        CreateFileFail = 9001,
        /// <summary>
        /// 未找到对应日志文件
        /// </summary>
        [Description("未找到对应日志文件")]
        LogFileNotFound=9002
    }
}
