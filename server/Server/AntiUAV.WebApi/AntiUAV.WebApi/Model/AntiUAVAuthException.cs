using AntiUAV.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.WebApi.Model
{
    /// <summary>
    /// 认证异常
    /// </summary>
    public class AntiUAVAuthException : Exception, IBussinessException
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="message"></param>
        /// <param name="httpCode"></param>
        public AntiUAVAuthException(string message, int httpCode = 403) : base(message)
        {
            HttpCode = httpCode;
        }

        /// <summary>
        /// http码
        /// </summary>
        public int HttpCode { get; private set; }
    }
}
