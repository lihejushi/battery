﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using XT.MVC.Core.Common;

namespace TuoKe.Framework.Filters
{
    /// <summary>
    /// 过滤来自非微信客户端浏览器的请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class WeixinInternalRequestAttribute : ActionFilterAttribute
    {
        private string _message;
        private string _ignoreParameter;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">错误提示信息</param>
        /// <param name="ignoreParameter">如果地址栏中提供改参数，则忽略浏览器判断，建议设置得复杂一些。如?abc=[任意字符]</param>
        public WeixinInternalRequestAttribute(string message,string ignoreParameter=null)
        {
            _message = message;
            _ignoreParameter = ignoreParameter;

            this.Order = 0;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (string.IsNullOrEmpty(_ignoreParameter) || string.IsNullOrEmpty(filterContext.RequestContext.HttpContext.Request.QueryString[_ignoreParameter]))
            //{
            //    var userAgent = filterContext.HttpContext.Request.UserAgent;
            //    if (string.IsNullOrEmpty(userAgent) || (!userAgent.Contains("MicroMessenger") && !userAgent.Contains("Windows Phone")))
            //    {
            //        //TODO:判断网页版登陆状态
            //        filterContext.Result = new ContentResult()
            //        {
            //            Content = _message
            //        };
            //    }
            //}
            if (ConfigHelper.GetBranch("Platform.Update") == "1")
            {
                if (DateTime.Now >= Convert.ToDateTime(ConfigHelper.GetBranch("Platform.Update.BeginTime")) && DateTime.Now <= Convert.ToDateTime(ConfigHelper.GetBranch("Platform.Update.EndTime")))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Other",
                        action = "Update",
                        backurl = filterContext.HttpContext.Request.RawUrl,
                        tip = 1
                    }));
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
