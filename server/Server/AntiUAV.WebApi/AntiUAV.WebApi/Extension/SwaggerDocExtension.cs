using AntiUAV.WebApi.SwaggerDoc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AntiUAV.WebApi
{
    public static class SwaggerDocExtension
    {
        public static IServiceCollection AddSwaggerService(this IServiceCollection services)
        {
            #region 注册Swagger服务
            services.AddSwaggerGen(c =>
            {
                #region 加载文档内容
                var title = "指挥控制平台API";
                var description = "低空安防反无人机指控平台API接口";
                //遍历ApiGroupNames所有枚举值生成接口文档，Skip(1)是因为Enum第一个FieldInfo是内置的一个Int值
                typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    //获取枚举值上的特性
                    var info = f.GetCustomAttributes(typeof(GroupInfoAttribute), false).OfType<GroupInfoAttribute>().FirstOrDefault();
                    // swagger文档配置
                    c.SwaggerDoc(f.Name, new OpenApiInfo
                    {
                        Version = info?.Version,
                        Title = $"{title} - {info?.Title} 文档",
                        Description = $"{description} - {info?.Description} - {info.Version}",
                        Contact = new OpenApiContact { Name = "北京蓝警科技有限公司", Email = "liuhao@blueengine.com.cn", Url = new Uri("http://www.blueengine.com.cn") },
                        License = new OpenApiLicense { Name = "北京蓝警科技有限公司 - 许可证", Url = new Uri("https://www.jianshu.com/u/94102b59cc2a") }
                    });
                    // 接口排序
                    c.OrderActionsBy(o => o.RelativePath);
                });
                //没有加特性的分到这个NoGroup上
                c.SwaggerDoc("NoGroup", new OpenApiInfo
                {
                    Title = "其他API",
                    Version = "未知",
                    Description = $"{description} - 未明确分组的API",
                    Contact = new OpenApiContact { Name = "北京蓝警科技有限公司", Email = "liuhao@blueengine.com.cn", Url = new Uri("http://www.blueengine.com.cn") },
                    License = new OpenApiLicense { Name = "北京蓝警科技有限公司 - 许可证", Url = new Uri("https://www.jianshu.com/u/94102b59cc2a") }
                });
                //判断接口归于哪个分组
                c.DocInclusionPredicate((docName, apiDescription) =>
                {
                    if (docName == "NoGroup")
                    {
                        return string.IsNullOrEmpty(apiDescription.GroupName);//当分组为NoGroup时，只要没加特性的都属于这个组
                    }
                    else
                    {
                        return apiDescription.GroupName == docName;
                    }
                });
                #endregion

                #region 重名报错解决
                c.CustomSchemaIds(type => type.FullName); // 解决相同类名会报错的问题
                #endregion

                #region 注释数据加载
                //添加读取注释服务
                var directory = PlatformServices.Default.Application.ApplicationBasePath;//AppDomain.CurrentDomain.BaseDirectory;
                // 找到所有的注释文档， 根据你的项目情况加上通配符。
                var files = Directory.GetFiles(directory, "*.xml").ToList();

                foreach (var file in files)
                {
                    c.IncludeXmlComments(file, true);//添加控制器层注释（true表示显示控制器注释）
                }
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                #endregion

                #region auth授权添加token
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入\"Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                #endregion
            });
            #endregion

            return services;
        }

        public static IApplicationBuilder UseSwaggerService(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "云波智慧鸟情监测分析系统API V1");
                //遍历ApiGroupNames所有枚举值生成接口文档，Skip(1)是因为Enum第一个FieldInfo是内置的一个Int值
                typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    //获取枚举值上的特性
                    var info = f.GetCustomAttributes(typeof(GroupInfoAttribute), false).OfType<GroupInfoAttribute>().FirstOrDefault();
                    options.SwaggerEndpoint($"/swagger/{f.Name}/swagger.json", info != null ? info.Title : f.Name);

                });
                options.SwaggerEndpoint("/swagger/NoGroup/swagger.json", "其他API");

            });
            return app;
        }
    }
}
