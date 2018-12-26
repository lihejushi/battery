using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using XT.MVC.Core.Common;

namespace XT.MVC.Core.Web
{
    public class SessionManager : ISessionManager
    {
        
        private readonly HttpContextBase _httpContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        public SessionManager(HttpContextBase httpContext)
        {
            this._httpContext = httpContext;
        }


        #region ISessionHelper 成员

        public T GetSession<T>(string name)
        {
            object value = GetSession(name);
            if (value == null) return default(T);
            return TypeHelper.To<T>(value);
        }

        public object GetSession(string name)
        {
            return _httpContext.Session[name];
        }

        public void SetSession(string name, object obj)
        {
            DelSession(name);
            AddSession(name, obj);
        }

        public void AddSession(string name, object obj)
        {
            _httpContext.Session.Add(name, obj);
        }

        public void DelSession(string name)
        {
            _httpContext.Session.Remove(name);
        }

        #endregion
    }
}
