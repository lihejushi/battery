using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using XT.MVC.Core.Web;

namespace XT.MVC.Core.Web
{
    public class CookieManager : ICookieManager
    {
        private readonly HttpContextBase _httpContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        public CookieManager(HttpContextBase httpContext)
        {
            this._httpContext = httpContext;
        }

        public void SetCookie(string name, string value, string domain, string path, DateTime? expires)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = value;
            if (!string.IsNullOrEmpty(domain)) cookie.Domain = domain;
            if (!string.IsNullOrEmpty(path)) cookie.Path = path;
            if (expires.HasValue) cookie.Expires = expires.Value;
            
            SendCookieToClient(cookie);
        }
        public void SetCookie(string name, string value)
        {
            SetCookie(name, value, string.Empty, string.Empty, null);
        }
        public void SetCookie(string name, string value, string domain)
        {
            SetCookie(name, value, domain, string.Empty, null);
        }
        public void SetCookie(string name, string value, DateTime expires)
        {
            SetCookie(name, value, string.Empty, string.Empty, expires);
        }
        public void SetCookie(string name, string value, string domain, DateTime expires)
        {
            SetCookie(name, value, domain, string.Empty, expires);
        }
        //根据Key获取cookie
        public string GetCookieValue(string name)
        {
            if (_httpContext.Request.Cookies != null && _httpContext.Request.Cookies[name] != null)
            {
                return _httpContext.Request.Cookies[name].Value;
            }
            return string.Empty;
        }

        //cookie删除
        public void ClearCookie(string name)
        {
            HttpCookie cookie = null;
            if (_httpContext.Request.Cookies != null && _httpContext.Request.Cookies[name] != null)
            {
                cookie = _httpContext.Request.Cookies[name];
                cookie.Expires = DateTime.Now.AddDays(-1);
                SendCookieToClient(cookie);
            }
        }
        //Cookie发送回客户端
        private void SendCookieToClient(HttpCookie cookie)
        {
            if (cookie != null)
            {
                _httpContext.Response.Cookies.Remove(cookie.Name);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }
    }
}
