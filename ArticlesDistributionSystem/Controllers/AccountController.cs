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

    [HandleError]
    public class AccountController : Controller
    {

        private IFormsAuthenticationService FormsService { get; set; }
        private IMembershipService MembershipService { get; set; }
        private IGroupService groupService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }
            if (groupService == null) { groupService = new GroupService(); }
            base.Initialize(requestContext);
        }

        public ActionResult Info()
        {
            string userName = User.Identity.Name;
            AccountInfo accountInfo = MembershipService.GetUserInfoByName(userName);
            return View(accountInfo);
        }
        [HttpPost]
        public ActionResult Info(AccountInfo model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                MembershipService.UpdateUserInfo(model);
            }
            return View(model);
        }

        public ActionResult Manager()
        {
            List<AccountInfo> allUserInfo = MembershipService.GetAllUser();
            ViewData["AllUserInfo"] = allUserInfo;
            return View();
        }

        [HttpPost]
        public ActionResult Manager(RegisterModel model)
        {
            int level = Convert.ToInt32(Request.Form["level"]);
            if (level == GroupInfo.AdminLevel ||
                level == GroupInfo.ExpertLevel) {
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, level);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    AccountInfo accountInfo = new AccountInfo();
                    accountInfo.UserName = model.UserName;
                    accountInfo.NickName = model.UserName;
                    accountInfo.Email = model.Email;
                    MembershipService.UpdateUserInfo(accountInfo);
                }
            }
            return RedirectToAction("Manager", "Account");
        }

        public ActionResult Delete()
        {
            int userID = Convert.ToInt32(Request.Params["user_id"]);
            if (userID > 0)
            {
                MembershipService.DeleteUser(userID);
            }
            return RedirectToAction("Manager", "Account");
        }

        public ActionResult ChangeGroup()
        {
            int userID = Convert.ToInt32(Request.Params["user_id"]);
            if (userID > 0)
            {
                int level = Convert.ToInt32(Request.Params["level"]);
                if (level == GroupInfo.AdminLevel ||
                    level == GroupInfo.ExpertLevel)
                {
                    int groupID = groupService.GetGroupInfoByGroupLevel(level).GroupID;
                    MembershipService.ChangeUserGroup(userID, groupID);
                }
            }
            return RedirectToAction("Manager", "Account");
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "提供的用户名或密码不正确。");
                }
            }

            // 如进行到此时出错，则重新显示表单
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // 尝试注册用户
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, GroupInfo.AuthorLevel);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    AccountInfo accountInfo = new AccountInfo();
                    accountInfo.UserName = model.UserName;
                    accountInfo.NickName = model.UserName;
                    accountInfo.Email = model.Email;
                    MembershipService.UpdateUserInfo(accountInfo);

                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // 如进行到此时出错，则重新显示表单
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "当前密码不正确或新密码无效。");
                }
            }

            // 如进行到此时出错，则重新显示表单
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

    }
}
