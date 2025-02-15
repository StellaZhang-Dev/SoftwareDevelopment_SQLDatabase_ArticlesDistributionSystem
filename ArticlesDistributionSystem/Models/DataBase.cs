using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.SqlClient;
using System.Data;
using ArticlesDistributionSystem.Helper;

namespace ArticlesDistributionSystem.Models
{
    interface IConnectionProviderService
    {
        SqlConnection GetConnection();
    }

    public class ConnectionProviderService : IConnectionProviderService
    {
        public SqlConnection GetConnection()
        {
            string server = "localhost\\SQLEXPRESS";
            string dbName = "adsdb";
            return SQLHelper.ConnectionUsingWindowAuth(server, dbName);
        }
    }
}