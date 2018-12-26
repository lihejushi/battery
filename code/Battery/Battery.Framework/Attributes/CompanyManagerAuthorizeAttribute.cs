using HelpYoung.Framework.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using XT.MVC.Core;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Framework;
using XT.MVC.Framework.Results;

namespace HelpYoung.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CompanyManagerAuthorizeAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        private List<string> _checkPermissions = new List<string>();
        private bool _isCheck = false;

        public bool IsCheck
        {
            get { return _isCheck; }
            set { _isCheck = value; }
        }
        public bool CompanyChecked
        {
            get; set;
        }
        public List<string> CheckPermissions
        {
            get { return _checkPermissions; }
            set { _checkPermissions = value; }
        }
        /// <summary>
        /// 企业验证
        /// </summary>
        /// <param name="isCheck">是否需要登陆</param>
        /// <param name="CompanyChecked">是否需要企业审核通过</param>
        /// <param name="permissions"></param>
        public CompanyManagerAuthorizeAttribute(bool isCheck, bool companyChecked = false, params string[] permissions)
        {
            IsCheck = isCheck;
            CompanyChecked = companyChecked;
            if (permissions != null && permissions.Length > 0) CheckPermissions = permissions.ToList();
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var appContext = (App)EngineContext.Current.Resolve<IAppContext>().CurrentApp;
            var userContext = (User)workContext.CurrentUser;
            if (workContext == null || workContext.IsLogin == false ||  /*检查是否登陆*/
                appContext.Platform != PlatformType.Company || appContext.IsAdmin != true ||    /*检查登陆平台是否正确*/
                userContext.UserType != UserType.Company /*检查用户类型是否正确*/
                )
            {
                filterContext.Controller.ViewData["CompanyState"] = -1;
                base.OnActionExecuting(filterContext);
            }
            else
            {
                var model = DAL.T.CompanyDal.GetCompanyModel(userContext.UserId);
                filterContext.Controller.ViewData["CompanyState"] =model.VerifyState;
            }
        }
        public virtual bool CheckAuthorize(AuthorizationContext filterContext)
        {

            //检查登录状态
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var appContext = (App)EngineContext.Current.Resolve<IAppContext>().CurrentApp;
            var userContext = (User)workContext.CurrentUser;


            if (workContext == null || workContext.IsLogin == false ||  /*检查是否登陆*/
                appContext.Platform != PlatformType.Company || appContext.IsAdmin != true ||    /*检查登陆平台是否正确*/
                userContext.UserType != UserType.Company /*检查用户类型是否正确*/
                ) return false;
            //检查必要权限，若没有需要检查的权限则只检查登录状态
            if (CheckPermissions == null || CheckPermissions.Count == 0 || CompanyChecked == false) return true;
            else if (CompanyChecked == true)
            {
                var company = DAL.T.CompanyDal.GetCompanyModel(userContext.UserId);
                if (company.VerifyState != 1) return false;
            }
            else
            {

            }
            return true;
        }
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (IsCheck == false)
                return;

            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                throw new InvalidOperationException("You cannot use [AdminAuthorize] attribute when a child action cache is active");

            if (IsCheck == true && CheckAuthorize(filterContext) == false)
            {
                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    Result result = new Result() { Code = -98, Message = "没有访问此接口的权限", Data = string.Empty };
                    filterContext.Result = new XTJsonResult(result, "yyyy-MM-dd HH:mm:ss");
                }
                else if (filterContext.HttpContext.Request.HttpMethod == "POST")
                {
                    filterContext.Result = new ContentResult() { Content = "没有访问此接口的权限", ContentEncoding = Encoding.UTF8 };
                }
                else
                {
                    //filterContext.Result = new RedirectResult(string.Format("~/Admin/Common/NotHasAuthorize?backurl={0}&accesstype={1}&open=self", System.Web.HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl), "Admin"));
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Common",
                        action = "NotHasAuthorize",
                        backurl = filterContext.HttpContext.Request.RawUrl,
                        area = "CompanyManager"
                    }));
                }
            }
            if (CompanyChecked == true && CheckAuthorize(filterContext) == false)
            {

                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    Result result = new Result() { Code = -98, Message = "没有访问此接口的权限", Data = string.Empty };
                    filterContext.Result = new XTJsonResult(result, "yyyy-MM-dd HH:mm:ss");
                }
                else if (filterContext.HttpContext.Request.HttpMethod == "POST")
                {
                    filterContext.Result = new ContentResult() { Content = "没有访问此接口的权限", ContentEncoding = Encoding.UTF8 };
                }
                else
                {
                    //filterContext.Result = new RedirectResult(string.Format("~/Admin/Common/NotHasAuthorize?backurl={0}&accesstype={1}&open=self", System.Web.HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl), "Admin"));
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Common",
                        action = "NotHasAuthorize",
                        backurl = filterContext.HttpContext.Request.RawUrl,
                        area = "CompanyManager"
                    }));
                }
            }
        }
    }
}
