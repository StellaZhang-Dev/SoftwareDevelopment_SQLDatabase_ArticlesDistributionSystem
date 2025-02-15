using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ArticlesDistributionSystem.Models;

namespace ArticlesDistributionSystem.Controllers
{
    public class InstallController : Controller
    {
        //
        // GET: /Install/

        public IWebSiteInitializeService FormsService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new ADSWebSiteInitializeService(); }
            base.Initialize(requestContext);
        }

        public ActionResult Index()
        {
            return RedirectToAction("Config", "Install");
        }

        public ActionResult Config()
        {
            WebSiteConfigModel model = new WebSiteConfigModel();
            model.DatabaseServer = "localhost\\SQLEXPRESS";
            model.Username = "sa";
            model.Password = "";
            model.DatabaseName = "adsdb";
            model.WindowsAuth = true;
            model.SQLSERVERAuth = false;
            return View(model);
        }

        [HttpPost]
        public ActionResult Config(WebSiteConfigModel model)
        {
            bool succeeded = false;
            if (model.WindowsAuth)
            {
                succeeded = FormsService.ValidateDBServerInWindowsAuth(model.DatabaseServer, model.DatabaseName);
            }
            else
            {
                succeeded = FormsService.ValidateDBServer(model.DatabaseServer, model.Username, model.Password, model.DatabaseName);
            }

            if (succeeded)
            {
                if (model.WindowsAuth)
                {
                    succeeded = FormsService.InitializeDBServerInWindowsAuth(model.DatabaseServer, model.DatabaseName);
                }
                else
                {
                    succeeded = FormsService.InitializeDBServer(model.DatabaseServer, model.Username, model.Password, model.DatabaseName);
                }
            }
            
            if (succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", FormsService.ConnectionErrorInfo());
            }

            if (model.WindowsAuth)
            {
                ViewData["IsWindowsAuth"] = true;
            }
            else
            {
                ViewData["IsWindowsAuth"] = false;
            }
            return View(model);
        }

    }
}
