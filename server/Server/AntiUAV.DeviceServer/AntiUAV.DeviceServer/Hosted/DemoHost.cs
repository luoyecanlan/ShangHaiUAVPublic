using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestDemo.Hosted
{
    public class DemoHost : BackgroundService
    {
        //public DemoHost(ILogger<DemoHost> logger, IMemoryCache memory, IHttpClientFactory factory)
        //{
        //    _memory = memory;
        //    _logger = logger;
        //    _factory = factory;
        //}

        private readonly IMemoryCache _memory;
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _factory;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var res = await _factory.CreateClient("testClient").GetAsync("www.baidu.com");
                        var data = await res.Content.ReadAsStringAsync();
                        _memory.Set("请求数据缓存键值", data);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "数据获取错误.");
                        await Task.Delay(2000);
                    }
                }
            });
        }
    }


    public class xxxxController : Controller
    {
        public xxxxController(IMemoryCache memory)
        {
            _memory = memory;
        }

        private readonly IMemoryCache _memory;

        public IActionResult GetView()
        {
            if (_memory.TryGetValue("请求数据缓存键值", out string data))
            {
                return View(data);
            }
            return View("没有找到数据");
        }
    }
}
