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
    public enum PublishState
    {
        PublishState_UnPublish = 0, //未发表
        PublishState_Judging = 1,   //评审中
        PublishState_Accept = 2,    //通过
        PublishState_Refuse = 3,    //不通过
    }
    public class ContentModel
    {
        public static int InvalidContentID = -1;
        public int ArticleID { get; set; }

        [Required]
        [DisplayName("标题")]
        public string Title { get; set; }

        [Required]
        [DisplayName("内容")]
        public string Content { get; set; }

        public DateTime PostTime { get; set; }
        public int AuthorID { get; set; }
        public string UniqueID { get; set; }
        public PublishState ArticlePublishState { get; set; }
    }

    public class ArticleInfo
    {
        public int ArticleID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PostTime { get; set; }
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public string UniqueID { get; set; }
    }

    public interface IContentService
    {
        int CreateContent(int author_id, string title, string content);
        bool UpdateContent(ContentModel content);
        bool DeleteContent(int articleID);
        ContentModel GetContentByArticleID(int articleID);
        List<ContentModel> GetAllContentsByUserID(int UserID);
    }

    public class MyContentService : IContentService
    {
        private readonly IConnectionProviderService _provider;
        private readonly IGroupService _groupService;
        private readonly IMembershipService _accountService;
        private readonly IPublishService _publishService;

        public MyContentService()
        {
            _provider = new ConnectionProviderService();
            _groupService = new GroupService();
            _accountService = new AccountMembershipService();
            _publishService = new MyArticlePublishService(this);
        }

        public bool DeleteContent(int articleID)
        {
            bool result = false;
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"delete from ArticleTable where article_id=@article_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@article_id", articleID));
                result = (command.ExecuteNonQuery() > 0);
                if (result)
                {
                    _publishService.DeletePublishRecordByArticleID(articleID);
                }
                return result;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool UpdateContent(ContentModel content)
        {
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                content.PostTime = DateTime.Now; //更新时间
                string sql = @"update ArticleTable set title = @title,
                                                        content = @content,
                                                        post_time = @post_time
                                                    where article_id = @article_id and 
                                                          author_id = @author_id and 
                                                            article_unique_id = @article_unique_id;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@title", content.Title));
                command.Parameters.Add(new SqlParameter("@content", content.Content));
                command.Parameters.Add(new SqlParameter("@post_time", content.PostTime));
                command.Parameters.Add(new SqlParameter("@article_id", content.ArticleID));
                command.Parameters.Add(new SqlParameter("@author_id", content.AuthorID));
                command.Parameters.Add(new SqlParameter("@article_unique_id", content.UniqueID));
                return command.ExecuteNonQuery() > 0;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<ContentModel> GetAllContentsByUserID(int UserID)
        {
            List<ContentModel> contentList = new List<ContentModel>();
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                string sql = @"select ArticleTable.article_id, 
                                      ArticleTable.title, 
                                      ArticleTable.content, 
                                      ArticleTable.author_id,
                                      ArticleTable.post_time,
                                      ArticleTable.article_unique_id from ArticleTable where author_id = @userid order by post_time desc;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@userid", UserID));
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    ContentModel content = new ContentModel();
                    content.ArticleID = Convert.ToInt32(reader["article_id"]);
                    content.Title = reader["title"].ToString();
                    content.Content = reader["content"].ToString();
                    content.AuthorID = Convert.ToInt32(reader["author_id"]);
                    content.UniqueID = reader["article_unique_id"].ToString();
                    content.PostTime = Convert.ToDateTime(reader["post_time"].ToString());
                    contentList.Add(content);
                }
            }
            finally
            {
                connection.Close();
            }
            //获取发布状态
            for (int i = 0; i < contentList.Count; i++)
            {
                ContentModel content = contentList.ElementAt(i);
                content.ArticlePublishState = _publishService.GetArticlePublishState(content.ArticleID);
            }
            return contentList;
        }

        public ContentModel GetContentByArticleID(int articleID)
        {
            SqlConnection connection = _provider.GetConnection();
            ContentModel content = new ContentModel();
            content.ArticleID = ContentModel.InvalidContentID;
            connection.Open();
            try
            {
                string sql = @"select article_id, title, content, author_id, post_time, article_unique_id from
                               ArticleTable where article_id=@articleID;";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@articleID", articleID));
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    content.ArticleID = Convert.ToInt32(reader["article_id"]);
                    content.Title = reader["title"].ToString();
                    content.Content = reader["content"].ToString();
                    content.AuthorID = Convert.ToInt32(reader["author_id"]);
                    content.UniqueID = reader["article_unique_id"].ToString();
                    content.PostTime = Convert.ToDateTime(reader["post_time"].ToString());
                }
            }
            finally
            {
                connection.Close();
            }
            //
            content.ArticlePublishState = _publishService.GetArticlePublishState(content.ArticleID);
            return content;
        }

        private string CreateTimeStamp()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            DateTime nowTime = DateTime.Now;
            long unixTime = (long)Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
            return unixTime.ToString();
        }

        private string CreateRandNumber()
        {
            Random ra = new Random(DateTime.Now.Millisecond);
            long num = ra.Next(10000);
            return String.Format("{0:D4}", num);
        }

        public int CreateContent(int author_id, string title, string content)
        {
            SqlConnection connection = _provider.GetConnection();
            connection.Open();
            try
            {
                DateTime postTime = DateTime.Now;
                if (title == null || title.Length == 0)
                {
                    title = "无标题";
                }
                if (content == null || content.Length == 0)
                {
                    content = "";
                }
                //string article_unique_id = System.Guid.NewGuid().ToString();
                string article_unique_id = CreateTimeStamp() + CreateRandNumber();
                string sql = @"insert into ArticleTable (title, content, author_id, post_time, article_unique_id) 
                                                 values (@title, @content, @author_id, @post_time, @article_unique_id);";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@title", title));
                command.Parameters.Add(new SqlParameter("@content", content));
                command.Parameters.Add(new SqlParameter("@author_id", author_id));
                command.Parameters.Add(new SqlParameter("@post_time", postTime));
                command.Parameters.Add(new SqlParameter("@article_unique_id", article_unique_id));
                int rowID = command.ExecuteNonQuery();
                if (rowID > 0) 
                {
                    //查询articleID
                    command.Parameters.Clear();
                    command.CommandText = "select @@IDENTITY";
                    int articleID = Convert.ToInt32(command.ExecuteScalar());
                    return articleID;
                } 
                else 
                {
                    return ContentModel.InvalidContentID;
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}