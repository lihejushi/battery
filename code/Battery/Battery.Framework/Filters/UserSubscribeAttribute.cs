using TuoKe.DAL.WX;
using TuoKe.Framework.Domain;
using TuoKe.Framework.Tools;
using TuoKe.Model.WX;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using XT.MVC.Core;
using XT.MVC.Core.Common;
using XT.MVC.Core.Encrypt;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Logging;
using XT.MVC.Framework.Results;

namespace TuoKe.Framework.Filters
{
    /// <summary>
    /// 预先处理数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class UserSubscribeAttribute : ActionFilterAttribute
    {
        public UserSubscribeAttribute()
        {
            this.Order = 20;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                throw new InvalidOperationException("You cannot use [AppAuthorize] attribute when a child action cache is active");
            if (filterContext.IsChildAction == false)
            {
                WX_SubscribeUsers user = null;
                if (CheckAuthorize(filterContext, out user) == false)
                {
                    //跳转到关注页面
                    if (string.IsNullOrEmpty(filterContext.RequestContext.HttpContext.Request.Headers["X-PJAX"]) == false)
                    {
                        filterContext.Result = new ContentResult()
                        {
                            Content = "<title>跳转</title><script>location.href='" + UrlHelper.GenerateContentUrl("~/Home/CTPoster?open=self&backurl=" + HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl) + "&tip=1", filterContext.HttpContext) + "'</script>您还没有关注河南电信微信服务号！",
                            ContentEncoding = Encoding.UTF8
                        };
                    }
                    else if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    {
                        XT.MVC.Framework.Result result = new XT.MVC.Framework.Result() { Code = -98, Message = "您还没有关注河南电信微信服务号", Data = string.Empty };
                        filterContext.Result = new XTJsonResult(result, "yyyy-MM-dd HH:mm:ss");
                    }
                    else if (filterContext.HttpContext.Request.HttpMethod == "POST")
                    {
                        filterContext.Result = new ContentResult() { Content = "您还没有关注河南电信微信服务号", ContentEncoding = Encoding.UTF8 };
                    }
                    else
                    {
                        //跳转到授权页面
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = "Home",
                            action = "CTPoster",
                            open = "self",
                            backurl = filterContext.HttpContext.Request.RawUrl,
                            tip = 1
                        }));
                    }
                }
                else
                {
                    filterContext.Controller.ViewData[ConstVar.WX_ViewBag_UserInfo] = user;
                }
            }
            base.OnActionExecuting(filterContext);
        }
        private bool CheckAuthorize(ActionExecutingContext filterContext, out WX_SubscribeUsers userInfo)
        {
            userInfo = null;
            //检查登录状态
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var appContext = (App)EngineContext.Current.Resolve<IAppContext>().CurrentApp;
            var userContext = (User)workContext.CurrentUser;

            if (userContext.IsLogin == false) return false;
            userInfo = filterContext.Controller.ViewData[ConstVar.WX_ViewBag_UserInfo] == null ? UserDAL.GetSubscribeUser(userContext.OpenId) : ((WX_SubscribeUsers)filterContext.Controller.ViewData[ConstVar.WX_ViewBag_UserInfo]);
            return userInfo != null && userInfo.IsUnSubscribe == 0;
        }
    }
}
