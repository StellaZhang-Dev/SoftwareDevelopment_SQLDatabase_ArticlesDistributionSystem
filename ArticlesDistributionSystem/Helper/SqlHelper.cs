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

namespace ArticlesDistributionSystem.Helper
{
    public class SQLHelper
    {
        public static string ConnectionStringUsingWindowAuth(string server, string dbName)
        {
            //windows验证方式
            string connectionString = "Data Source=" + server + ";Initial Catalog=" + dbName + ";Integrated Security=SSPI;";
            return connectionString;
        }
        public static string ConnectionStringUsingSQLServerAuth(string server, string user, string pwd, string dbName)
        {
            //SQLSERVER验证方式
            string connectionString = "Data Source=" + server + ";Initial Catalog=" + dbName + ";User ID=" + user + ";Password=" + pwd;
            return connectionString;
        }
        public static SqlConnection ConnectionUsingWindowAuth(string server, string dbName)
        {
            string connectionString = SQLHelper.ConnectionStringUsingWindowAuth(server, dbName);
            return new SqlConnection(connectionString);
        }
        public static SqlConnection ConnectionUsingSQLServerAuth(string server, string user, string pwd, string dbName)
        {
            string connectionString = SQLHelper.ConnectionStringUsingSQLServerAuth(server, user, pwd, dbName);
            return new SqlConnection(connectionString);
        }

        public static int ExecuteSQLWithConnection(SqlConnection connection, string sql)
        {
            SqlCommand command = new SqlCommand(sql, connection);
            return command.ExecuteNonQuery();
        }

        public static int ExecuteSQLWithConnectionAndParameter(SqlConnection connection, string sql, SqlParameter [] parameters)
        {
            SqlCommand command = new SqlCommand(sql, connection);
            for (int i = 0; i < parameters.Length; i++)
            {
                SqlParameter parameter = parameters[i];
                command.Parameters.Add(parameter);
            }
            return command.ExecuteNonQuery();
        }

        public static int ExecuteSQLWithConnectionAutomaticOpenAndClose(SqlConnection connection, string sql)
        {
            connection.Open();
            int ret = SQLHelper.ExecuteSQLWithConnection(connection, sql);
            connection.Close();
            return ret;
        }

        public static bool IsTableExistWithConnection(SqlConnection connection, string tableName)
        {
            string sql = "select count(*) from sysobjects where type='u' and [name]=@tablename;";
            SqlParameter parameter = new SqlParameter("@tablename", tableName);
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add(parameter);
            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }
    }
}