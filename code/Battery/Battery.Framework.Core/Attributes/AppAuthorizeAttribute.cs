using ChinaTelcom_MP.Framework.Domain;
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

namespace ChinaTelcom_MP.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AppAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        public virtual bool CheckAuthorize(AuthorizationContext filterContext)
        {
            //检查登录状态
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var appContext = (App)EngineContext.Current.Resolve<IAppContext>().CurrentApp;
            var userContext = (User)workContext.CurrentUser;


            if (workContext == null || workContext.IsLogin == false ||  /*检查是否登陆*/
                appContext.IsAdmin != false  /*检查登陆平台是否正确*/
                ) return false;
            return true;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                throw new InvalidOperationException("You cannot use [AppAuthorize] attribute when a child action cache is active");

            if (CheckAuthorize(filterContext) == false)
            {
                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    Result result = new Result() { Code = -98, Message = "没有访问此接口的权限", Data = string.Empty };
                    filterContext.Result = new XTJsonResult(-98, "", string.Empty, "yyyy-MM-dd HH:mm:ss");
                }
                else if (filterContext.HttpContext.Request.HttpMethod == "POST")
                {
                    filterContext.Result = new ContentResult() { Content = "没有访问此接口的权限", ContentEncoding = Encoding.UTF8 };
                }
                else
                {
                    //跳转到授权页面
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Users",
                        action = "Login",
                        open = "self",
                        backurl = filterContext.HttpContext.Request.RawUrl,
                    }));
                }
            }
        }
    }
}
