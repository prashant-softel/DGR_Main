using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGRAPIs.Helper;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
namespace DGRAPIs.Helper
{
    public class DatabaseProvider
    {
        private readonly string MainConnection;
        public readonly IConfiguration _configuration;
        public DatabaseProvider(IConfiguration configuration)
        {
            /*if (configuration.GetConnectionString("AllowConnection") == "1")
            {
                MainConnection = configuration.GetConnectionString("Con");
            }
            else 
            {
            */
                MainConnection = configuration.GetConnectionString("Con");
            //}
            
        }
        private MYSQLDBHelper GetSqlInstance(int timeout = -1)
        {
            string connstr = MainConnection; // SQL Server connection string
            return new MYSQLDBHelper(connstr);
        }
        public MYSQLDBHelper SqlInstance(int timeout = -1) => GetSqlInstance(timeout);
    }
}
