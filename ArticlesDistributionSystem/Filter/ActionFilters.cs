using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using ArticlesDistributionSystem.Models;

namespace ArticlesDistributionSystem.ActionFilters
{
    public class UserFilter : ActionFilterAttribute
    {
        private readonly IMembershipService _membershipService;
        public UserFilter()
        {
            _membershipService = new AccountMembershipService();
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //登录状态拦截
            //判断是否有session
            bool isLogin = Convert.ToBoolean(filterContext.HttpContext.Session["isLogin"]);
            if (isLogin)
            {
                //已经登录过
            }
            else
            {
                //没有session，判断cookie，是否有记住登录，有则恢复session状态
                if (filterContext.HttpContext.Request.IsAuthenticated)
                {
                    isLogin = true;
                    //设置session
                    filterContext.HttpContext.Session["isLogin"] = isLogin;
                    string loginUser = filterContext.HttpContext.User.Identity.Name;
                    int userLevel = _membershipService.GetUserLevelByName(loginUser);
                    filterContext.HttpContext.Session["Level"] = userLevel;
                }
                else
                {
                    //清理session
                    filterContext.HttpContext.Session.Clear();
                }
            }
            //
            if (!isLogin)
            {
                //没有登录，重定向到登录页面
                filterContext.Result = new RedirectResult("/Account/LogOn");
            }
        }
    }
}