using Battery.DAL.Log;
using Battery.DAL.Sys;
using Battery.Framework.Attributes;
using Battery.Framework.Controllers;
using Battery.Framework.Domain;
using Battery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using XT.MVC.Core.Encrypt;
using XT.MVC.Framework;
using XT.MVC.Framework.Results;
using Battery.Model.Log;
using Battery.Framework;

namespace Battery.Areas.SysManager.Controllers
{
    public class HomeController : SysBaseController
    {
        [SysManagerAuthorize(true)]
        public ActionResult Index(PagedModel pModel, string cmd = "", string CompanyName = "")
        {
            
            return View(); 
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string userName, string password, string vCode)
        {
            Result result = new Result() { Code = -1, Message = "登录失败", Data = string.Empty };
            try
            {
                //if (Session["ValidateCode"] == null || vCode.ToLower() != Session["ValidateCode"].ToString().ToLower()) return new XTJsonResult(-5, "验证码错误");

                Sys_Person person = PermissionDAL.GetPersonByName(userName);
                if (person == null)
                    return new XTJsonResult(-2, "用户不存在");

                password = new HashHelper().HashString(password, person.Password.Substring(person.Password.Length - 12, 12));

                if (person.Password != password)
                    return new XTJsonResult(-3, "登录密码错误");
                if (person.State != 1)
                    return new XTJsonResult(-4, person.State == 0 ? "用户已禁用" : "用户已停职");

                BaseLogin(UserType.Sys, 0, person.Id, person.UserName, person.TrueName,"", string.Empty);
                return new XTJsonResult(1, "登录成功");
            }
            catch(Exception ex)
            {
                return new XTJsonResult(-1, "登录失败");
            }
        }

        public ActionResult Logout()
        {
            BaseLoginOut();
            return new RedirectToRouteResult(new RouteValueDictionary(new
            {
                controller = "Home",
                action = "Login"
            }));
        } 
        [SysManagerAuthorize(true)]
        public ActionResult EditPwd()
        {
            if (Request.HttpMethod == "POST")
            {
                string oldPwd = GetFormParaValue("OldPassword");
                string newPwd = GetFormParaValue("NewPassword");
                string pwd2 = GetFormParaValue("Password2");
                if (string.IsNullOrEmpty(oldPwd)) return new XTJsonResult(-1, "请输入旧密码");
                if (string.IsNullOrEmpty(newPwd)) return new XTJsonResult(-1, "请输入新密码");
                if (newPwd.Trim() != pwd2.Trim()) return new XTJsonResult(-1, "请重复输入新密码");


                Sys_Person person = PermissionDAL.GetPersonModel(CurrentUser.UserId);
                if (person != null)
                {
                    oldPwd = new HashHelper().HashString(oldPwd, person.Password.Substring(person.Password.Length - 12, 12));

                    if (person.Password != oldPwd)
                        return new XTJsonResult(-1, "旧密码不正确");
                    else
                    {
                        int result = PermissionDAL.EditPersonPW(CurrentUser.UserId, new HashHelper().HashString(newPwd));
                        return new XTJsonResult(result, result != 1 ? "执行错误" : "");
                    }
                }
                return new XTJsonResult(-1, "旧密码不正确");
            }
            return View();
        }
    }
}
