using AntiUAV.WebApi.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi.Middleware
{
    public class TokenVerificationMiddleware
    {

        private readonly RequestDelegate _next;

        public TokenVerificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var questUrl = httpContext.Request.Path.Value.ToUpperInvariant();
            //是否经过验证
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var tokenKey = httpContext.GetCurrentTokenCacheKey();
                var tokenmodel = await RedisHelper.GetAsync<TokenModel>(tokenKey);
                var old_tokenmodel = await RedisHelper.GetAsync<TokenModel>($"{tokenKey}_old");
                if (tokenmodel?.access_token != httpContext.GetCurrentToken() && old_tokenmodel?.access_token != httpContext.GetCurrentToken())//验证token是否是系统内的
                {
                    throw new AntiUAVAuthException("非系统内认证令牌.");
                }
            }
            await _next(httpContext);
        }
    }
}
