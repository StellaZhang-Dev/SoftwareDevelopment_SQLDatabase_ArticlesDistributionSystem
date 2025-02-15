using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;
using ArticlesDistributionSystem.Helper;

namespace ArticlesDistributionSystem.Models
{

    #region 模型
    [PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessage = "新密码和确认密码不匹配。")]
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("当前密码")]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("新密码")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("确认新密码")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [DisplayName("用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("密码")]
        public string Password { get; set; }

        [DisplayName("记住我?")]
        public bool RememberMe { get; set; }
    }

    [PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage = "密码和确认密码不匹配。")]
    public class RegisterModel
    {
        [Required]
        [DisplayName("用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("电子邮件地址")]
        public string Email { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("密码")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("确认密码")]
        public string ConfirmPassword { get; set; }
    }

    public class AccountInfo
    {
        public int UserID { get; set; }

        [DisplayName("用户名")]
        public string UserName { get; set; }

        [DisplayName("昵称")]
        public string NickName { get; set; }

        [DisplayName("电子邮件地址")]
        public string Email { get; set; }

        [DisplayName("联系方式")]
        public string Contact { get; set; }

        [DisplayName("个人描述")]
        public string Description { get; set; }

        [DisplayName("所属组别")]
        public string GroupName { get; set; }
    }

    #endregion

    #region Services
    // FormsAuthentication 类型是密封的且包含静态成员，因此很难对
    // 调用其成员的代码进行单元测试。下面的接口和 Helper 类演示
    // 如何围绕这种类型创建一个抽象包装，以便可以对 AccountController
    // 代码进行单元测试。

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        MembershipCreateStatus CreateUser(string userName, string password, int level);
        bool DeleteUser(int userID);
        bool ChangeUserGroup(int userID, int groupID);
        bool UpdateUserInfo(AccountInfo accountInfo); //
        bool ValidateUser(string userName, string password); 
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        int GetUserLevelByName(string userName);
        int GetUserIDByUserName(string userName);
        AccountInfo GetUserInfoByName(string userName);
        AccountInfo GetUserInfoByUserID(int userID);
        List<AccountInfo> GetAllUser();
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly IConnectionProviderService _provider;
        private readonly IGroupService _groupService;
        public AccountMembershipService()
        {
            _provider = new ConnectionProviderService();
            _groupService = new GroupService();
        }

        public int MinPasswordLength
        {
            get
            {
                return 6;
            }
        }

        public List<AccountInfo> GetAllUser()
        {
            List<AccountInfo> list = new List<AccountInfo>();
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"select UserTable.user_id, UserTable.username, 
                                        GroupTable.group_name,
                                        UserInfoTable.nickname, UserInfoTable.contact, UserInfoTable.email, UserInfoTable.description from
                                    UserTable, UserInfoTable, GroupTable where
                                    UserTable.user_id = UserInfoTable.user_id and
                                    UserTable.group_id = GroupTable.group_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    AccountInfo info = new AccountInfo();
                    info.UserID = Convert.ToInt32(reader["user_id"]);
                    info.UserName = reader["username"].ToString();
                    info.GroupName = reader["group_name"].ToString();
                    info.NickName = reader["nickname"].ToString();
                    info.Contact = reader["contact"].ToString();
                    info.Email = reader["email"].ToString();
                    info.Description = reader["description"].ToString();
                    list.Add(info);
                }
            }
            finally
            {
                connection.Close();
            }
            return list;
        }

        public bool DeleteUser(int userID)
        {
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"delete from UserTable where user_id=@user_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@user_id", userID);
                return (command.ExecuteNonQuery() > 0);
            }
            finally
            {
                connection.Close();
            }
        }

        public bool ChangeUserGroup(int userID, int groupID)
        {
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"update UserTable set group_id=@group_id where user_id=@user_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@group_id", groupID);
                command.Parameters.AddWithValue("@user_id", userID);
                return (command.ExecuteNonQuery() > 0);
            }
            finally
            {
                connection.Close();
            }
        }

        public bool UpdateUserInfo(AccountInfo accountInfo)
        {
            if (String.IsNullOrEmpty(accountInfo.UserName)) return false;

            if (accountInfo.NickName == null) accountInfo.NickName = "";
            if (accountInfo.Contact == null) accountInfo.Contact = "";
            if (accountInfo.Email == null) accountInfo.Email = "";
            if (accountInfo.Description == null) accountInfo.Description = "";

            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                //获取用户ID
                Int32 userID = GetUserIDByUserName(accountInfo.UserName);
                //查询用户信息是否存在
                bool hasUserInfo = false;
                string sql = @"select count(*) from UserInfoTable where user_id = @userid;";
                SqlParameter param = new SqlParameter("@userid", userID);
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(param);
                hasUserInfo = Convert.ToInt32(command.ExecuteScalar()) > 0;
                if (hasUserInfo)
                {
                    //update
                    sql = @"update UserInfoTable set UserInfoTable.nickname = @nickname, UserInfoTable.contact = @contact,
                                    UserInfoTable.email = @email, UserInfoTable.description = @description 
                                    where user_id = @userid;";
                }
                else
                {
                    //insert
                    sql = @"insert into UserInfoTable (user_id, nickname, contact, description, email) 
                                               values (@userid, @nickname, @contact, @description, @email);";
                    
                }
                command.CommandText = sql;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@userid", userID));
                command.Parameters.Add(new SqlParameter("@nickname", accountInfo.NickName));
                command.Parameters.Add(new SqlParameter("@contact", accountInfo.Contact));
                command.Parameters.Add(new SqlParameter("@email", accountInfo.Email));
                command.Parameters.Add(new SqlParameter("@description", accountInfo.Description));
                command.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                string message = err.Message;
            }
            finally
            {
                connection.Close();
            }
            return false;
        }

        public AccountInfo GetUserInfoByUserID(int userID)
        {
            AccountInfo accountInfo = new AccountInfo();
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"select UserTable.username, GroupTable.group_name, 
                                    UserInfoTable.nickname, UserInfoTable.contact,
                                    UserInfoTable.description, UserInfoTable.email 
                                    from UserTable, GroupTable, UserInfoTable where
                                    UserTable.user_id = UserInfoTable.user_id and 
                                    UserTable.group_id = GroupTable.group_id and 
                                    UserTable.user_id = @user_id;";
                SqlParameter userNameParam = new SqlParameter("@user_id", userID);
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(userNameParam);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    accountInfo.UserName = reader["username"].ToString();
                    accountInfo.GroupName = reader["group_name"].ToString();
                    accountInfo.NickName = reader["nickname"].ToString();
                    accountInfo.Contact = reader["contact"].ToString();
                    accountInfo.Description = reader["description"].ToString();
                    accountInfo.Email = reader["email"].ToString();
                }
            }
            finally
            {
                connection.Close();
            }

            return accountInfo;
        }

        public AccountInfo GetUserInfoByName(string userName)
        {
            AccountInfo accountInfo = new AccountInfo();
            if (String.IsNullOrEmpty(userName)) return accountInfo;
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"select UserTable.username, GroupTable.group_name, 
                                    UserInfoTable.nickname, UserInfoTable.contact,
                                    UserInfoTable.description, UserInfoTable.email 
                                    from UserTable, GroupTable, UserInfoTable where
                                    UserTable.user_id = UserInfoTable.user_id and 
                                    UserTable.group_id = GroupTable.group_id and 
                                    UserTable.username = @username;";
                SqlParameter userNameParam = new SqlParameter("@username", userName);
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(userNameParam);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    accountInfo.UserName = reader["username"].ToString();
                    accountInfo.GroupName = reader["group_name"].ToString();
                    accountInfo.NickName = reader["nickname"].ToString();
                    accountInfo.Contact = reader["contact"].ToString();
                    accountInfo.Description = reader["description"].ToString();
                    accountInfo.Email = reader["email"].ToString();
                }
            }
            finally
            {
                connection.Close();
            }

            return accountInfo;
        }

        public int GetUserLevelByName(string userName)
        {
            if (String.IsNullOrEmpty(userName)) return GroupInfo.GuestLevel;
            int groupLevel = GroupInfo.GuestLevel;
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"select GroupTable.level from UserTable,GroupTable where
                                UserTable.group_id = GroupTable.group_id and 
                                UserTable.username = @username;";
                SqlParameter userNameParam = new SqlParameter("@username", userName);
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(userNameParam);
                groupLevel = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception)
            {
            }
            finally
            {
                connection.Close();
            }
            return groupLevel;
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为 null 或为空。", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("值不能为 null 或为空。", "password");

            bool isValidate = false;
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"select count(*) from UserTable where username = @username and password = @password";
                SqlParameter userNameParam = new SqlParameter("@username", userName);
                SqlParameter passwordParam = new SqlParameter("@password", password);
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(userNameParam);
                command.Parameters.Add(passwordParam);
                isValidate = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            finally
            {
                connection.Close();
            }
            return isValidate;
        }

        public int GetUserIDByUserName(string userName)
        {
            int userID = 0;
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                //判断用户是否存在
                string sql = @"select user_id from UserTable where username = @username;";
                SqlParameter parameter = new SqlParameter("@username", userName);
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(parameter);
                userID = Convert.ToInt32(command.ExecuteScalar());
            }
            finally
            {
                connection.Close();
            }
            return userID;
        }

        private bool IsUserExists(string userName)
        {
            bool isExists = false;
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                //判断用户是否存在
                string sql = @"select count(*) from UserTable where username = @username;";
                SqlParameter parameter = new SqlParameter("@username", userName);
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(parameter);
                isExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            finally
            {
                connection.Close();
            }
            return isExists;
        }

        public MembershipCreateStatus CreateUser(string userName, string password, int level)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为 null 或为空。", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("值不能为 null 或为空。", "password");
            
            MembershipCreateStatus status = MembershipCreateStatus.Success;
            //用户是否存在
            if (this.IsUserExists(userName))
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return status;
            }

            //查询用户组
            GroupInfo groupInfo = _groupService.GetGroupInfoByGroupLevel(level);
            if (groupInfo.GroupID == GroupInfo.InvalidGroupID)
            {
                return MembershipCreateStatus.ProviderError;
            }

            //创建用户
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                //判断用户是否存在
                string sql = @"insert into UserTable (username, password, group_id) values(@username, @password, @group_id);";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@username", userName));
                command.Parameters.Add(new SqlParameter("@password", password));
                command.Parameters.Add(new SqlParameter("@group_id", groupInfo.GroupID));
                int ret = command.ExecuteNonQuery();
            }
            catch
            {
                status = MembershipCreateStatus.ProviderError;
            }
            finally
            {
                connection.Close();
            }
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为 null 或为空。", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("值不能为 null 或为空。", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("值不能为 null 或为空。", "newPassword");

            //TODO: 
            // 在某些出错情况下，基础 ChangePassword() 将引发异常，
            // 而不是返回 false。
            try
            {
                return false;
                //MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                //return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为 null 或为空。", "userName");
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
    #endregion

    #region Validation
    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // 请参见 http://go.microsoft.com/fwlink/?LinkID=177550 以查看
            // 状态代码的完整列表。
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "用户名已存在。请另输入一个用户名。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "已存在与该电子邮件地址对应的用户名。请另输入一个电子邮件地址。";

                case MembershipCreateStatus.InvalidPassword:
                    return "提供的密码无效。请输入有效的密码值。";

                case MembershipCreateStatus.InvalidEmail:
                    return "提供的电子邮件地址无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "提供的密码取回答案无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidQuestion:
                    return "提供的密码取回问题无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidUserName:
                    return "提供的用户名无效。请检查该值并重试。";

                case MembershipCreateStatus.ProviderError:
                    return "身份验证提供程序返回了错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                case MembershipCreateStatus.UserRejected:
                    return "已取消用户创建请求。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                default:
                    return "发生未知错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertiesMustMatchAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' 和 '{1}' 不匹配。";
        private readonly object _typeId = new object();

        public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
            : base(_defaultErrorMessage)
        {
            OriginalProperty = originalProperty;
            ConfirmProperty = confirmProperty;
        }

        public string ConfirmProperty { get; private set; }
        public string OriginalProperty { get; private set; }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                OriginalProperty, ConfirmProperty);
        }

        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            object originalValue = properties.Find(OriginalProperty, true /* ignoreCase */).GetValue(value);
            object confirmValue = properties.Find(ConfirmProperty, true /* ignoreCase */).GetValue(value);
            return Object.Equals(originalValue, confirmValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' 必须至少包含 {1} 个字符。";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }
    }
    #endregion

}
