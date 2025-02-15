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
    public class ArticleType
    {
        public const int InvalidTypeID = -1;
        public int TypeID { get; set; }

        [Required]
        [DisplayName("作品类别")]
        [DataType(DataType.Text)]
        public string TypeName { get; set; }

        public string TypeUniqueID { get; set; }
    }

    public class PublishModel
    {
        public int ArticleID { get; set; }
        public string ArticleTitle { get; set; }
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public List<ArticleType> ArticleTypeList { get; set; }
        public string ArticleDescription { get; set; }
    }

    public class ArticleJudgingInfo
    {
        public int PublishID { get; set; }
        public int ArticleID { get; set; }
        public int AuthorID { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleContent { get; set; }
        public string ArticleUniqueID { get; set; }
        public string ArticleDescription { get; set; }
        public int ArticleTypeID { get; set; }
        public string ArticleTypeName { get; set; }
        public string ArticleTypeUniqueID { get; set; }
        public string AuthorName { get; set; }
        public int ExpertID { get; set; }
        public string ExpertName { get; set; }
        public string ReplyMessage { get; set; }
        public DateTime PublishTime { get; set; }
        public DateTime ConfirmTime { get; set; }
    }

    interface IArticleTypeService
    {
        int CreateArticleType(string typeName);
        bool DeleteArticleType(int typeID);
        bool UpdateArticleType(int typeID, string typeName);
        ArticleType GetArticleTypeByTypeID(int typeID);
        List<ArticleType> GetAllArticleType();
    }

    interface IPublishService
    {
        bool PublishArticle(int articleID, int typeID, string article_description);
        bool DeletePublishRecordByArticleID(int articleID);
        bool ExpertReply(int publishID, int expertID, string replyMessage, bool pass);
        PublishState GetArticlePublishState(int articleID);
        PublishModel GetArticlePublishInfo(int articleID);
        ArticleJudgingInfo GetPublishJudgingInfo(int articleID);
        List<ArticleJudgingInfo> GetJudgingArticleList();
        List<ArticleJudgingInfo> GetJudgedArticleList(int expertUserID);
    }

    public class MyArticlePublishService : IPublishService
    {
        private readonly IConnectionProviderService _provider;
        private readonly IGroupService _groupService;
        private readonly IMembershipService _accountService;
        private readonly IContentService _contentService;
        private readonly IArticleTypeService _articleTypeService;

        public MyArticlePublishService(IContentService contentService)
        {
            _provider = new ConnectionProviderService();
            _groupService = new GroupService();
            _accountService = new AccountMembershipService();
            _articleTypeService = new MyArticleTypeService();
            _contentService = contentService;
        }

        public bool DeletePublishRecordByArticleID(int articleID)
        {
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"delete from PublishStatusTable where article_id=@article_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@article_id", articleID);
                return (command.ExecuteNonQuery() > 0);
            }
            finally
            {
                connection.Close();
            }
        }

        public bool ExpertReply(int publishID, int expertID, string replyMessage, bool pass)
        {
            DateTime confirmTime = DateTime.Now;
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                if (replyMessage == null) replyMessage = "";
                PublishState publishStatus = (pass) ? PublishState.PublishState_Accept : PublishState.PublishState_Refuse;
                string sql = @"update PublishStatusTable set officer_id=@officer_id, publish_status=@publish_status, 
                                    confirm_time=@confirm_time, reply_message=@reply_message where
                                    publish_status_id=@publish_status_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@officer_id", expertID);
                command.Parameters.AddWithValue("@publish_status", publishStatus);
                command.Parameters.AddWithValue("@confirm_time", confirmTime);
                command.Parameters.AddWithValue("@reply_message", replyMessage);
                command.Parameters.AddWithValue("@publish_status_id", publishID);
                return (command.ExecuteNonQuery() > 0);
            }
            finally
            {
                connection.Close();
            }
        }

        public List<ArticleJudgingInfo> GetJudgedArticleList(int expertUserID)
        {
            List<ArticleJudgingInfo> list = new List<ArticleJudgingInfo>();
            List<int> judgingIdList = new List<int>();
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"select article_id from PublishStatusTable where 
                                (publish_status=@publish_status1 or publish_status=@publish_status2) and 
                                officer_id=@officer_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@publish_status1", PublishState.PublishState_Accept);
                command.Parameters.AddWithValue("@publish_status2", PublishState.PublishState_Refuse);
                command.Parameters.AddWithValue("@officer_id", expertUserID);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int articleID = Convert.ToInt32(reader["article_id"]);
                    judgingIdList.Add(articleID);
                }
            }
            finally
            {
                connection.Close();
            }
            //
            for (int i = 0; i < judgingIdList.Count; i++)
            {
                int articleID = judgingIdList[i];
                list.Add(GetPublishJudgingInfo(articleID));
            }
            return list;
        }

        public List<ArticleJudgingInfo> GetJudgingArticleList()
        {
            List<ArticleJudgingInfo> list = new List<ArticleJudgingInfo>();
            List<int> judgingIdList = new List<int>();
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"select article_id from PublishStatusTable where publish_status=@publish_status;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@publish_status", PublishState.PublishState_Judging);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int articleID = Convert.ToInt32(reader["article_id"]);
                    judgingIdList.Add(articleID);
                }
            }
            finally
            {
                connection.Close();
            }
            //
            for (int i = 0; i < judgingIdList.Count; i++)
            {
                int articleID = judgingIdList[i];
                list.Add(GetPublishJudgingInfo(articleID));
            }
            return list;
        }

        public ArticleJudgingInfo GetPublishJudgingInfo(int articleID)
        {
            ArticleJudgingInfo judgingInfo = new ArticleJudgingInfo();
            //
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"select publish_status_id, article_id, officer_id, article_type, 
                                        article_description, reply_message, confirm_time, publish_time 
                                        from PublishStatusTable where article_id=@article_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@article_id", articleID);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    judgingInfo.PublishID = Convert.ToInt32(reader["publish_status_id"]);
                    if (reader.IsDBNull(2))
                    {
                        judgingInfo.ExpertID = -1;
                    }
                    else
                    {
                        judgingInfo.ExpertID = Convert.ToInt32(reader["officer_id"]);
                    }
                    if (!reader.IsDBNull(6))
                    {
                        judgingInfo.ConfirmTime = Convert.ToDateTime(reader["confirm_time"].ToString());
                    }
                    judgingInfo.PublishTime = Convert.ToDateTime(reader["publish_time"].ToString());
                    judgingInfo.ArticleID = Convert.ToInt32(reader["article_id"]);
                    judgingInfo.ArticleTypeID = Convert.ToInt32(reader["article_type"]);
                    judgingInfo.ArticleDescription = reader["article_description"].ToString();
                    judgingInfo.ReplyMessage = reader["reply_message"].ToString();
                }
            }
            finally
            {
                connection.Close();
            }
            //
            //获取文章信息
            ContentModel contentModel = _contentService.GetContentByArticleID(judgingInfo.ArticleID);
            judgingInfo.ArticleTitle = contentModel.Title;
            judgingInfo.ArticleUniqueID = contentModel.UniqueID;
            //获取作者信息
            judgingInfo.AuthorID = contentModel.AuthorID;
            judgingInfo.AuthorName = _accountService.GetUserInfoByUserID(contentModel.AuthorID).UserName;
            //审批专家
            judgingInfo.ExpertName = _accountService.GetUserInfoByUserID(judgingInfo.ExpertID).UserName;
            //作品类别
            ArticleType articleType = _articleTypeService.GetArticleTypeByTypeID(judgingInfo.ArticleTypeID);
            judgingInfo.ArticleTypeName = articleType.TypeName;
            judgingInfo.ArticleTypeUniqueID = articleType.TypeUniqueID;
            return judgingInfo;
        }

        public PublishModel GetArticlePublishInfo(int articleID)
        {
            PublishModel publishModel = new PublishModel();
            //获取文章信息
            ContentModel contentModel = _contentService.GetContentByArticleID(articleID);
            publishModel.ArticleID = contentModel.ArticleID;
            publishModel.ArticleTitle = contentModel.Title;
            //获取作者信息
            publishModel.AuthorID = contentModel.AuthorID;
            publishModel.AuthorName = _accountService.GetUserInfoByUserID(contentModel.AuthorID).UserName;
            //作品类型列表
            publishModel.ArticleTypeList = _articleTypeService.GetAllArticleType();
            return publishModel;
        }

        public bool PublishArticle(int articleID, int typeID, string article_description)
        {
            PublishState publishState = GetArticlePublishState(articleID);
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                if (article_description == null ||
                    article_description.Length == 0) article_description = "没有作品简介";
                string sql = "";
                SqlCommand command;
                DateTime publishTime = DateTime.Now;
                if (publishState == PublishState.PublishState_UnPublish)
                {
                    //未发布
                    sql = @"insert into PublishStatusTable (article_id, article_type, publish_status, publish_time, article_description) 
                                values(@article_id, @article_type, @publish_status, @publish_time, @article_description);";
                }
                else
                {
                    //已经发布过，再次发布
                    sql = @"update PublishStatusTable set article_type=@article_type, publish_status=@publish_status, 
                                    publish_time=@publish_time, confirm_time=null, officer_id=null, 
                                    reply_message=null, article_description=@article_description where
                                    article_id=@article_id;";
                }
                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@article_id", articleID);
                command.Parameters.AddWithValue("@article_type", typeID);
                command.Parameters.AddWithValue("@publish_status", PublishState.PublishState_Judging);
                command.Parameters.AddWithValue("@publish_time", publishTime);
                command.Parameters.AddWithValue("@article_description", article_description);
                return (command.ExecuteNonQuery() > 0);
            }
            finally
            {
                connection.Close();
            }
        }

        public PublishState GetArticlePublishState(int articleID)
        {
            PublishState publishState = PublishState.PublishState_UnPublish;
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"select publish_status from PublishStatusTable where article_id=@article_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@article_id", articleID);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    publishState = (PublishState)Convert.ToInt32(reader["publish_status"]);
                }
                return publishState;
            }
            finally
            {
                connection.Close();
            }
        }
    }

    public class MyArticleTypeService : IArticleTypeService
    {
        private readonly IConnectionProviderService _provider;
        private readonly IGroupService _groupService;
        private readonly IMembershipService _accountService;

        public MyArticleTypeService()
        {
            _provider = new ConnectionProviderService();
            _groupService = new GroupService();
            _accountService = new AccountMembershipService();
        }

        public ArticleType GetArticleTypeByTypeID(int typeID)
        {
            ArticleType type = new ArticleType();
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"select article_type_id,type_name,type_unique_id from 
                                ArticleTypeTable where article_type_id=@article_type_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@article_type_id", typeID);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    type.TypeID = Convert.ToInt32(reader["article_type_id"]);
                    type.TypeName = reader["type_name"].ToString();
                    type.TypeUniqueID = reader["type_unique_id"].ToString();
                }
            }
            finally
            {
                connection.Close();
            }
            return type;
        }

        public bool UpdateArticleType(int typeID, string typeName)
        {
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"update ArticleTypeTable set type_name=@type_name where article_type_id=@article_type_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@article_type_id", typeID));
                command.Parameters.Add(new SqlParameter("@type_name", typeName));
                return (command.ExecuteNonQuery() > 0);
            }
            finally
            {
                connection.Close();
            }
        }

        public List<ArticleType> GetAllArticleType()
        {
            List<ArticleType> list = new List<ArticleType>();
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"select article_type_id,type_name,type_unique_id from ArticleTypeTable";
                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ArticleType type = new ArticleType();
                    type.TypeID = Convert.ToInt32(reader["article_type_id"]);
                    type.TypeName = reader["type_name"].ToString();
                    type.TypeUniqueID = reader["type_unique_id"].ToString();
                    list.Add(type);
                }
            }
            finally
            {
                connection.Close();
            }
            return list;
        }

        public bool DeleteArticleType(int typeID)
        {
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"delete from ArticleTypeTable where article_type_id=@article_type_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@article_type_id", typeID));
                return (command.ExecuteNonQuery() > 0);
            }
            finally
            {
                connection.Close();
            }
        }

        private string CreateTypeUniqueID()
        {
            string typeUniqueID = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
            Random ra = new Random(DateTime.Now.Millisecond);
            typeUniqueID = typeUniqueID + String.Format("{0:D4}", ra.Next(10000));
            return typeUniqueID;
        }

        public int CreateArticleType(string typeName)
        {
            if (String.IsNullOrEmpty(typeName)) return ArticleType.InvalidTypeID;
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string type_unique_id = CreateTypeUniqueID();
                string sql = @"insert into ArticleTypeTable (type_name, type_unique_id) 
                                                 values (@type_name, @type_unique_id);";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@type_name", typeName));
                command.Parameters.Add(new SqlParameter("@type_unique_id", type_unique_id));
                int rowID = command.ExecuteNonQuery();
                if (rowID > 0)
                {
                    command.Parameters.Clear();
                    command.CommandText = "select @@IDENTITY";
                    int typeID = Convert.ToInt32(command.ExecuteScalar());
                    return typeID;
                }
                else
                {
                    return ArticleType.InvalidTypeID;
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}