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
using ArticlesDistributionSystem.Models;

namespace ArticlesDistributionSystem.Models
{

    #region 模型
    public class WebSiteConfigModel
    {
        [Required]
        [DataType(DataType.Text)]
        [DisplayName("数据库地址")]
        public string DatabaseServer { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("帐号")]
        public string Username { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("密码")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("数据库名")]
        public string DatabaseName { get; set; }

        [DisplayName("Windows验证方式")]
        public bool WindowsAuth { get; set; }

        [DisplayName("SQLSERVER验证方式")]
        public bool SQLSERVERAuth { get; set; }
    }
    
    #endregion

    #region Services

    public interface IWebSiteInitializeService
    {
        bool ValidateDBServerInWindowsAuth(string server, string dbName);
        bool ValidateDBServer(string server, string username, string password, string dbName);
        bool InitializeDBServerInWindowsAuth(string server, string dbName);
        bool InitializeDBServer(string server, string username, string password, string dbName);
        string ConnectionErrorInfo();
    }

    public class ADSWebSiteInitializeService : IWebSiteInitializeService
    {
        private string error { get; set; }
        public ADSWebSiteInitializeService()
        {
        }

        public string ConnectionErrorInfo()
        {
            return this.error;
        }

        private bool ValidateDBServerWithConnectionString(string connectionString)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch (Exception err)
            {
                this.error = err.Message;
                return false;
            }
        }

        public bool ValidateDBServerInWindowsAuth(string server, string dbName)
        {
            string connectionString = SQLHelper.ConnectionStringUsingWindowAuth(server, dbName);
            return ValidateDBServerWithConnectionString(connectionString);
        }
        public bool ValidateDBServer(string server, string username, string password, string dbName)
        {
            string connectionString = SQLHelper.ConnectionStringUsingSQLServerAuth(server, username, password, dbName);
            return ValidateDBServerWithConnectionString(connectionString);
        }
        public bool InitializeDBServerWithConnection(SqlConnection connection)
        {
            try
            {
                connection.Open();
                //是否第一次创建
                bool needInitData = !SQLHelper.IsTableExistWithConnection(connection, "GroupTable");
                //
                InitGroupTableWithConnection(connection);
                InitUserTableWithConnection(connection);
                InitUserInfoTableWithConnection(connection);
                InitArticleTypeTableWithConnection(connection);
                InitArticleTableWithConnection(connection);
                InitPublishTableWithConnection(connection);
                //数据库表创建完成
                connection.Close();
                if (needInitData)
                {
                    InitGroupAndUserData();
                }
                return true;
            }
            catch (Exception err)
            {
                this.error = err.Message;
                return false;
            }
        }

        public bool InitializeDBServerInWindowsAuth(string server, string dbName)
        {
            SqlConnection connection = SQLHelper.ConnectionUsingWindowAuth(server, dbName);
            return InitializeDBServerWithConnection(connection);
        }

        public bool InitializeDBServer(string server, string user, string pwd, string dbName)
        {
            SqlConnection connection = SQLHelper.ConnectionUsingSQLServerAuth(server, user, pwd, dbName);
            return InitializeDBServerWithConnection(connection);
        }

        //初始化数据
        private void InitGroupAndUserData()
        {
            //初始化数据
            IGroupService groupService = new GroupService();
            groupService.AddGroupWithNameAndLevel("管理员", GroupInfo.AdminLevel);
            groupService.AddGroupWithNameAndLevel("评审专家", GroupInfo.ExpertLevel);
            groupService.AddGroupWithNameAndLevel("作家", GroupInfo.AuthorLevel);
            groupService.AddGroupWithNameAndLevel("访客", GroupInfo.GuestLevel);
            //创建管理员
            IMembershipService membershipService = new AccountMembershipService();
            membershipService.CreateUser("admin", "123456", GroupInfo.AdminLevel);
            AccountInfo accountInfo = new AccountInfo();
            accountInfo.UserName = "admin";
            accountInfo.NickName = "鹳狸猿";
            membershipService.UpdateUserInfo(accountInfo);
        }

        // 创建数据库表
        private void InitGroupTableWithConnection(SqlConnection connection)
        {
            if (SQLHelper.IsTableExistWithConnection(connection, "GroupTable")) return;
            string sql = @"create table GroupTable(
                            group_id int identity(1,1) primary key, 
                            group_name varchar(64) not null, 
                            level int not null);";
            int result = SQLHelper.ExecuteSQLWithConnection(connection, sql);
        }

        private void InitUserTableWithConnection(SqlConnection connection)
        {
            if (SQLHelper.IsTableExistWithConnection(connection, "UserTable")) return;
            string sql = @"create table UserTable(
                            user_id int identity(1,1) primary key, 
                            username varchar(64) not null, 
                            password varchar(128) not null, 
                            group_id int not null foreign key references GroupTable(group_id) on delete cascade);";
            int result = SQLHelper.ExecuteSQLWithConnection(connection, sql);
        }

        private void InitUserInfoTableWithConnection(SqlConnection connection)
        {
            if (SQLHelper.IsTableExistWithConnection(connection, "UserInfoTable")) return;
            string sql = @"create table UserInfoTable(
                            userinfo_id int identity(1,1) primary key, 
                            user_id int not null foreign key references UserTable(user_id) on delete cascade, 
                            nickname varchar(64),
                            contact varchar(64),
                            description varchar(500),
                            email varchar(128));";
            int result = SQLHelper.ExecuteSQLWithConnection(connection, sql);
        }

        private void InitArticleTypeTableWithConnection(SqlConnection connection)
        {
            if (SQLHelper.IsTableExistWithConnection(connection, "ArticleTypeTable")) return;
            string sql = @"create table ArticleTypeTable(
                            article_type_id int identity(1,1) primary key, 
                            type_name varchar(64) not null,
                            type_unique_id varchar(64) not null);";
            int result = SQLHelper.ExecuteSQLWithConnection(connection, sql);
        }

        private void InitArticleTableWithConnection(SqlConnection connection)
        {
            if (SQLHelper.IsTableExistWithConnection(connection, "ArticleTable")) return;
            string sql = @"create table ArticleTable(
                            article_id int identity(1,1) primary key, 
                            title varchar(200) not null,
                            content ntext,
                            author_id int not null 
                            foreign key references UserTable(user_id) on delete cascade,
                            post_time datetime,
                            article_unique_id varchar(200) not null);";
            int result = SQLHelper.ExecuteSQLWithConnection(connection, sql);
        }

        private void InitPublishTableWithConnection(SqlConnection connection)
        {
            if (SQLHelper.IsTableExistWithConnection(connection, "PublishStatusTable")) return;
            string sql = @"create table PublishStatusTable(
                            publish_status_id int identity(1,1) primary key, 
                            article_id int,
                            article_type int,
                            officer_id int,
                            publish_status int,
                            publish_time datetime,
                            confirm_time datetime,
                            article_description varchar(500),
                            reply_message varchar(500));";
            int result = SQLHelper.ExecuteSQLWithConnection(connection, sql);
        }
    }

    #endregion
}
