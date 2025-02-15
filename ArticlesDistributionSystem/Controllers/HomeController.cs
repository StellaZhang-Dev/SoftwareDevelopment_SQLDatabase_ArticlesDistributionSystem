using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArticlesDistributionSystem.ActionFilters;

namespace ArticlesDistributionSystem.Controllers
{
    //[HandleError]
    public class HomeController : Controller
    {
        //[UserFilter]
        public ActionResult Index()
        {
            ViewData["Message"] = "欢迎使用 ASP.NET MVC!";

            return View("~/Views/Home/Index.aspx");
        }

    }
}
