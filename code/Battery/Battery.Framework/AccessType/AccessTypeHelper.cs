using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using XT.MVC.Core.Web;
using Battery.Framework.AccessType;

namespace Battery.Framework.AccessType
{
    public class AccessTypeHelper : IAccessTypeHelper
    {
        private ICookieManager _cookieHelper;
        private HttpContextBase _httpContext;
        public AccessTypeHelper(HttpContextBase httpContext, ICookieManager cookieHelper)
        {
            this._httpContext = httpContext;
            this._cookieHelper = cookieHelper;
        }

        public bool IsMobile()
        {
            //既不是App，也不是微信，默认为浏览器访问
            return !IsApp() && !IsWeiXin();
            //return _httpContext.Request.Browser.IsMobileDevice;
        }

        public bool IsApp()
        {
            //传参数
            if (_httpContext.Request.QueryString["accesstype"] != null && _httpContext.Request.QueryString["accesstype"] == "app")
            {
                return true;
            }
            else
            {
                //cookie存在
                var cookie = _cookieHelper.GetCookieValue("XT.AccessType");
                if (string.IsNullOrEmpty(cookie) == false && cookie.ToLower() == "app")
                    return true;
            }
            return false;
        }

        public bool IsWeiXin()
        {
            if (_httpContext.Request.UserAgent.Contains("MicroMessenger"))
            {
                return true;
            }
            return false;
        }

        public bool IsShare()
        {
            if (_httpContext.Request.QueryString["client"] != null && _httpContext.Request.QueryString["client"] != "")
            {
                return true;
            }
            ////传参数
            //if (_httpContext.Request.QueryString["accesstype"] != null && _httpContext.Request.QueryString["accesstype"] == "app")
            //{
            //    return true;
            //}
            //else
            //{
            //    //cookie存在
            //    var cookie = _httpContext.Request.Cookies["XT.AccessType"];
            //    if (cookie != null && cookie.Value == "app")
            //        return true;
            //}
            return false;
        }

        public bool IsAndroid()
        {
            string ua = _httpContext.Request.UserAgent;
            if (string.IsNullOrEmpty(ua) == false)
            {
                return Regex.IsMatch(ua, "android", RegexOptions.IgnoreCase);
            }
            return false;
        }
        public bool IsIOS()
        {
            string ua = _httpContext.Request.UserAgent;
            if (string.IsNullOrEmpty(ua) == false)
            {
                return Regex.IsMatch(ua, "iphone|ipad|ipod", RegexOptions.IgnoreCase);
            }
            return false;
        }
    }
}
