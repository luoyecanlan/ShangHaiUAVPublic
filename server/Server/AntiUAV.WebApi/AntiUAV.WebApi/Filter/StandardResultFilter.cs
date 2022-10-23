using AntiUAV.WebApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Filter
{
    /// <summary>
    /// 返回结果统一标准化过滤器
    /// </summary>
    public class StandardResultFilter : ResultFilterAttribute
    {
        /// <summary>
        /// 重写结果序列化
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult)
            {
                var objectResult = context.Result as ObjectResult;
                context.Result = objectResult.StatusCode switch
                {
                    200 => new ObjectResult(new ServiceResponse<object>() { Code = ServiceResponseCode.Success, Message = "success", Data = objectResult.Value }),
                    _ => new ObjectResult(new ServiceResponse() { Code = ServiceResponseCode.Fail, Message = objectResult.Value.ToString() }),
                };
            }
            else if (context.Result is ContentResult)
            {
                var objectResult = context.Result as ContentResult;
                context.Result = objectResult.StatusCode switch
                {
                    200 => new ObjectResult(new ServiceResponse<object>() { Code = ServiceResponseCode.Success, Message = "success", Data = objectResult.Content }),
                    _ => new ObjectResult(new ServiceResponse { Code = ServiceResponseCode.Fail, Message = objectResult.Content }),
                };
            }
            else if(context.Result is FileResult)
            {

            }
            else
            {
                context.Result = new ObjectResult(new ServiceResponse()
                {
                    Code = ServiceResponseCode.UnKnowError,
                    Message = "未知的返回类型."
                });
            }
            //context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            base.OnResultExecuting(context);
        }
    }
}
