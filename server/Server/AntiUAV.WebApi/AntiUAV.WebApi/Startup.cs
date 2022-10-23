using AntiUAV.WebApi.Bgs;
using AntiUAV.WebApi.Filter;
using AntiUAV.WebApi.Middleware;
using AntiUAV.WebApi.Model;
using AntiUAV.Bussiness;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;

namespace AntiUAV.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 配置信息
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddGrpc();
            services.AddControllers(options =>
                    {
                        options.Filters.Add<StandardResultFilter>();
                    })
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                        options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConvert.DateTimeConverter());
                        options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConvert.DateTimeNullableConverter());
                        //options.JsonSerializerOptions.IgnoreNullValues = true;
                    });
            services.AddCors(options =>
            {
                options.AddPolicy("antiuav", policy =>
                 {
                     var host = Configuration.GetValue("CrosHost", "*");
                     policy.WithOrigins(host.Split(','))//注意端口号后不要带/斜杆：比如localhost:8000/
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                 });
            });
            services.AddSignalRHost();
            services.AddAuthService(Configuration)
                    .AddDbService(Configuration)
                    .AddAntiUAVBussinessServices(Configuration)
                    .AddConfigService(Configuration)
                    .AddRabbitMqService(Configuration)
                    //.AddMetadataBussinessService()
                    //.AddRedisCache(Configuration)
                    .AddMemoryCache()
                    //.AddReverseCallService(Configuration)
                    .AddSwaggerService();
            //services.AddSingleton(typeof(DeviceRpcClient));
            services.AddHostedService<TargetCleanBgs>()
                .AddHostedService<TargetPushService>()
                .AddHostedService<DevStatusService>()
                .AddHostedService<ObstructControlReceiveService>()
                //.AddHostedService<SummaryBgService>()
                .AddHostedService<RelationShipService>();

            //.AddHostedService<MockDataService>();
        }

        /// <summary>
        ///  This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);//189报505错误的问题，目前看解决不了
            NLog.Web.NLogBuilder.ConfigureNLog("NLog.config");
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseRouting();

            app.UseCors("antiuav");

            app.UseAuthService()
               .UseSwaggerService()
               .UseMiddleware<TokenVerificationMiddleware>()
                .UseStaticFiles(new StaticFileOptions()//自定义自己的文件路径
                {
                    FileProvider = new PhysicalFileProvider($"{AppContext.BaseDirectory}export"),//指定实际物理路径
                    RequestPath = new PathString("/api/export")//对外的访问路径
                }); 
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGrpcService<CenterRpcServer>();
                endpoints.MapHub<SignalRHub>(SignalRHub.hubRouter);
                endpoints.MapControllers();
            });
        }
    }
}
