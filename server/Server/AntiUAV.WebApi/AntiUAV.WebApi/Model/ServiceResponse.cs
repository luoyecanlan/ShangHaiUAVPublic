using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.WebApi.Model
{
    /// <summary>
    /// 结果标准化模型
    /// </summary>
    public class ServiceResponse
    {
        /// <summary>
        /// HttpCode
        /// </summary>
        public double HttpCode { get; set; }
        /// <summary>
        /// 服务码
        /// </summary>
        public ServiceResponseCode Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 结果标准化模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceResponse<T> : ServiceResponse
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// 服务码枚举
    /// </summary>
    public enum ServiceResponseCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,
        /// <summary>
        /// 失败
        /// </summary>
        Fail,
        /// <summary>
        /// 异常
        /// </summary>
        Error,
        /// <summary>
        /// 无权限
        /// </summary>
        NoPermission,
        /// <summary>
        /// 未找到服务
        /// </summary>
        NotFoundService,
        /// <summary>
        /// 请求异常
        /// </summary>
        RequestError,
        /// <summary>
        /// 未知异常
        /// </summary>
        UnKnowError
    }
}
