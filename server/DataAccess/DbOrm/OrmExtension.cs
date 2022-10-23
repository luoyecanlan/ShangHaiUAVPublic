using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm
{
    public static class OrmExtension
    {
        public static IServiceCollection AddOrmDb(this IServiceCollection services, string connStr)
        {
            LinqToDB.Data.DataConnection.DefaultSettings = new Linq2DBConnectionStringSettings(connStr);
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
            return services;
        }
    }
}
