using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Models
{
    /// <summary>
    /// 业务异常类
    /// </summary>
    public class BussinessException : Exception, IBussinessException
    {
        /// <summary>
        /// 异常码
        /// </summary>
        public BussinessExceptionCode Code { get; private set; }
        /// <summary>
        /// http码(默认507)
        /// </summary>
        public int HttpCode { get; set; }

        /// <summary>
        /// 默认httpcode码
        /// </summary>
        private static readonly int _defHttpCode = 507;

        /// <summary>
        /// 业务异常构造
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="message">自定义错误消息</param>
        /// <param name="httpcode">自定义http码</param>
        public BussinessException(BussinessExceptionCode code, int httpcode, string message)
            : this(code, null, httpcode, message)
        {

        }

        /// <summary>
        /// 业务异常构造
        /// </summary>
        /// <param name="code">错误码</param>
        public BussinessException(BussinessExceptionCode code)
            : this(code, null, _defHttpCode, null)
        {

        }

        /// <summary>
        /// 业务异常构造
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="message">自定义错误消息</param>
        public BussinessException(BussinessExceptionCode code, string message)
            : this(code, null, _defHttpCode, message)
        {

        }

        /// <summary>
        /// 业务异常构造
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="inner">错误信息</param>
        /// <param name="message">自定义错误消息</param>
        public BussinessException(BussinessExceptionCode code, Exception inner, string message)
          : this(code, inner, _defHttpCode, message)
        {

        }

        /// <summary>
        /// 业务异常构造
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="inner">错误信息</param>
        public BussinessException(BussinessExceptionCode code, Exception inner)
          : this(code, inner, _defHttpCode, null)
        {

        }

        /// <summary>
        /// 业务异常构造
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="inner">错误信息</param>
        /// <param name="message">自定义错误消息</param>
        /// <param name="httpcode">自定义http码</param>
        public BussinessException(BussinessExceptionCode code, Exception inner, int httpcode, string message)
            : base($"[{code.GetValue()}] {code.GetDescription()}({message ?? "无"}).", inner)
        {
            Code = code;
            HttpCode = httpcode;
        }
    }

    /// <summary>
    /// 异常
    /// </summary>
    public interface IBussinessException
    {
        /// <summary>
        /// http码
        /// </summary>
        int HttpCode { get; }
    }
}
