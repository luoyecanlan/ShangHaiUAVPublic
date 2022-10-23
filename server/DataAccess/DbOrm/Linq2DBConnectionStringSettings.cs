using LinqToDB.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm
{
    public class Linq2DBConnectionStringSettings : ILinqToDBSettings
    {
        public Linq2DBConnectionStringSettings(string conn, string type = "MySql")
        {
            _connStr = conn;
            DefaultDataProvider = type;
        }
        private string _connStr { get; set; }
        public IEnumerable<IDataProviderSettings> DataProviders { get { yield break; } }

        public string DefaultConfiguration => "Linq2DbConfigur";

        public string DefaultDataProvider { get; }

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = DefaultConfiguration,
                        ProviderName = DefaultDataProvider,
                        ConnectionString = _connStr
                    };
            }
        }
    }
}
