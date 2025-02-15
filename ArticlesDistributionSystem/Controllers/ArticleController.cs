using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ArticlesDistributionSystem.Models;

namespace ArticlesDistributionSystem.Controllers
{
    public class ArticleController : Controller
    {
        private IContentService contentManager { get; set; }
        private IMembershipService accountManager { get; set; }
        private IArticleTypeService typeManager { get; set; }
        private IPublishService publishManager { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            this.contentManager = new MyContentService();
            this.accountManager = new AccountMembershipService();
            this.typeManager = new MyArticleTypeService();
            this.publishManager = new MyArticlePublishService(this.contentManager);
            base.Initialize(requestContext);
        }
        //
        // GET: /Article/

        private int GetCurrentUserID()
        {
            return this.accountManager.GetUserIDByUserName(User.Identity.Name);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyArticle()
        {
            int userID = GetCurrentUserID();
            List<ContentModel> contentList = this.contentManager.GetAllContentsByUserID(userID);
            ViewData["AllContent"] = contentList;
            return View();
        }

        public ActionResult Publish()
        {
            int articleID = Convert.ToInt32(Request.Params["article_id"]);
            if (articleID > 0)
            {
                PublishModel publishModel = publishManager.GetArticlePublishInfo(articleID);
                return View(publishModel);
            }
            else
            {
                return RedirectToAction("MyArticle", "Article");
            }
        }

        [HttpPost]
        public ActionResult Publish(PublishModel publishModel)
        {
            int articleID = publishModel.ArticleID;
            int authorID = publishModel.AuthorID;
            int typeID = Convert.ToInt32(Request.Form["article_type"]);
            if (typeID > 0 &&
                articleID > 0 &&
                authorID > 0)
            {
                publishManager.PublishArticle(articleID, typeID, publishModel.ArticleDescription);
            }
            return RedirectToAction("MyArticle", "Article");
        }

        public ActionResult Review()
        {
            List<ArticleJudgingInfo> list = publishManager.GetJudgingArticleList();
            ViewData["JudgingArticleList"] = list;
            int userID = GetCurrentUserID();
            List<ArticleJudgingInfo> list2 = publishManager.GetJudgedArticleList(userID);
            ViewData["JudgedArticleList"] = list2;
            return View();
        }

        public ActionResult ReviewDetail()
        {
            int articleID = Convert.ToInt32(Request.Params["article_id"]);
            if (articleID > 0)
            {
                ArticleJudgingInfo judgeItem = publishManager.GetPublishJudgingInfo(articleID);
                return View(judgeItem);
            }
            else
            {
                return RedirectToAction("Review", "Article");
            }
        }
        [HttpPost]
        public ActionResult ReviewDetail(ArticleJudgingInfo judgingItem)
        {
            int publishID = judgingItem.PublishID;
            string replyMessage = judgingItem.ReplyMessage;
            bool pass = (Convert.ToInt32(Request.Form["result"]) == 1);
            if (publishID > 0)
            {
                publishManager.ExpertReply(publishID, GetCurrentUserID(), replyMessage, pass);
            }
            return RedirectToAction("Review", "Article");
        }

        public ActionResult ReviewDetailPreview()
        {
            int articleID = Convert.ToInt32(Request.Params["article_id"]);
            if (articleID > 0)
            {
                ArticleJudgingInfo judgeItem = publishManager.GetPublishJudgingInfo(articleID);
                return View(judgeItem);
            }
            else
            {
                return RedirectToAction("Review", "Article");
            }
        }

        public ActionResult Delete()
        {
            int articleID = Convert.ToInt32(Request.Params["article_id"]);
            if (articleID > 0)
            {
                contentManager.DeleteContent(articleID);
                return RedirectToAction("MyArticle", "Article");
            }
            else
            {
                return RedirectToAction("MyArticle", "Article");
            }
        }

        public ActionResult DelType()
        {
            int typeID = Convert.ToInt32(Request.Params["type_id"]);
            typeManager.DeleteArticleType(typeID);
            return RedirectToAction("Type", "Article");
        }

        public ActionResult Type()
        {
            List<ArticleType> allTypes = typeManager.GetAllArticleType();
            ViewData["AllTypes"] = allTypes;
            return View();
        }

        [HttpPost]
        public ActionResult Type(ArticleType model)
        {
            typeManager.CreateArticleType(model.TypeName);
            return RedirectToAction("Type", "Article");
        }

        public ActionResult Write()
        {
            int articleID = Convert.ToInt32(Request.Params["article_id"]);
            if (articleID > 0)
            {
                //编辑
                ContentModel data = this.contentManager.GetContentByArticleID(articleID);
                return View(data);
            }
            else
            {
                //新建
                return View();
            }
        }

        public ActionResult Preview()
        {
            //文章预览
            int articleID = Convert.ToInt32(Request.Params["article_id"]);
            ContentModel data = this.contentManager.GetContentByArticleID(articleID);
            if (data.ArticlePublishState == PublishState.PublishState_Refuse ||
                data.ArticlePublishState == PublishState.PublishState_Accept)
            {
                ViewData["JudgeInfo"] = this.publishManager.GetPublishJudgingInfo(articleID);
            }
            return View(data);
        }

        [HttpPost]
        public ActionResult Write(ContentModel model)
        {
            if (model.ArticleID > 0)
            {
                //编辑
                int articleID = model.ArticleID;
                if (this.contentManager.UpdateContent(model))
                {
                    return Redirect("/Article/Preview/?article_id=" + articleID.ToString());
                }
                else
                {
                    return View(model);
                }
            }
            else
            {
                //新建
                int articleID = this.contentManager.CreateContent(GetCurrentUserID(), model.Title, model.Content);
                if (articleID != ContentModel.InvalidContentID)
                {
                    return Redirect("/Article/Preview/?article_id=" + articleID.ToString());
                }
                else
                {
                    return View(model);
                }
            }
        }
    }
}
