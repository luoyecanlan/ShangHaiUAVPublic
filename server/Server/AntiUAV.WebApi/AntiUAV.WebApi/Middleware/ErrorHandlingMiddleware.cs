using AntiUAV.Bussiness.Models;
using AntiUAV.WebApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            this.next = next;
            _logger = logger;
        }

        private readonly ILogger _logger;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0059:不需要赋值", Justification = "<挂起>")]
        public async Task Invoke(HttpContext context)
        {
            bool isCatched = false;
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                isCatched = true;
                if (ex is IBussinessException)
                {
                    context.Response.StatusCode = ((IBussinessException)ex).HttpCode;
                }
                else
                {
                    context.Response.StatusCode = 505;
                    _logger.LogError(ex, "服务器未知错误.");
                }
                await HandleExceptionAsync(context, new ServiceResponse<object>()
                {
                    Code = ServiceResponseCode.Fail,
                    HttpCode = context.Response.StatusCode,
                    Message = ex.Message
                });
            }
            finally
            {
                if (!isCatched && context.Response.StatusCode != 200)//未捕捉过并且状态码不为200
                {
                    var res = context.Response.StatusCode switch
                    {
                        401 => new ServiceResponse() { Code = ServiceResponseCode.NoPermission, Message = "未登录", HttpCode = context.Response.StatusCode },
                        403 => new ServiceResponse() { Code = ServiceResponseCode.NoPermission, Message = "未授权", HttpCode = context.Response.StatusCode },
                        404 => new ServiceResponse() { Code = ServiceResponseCode.NotFoundService, Message = "未找到服务", HttpCode = context.Response.StatusCode },
                        502 => new ServiceResponse() { Code = ServiceResponseCode.RequestError, Message = "请求错误", HttpCode = context.Response.StatusCode },
                        _ => new ServiceResponse() { Code = ServiceResponseCode.UnKnowError, Message = "未知错误", HttpCode = context.Response.StatusCode },
                    };

                    //await HandleExceptionAsync(context, res);//此处是要测试下如何能不返回错误
                }
            }
        }
        private static async Task HandleExceptionAsync(HttpContext context, ServiceResponse data)
        {
            if (context.Request.ContentType?.ToLower().ToString().Contains("application/json") ?? true)
            {
                var result = JsonConvert.SerializeObject(data);
                context.Response.ContentType = "application/json;charset=utf-8";
                //context.Response.Headers["Access-Control-Allow-Origin"] = "*";//如果不加此句，服务器返回的数据到浏览器会拒绝
                await context.Response.WriteAsync(result);
            }
            return;
        }
    }
}
