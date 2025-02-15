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
using ArticlesDistributionSystem.Models;
using ArticlesDistributionSystem.Helper;

namespace ArticlesDistributionSystem.Models
{
    public class GroupInfo
    {
        public static int InvalidGroupID { get { return -1; } }
        public static int AdminLevel { get { return 0; } }
        public static int ExpertLevel { get { return 5; } }
        public static int AuthorLevel { get { return 10; } }
        public static int GuestLevel { get { return 20; } }
        public static int DefaultLevel { get { return GroupInfo.GuestLevel; } }

        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public int Level { get; set; }

        public GroupInfo()
        {
            this.GroupID = InvalidGroupID;
            this.GroupName = "";
            this.Level = DefaultLevel;
        }
    }

    interface IGroupService
    {
        GroupInfo AddGroupWithNameAndLevel(string name, int level);
        GroupInfo GetGroupInfoByGroupID(int groupID);
        GroupInfo GetGroupInfoByGroupLevel(int level);
    }

    public class GroupService : IGroupService
    {
        private readonly IConnectionProviderService _provider = null;

        public GroupService()
        {
            _provider = new ConnectionProviderService();
        }

        public GroupInfo AddGroupWithNameAndLevel(string name, int level)
        {
            SqlConnection connection = _provider.GetConnection();
            GroupInfo groupInfo = new GroupInfo();
            groupInfo.GroupName = name;
            groupInfo.Level = level;
            connection.Open();
            try
            {
                string sql = @"insert into GroupTable (group_name, level) values(@group_name, @level);";
                SqlParameter p_name = new SqlParameter("@group_name", groupInfo.GroupName);
                SqlParameter p_level = new SqlParameter("@level", groupInfo.Level);
                SqlParameter [] parameters = new SqlParameter[] { p_name, p_level };
                //插入数据
                SqlCommand command = new SqlCommand(sql, connection);
                for (int i = 0; i < parameters.Length; i++)
                {
                    SqlParameter parameter = parameters[i];
                    command.Parameters.Add(parameter);
                }
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                //查询ID
                command.CommandText = "select @@IDENTITY";
                int groupID = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                groupInfo.GroupID = groupID;
            }
            catch (Exception err)
            {
                string message = err.Message;
                connection.Close();
            }
            return groupInfo;
        }

        public GroupInfo GetGroupInfoByGroupLevel(int level)
        {
            SqlConnection connection = _provider.GetConnection();
            GroupInfo groupInfo = new GroupInfo();
            connection.Open();
            try
            {
                string sql = @"select group_id, group_name, level from GroupTable where level = @level;";
                SqlParameter p_groupID = new SqlParameter("@level", level);
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(p_groupID);
                //查询
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    groupInfo.GroupID = Convert.ToInt32(reader["group_id"]);
                    groupInfo.GroupName = reader["group_name"].ToString();
                    groupInfo.Level = Convert.ToInt32(reader["level"]);
                }
            }
            finally
            {
                connection.Close();
            }
            return groupInfo;
        }

        public GroupInfo GetGroupInfoByGroupID(int groupID)
        {
            SqlConnection connection = _provider.GetConnection();
            GroupInfo groupInfo = new GroupInfo();
            connection.Open();
            try
            {
                string sql = @"select group_id, group_name, level from GroupTable where group_id = @group_id;";
                SqlParameter p_groupID = new SqlParameter("@group_id", groupID);
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(p_groupID);
                //查询
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    groupInfo.GroupID = Convert.ToInt32(reader["group_id"]);
                    groupInfo.GroupName = reader["group_name"].ToString();
                    groupInfo.Level = Convert.ToInt32(reader["level"]);
                }
            }
            finally
            {
                connection.Close();
            }
            return groupInfo;
        }

        public object groupID { get; set; }
    }
}