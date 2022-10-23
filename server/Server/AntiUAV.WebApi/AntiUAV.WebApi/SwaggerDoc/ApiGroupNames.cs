using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.SwaggerDoc
{
    /// <summary>
    /// 系统分组枚举值
    /// </summary>
    public enum ApiGroupNames
    {
        //[GroupInfo(Title = "设备管理API", Description = "设备新增,修改,删除", Version = "v1")]
        //Device,
        //[GroupInfo(Title = "告警区域管理API", Description = "告警区域新增,修改,删除", Version = "v1")]
        //AlarmArea,
        /// <summary>
        /// 元数据
        /// </summary>
        [GroupInfo(Title = "元数据管理API", Description = "设备,告警区,用户信息管理", Version = "v1")]
        Meatdata,
        /// <summary>
        /// 设备操作
        /// </summary>
        [GroupInfo(Title = "设备操作API", Description = "设备服务开关,设备运行指令下发", Version = "v1")]
        DeviceOperation,
        /// <summary>
        /// 访问操作
        /// </summary>
        [GroupInfo(Title = "身份认证API", Description = "登录,注销,token失效后刷新", Version = "v1")]
        Access,
        /// <summary>
        /// 实时操作
        /// </summary>
        [GroupInfo(Title = "实时数据API", Description = "目标航迹,设备状态实时数据获取", Version = "v1")]
        RealTime,
        /// <summary>
        /// 令牌
        /// </summary>
        [GroupInfo(Title = "身份认证Token管理API", Description = "token查看，注销操作", Version = "v1")]
        Token,
        /// <summary>
        /// 历史目标
        /// </summary>
        [GroupInfo(Title = "历史目标操作API", Description = "历史目标查看，删除操作", Version = "v1")]
        TargetHistory,
        /// <summary>
        /// 地理信息相关
        /// </summary>
        [GroupInfo(Title = "Gis相关算法API", Description = "Gis相关算法", Version = "v1")]
        Gis,
        /// <summary>
        /// 配置
        /// </summary>
        [GroupInfo(Title = "配置信息相关API", Description = "配置信息相关", Version = "v1")]
        Config

        ///// <summary>
        ///// 访问令牌API(全平台通用)
        ///// </summary>
        //[GroupInfo(Title = "访问令牌API(全平台通用)", Description = "身份认证及授权接口，用于全平台访问令牌的颁发、注销及刷新。", Version = "v1")]
        //AccessToken,
        ///// <summary>
        ///// 令牌管理API（后台服务）
        ///// </summary>
        //[GroupInfo(Title = "令牌管理API", Description = "全平台所有授权令牌的管理。", Version = "v1")]
        //Token,
        ///// <summary>
        ///// 用户信息API（全平台通用）
        ///// </summary>
        //[GroupInfo(Title = "用户信息API（全平台通用）", Description = "仅对当前登录用户自己信息管理的接口，适用全平台内所有授权过的用户。", Version = "v1")]
        //User,
        ///// <summary>
        ///// 元数据管理API（后台服务）
        ///// </summary>
        //[GroupInfo(Title = "元数据管理API（后台服务）", Description = "反无人机平台内的元数据管理，针对用户、设备、预警区等基础数据的管理，同时监控各服务状态等操作的API接口。", Version = "v1")]
        //MeatdataServer,
        ///// <summary>
        ///// 指挥控制平台API（客户端）
        ///// </summary>
        //[GroupInfo(Title = "指挥控制平台API", Description = "指控平台客户端API接口，包括客户端对目标的查看及操作，设备的查看及操作，一整套的预警展示", Version = "v1")]
        //Client,
        ///// <summary>
        ///// 设备服务API（设备服务）
        ///// </summary>
        //[GroupInfo(Title = "设备服务API", Description = "单一设备信息提交API，负责数据采集和数据提交工作，与客户端保持心跳短链接", Version = "v1")]
        //Device,
        ///// <summary>
        ///// 设备操作服务API（设备服务）
        ///// </summary>
        //[GroupInfo(Title = "设备操作服务API", Description = "与设备服务下行交互的API", Version = "v1")]
        //DeviceOpt

    }
}
