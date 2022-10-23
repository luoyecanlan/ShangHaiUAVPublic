using AntiUAV.WebApi.Config;
using AntiUAV.WebApi.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AntiUAV.WebApi
{
    public static class AuthExtension
    {
        public static IServiceCollection AddAuthService(this IServiceCollection services, IConfiguration config)
        {
            //注入授权Handler
            services.AddAuthorization();
            services.AddAuthentication(s =>
            {
                //添加JWT Scheme
                s.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                s.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                s.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = AuthConfig.GetTokenValidationParameters(config);
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        //Token expired
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            return services;
        }

        public static IApplicationBuilder UseAuthService(this IApplicationBuilder app)
        {
            //添加jwt验证
            return app.UseAuthentication()
                      .UseAuthorization();
        }

        /// <summary>
        /// 获取当前请求的令牌缓存KEY
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetCurrentTokenCacheKey(this HttpContext context) => $"Token_{context?.GetCurrentUserId()}";

        /// <summary>
        /// 获取指定的令牌缓存KEY
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetTokenCacheKey(this HttpContext context, string token = null) => $"Token_{token ?? context?.GetCurrentUserId()}";

        /// <summary>
        /// 获取当前请求的令牌
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetCurrentToken(this HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues current))
            {
                return current.ToString().Replace("Bearer ", "");
            }
            return default;
        }

        /// <summary>
        /// 获取当前请求用户名
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetCurrentUsername(this HttpContext context)
        {
            return RedisHelper.Get<TokenModel>(context.GetCurrentTokenCacheKey())?.username ?? "未知用户";
        }

        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetCurrentUserId(this HttpContext context)
        {
            return context.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
        }

        /// <summary>
        /// 获取当前用户角色
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetCurrentUserRole(this HttpContext context)
        {
            return context.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Role).Value;
        }
    }
}
