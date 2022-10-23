using DbOrm;
using DbOrm.AntiUAV;
using DbOrm.CRUD;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV
{
    public static class DbAntiUAVExtension
    {
        public static IServiceCollection AddDbService(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddOrmDb(configuration.GetConnectionString("MySql"))
                           .AddTransient<IEntityCrudService, AntiUAVEntityCrudService>();
        }
    }
}
