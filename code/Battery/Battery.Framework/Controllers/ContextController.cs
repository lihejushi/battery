using Battery.Framework.Domain;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XT.MVC.Core;
using XT.MVC.Core.Common;
using XT.MVC.Core.Encrypt;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Logging;
using XT.MVC.Core.Web;
using XT.MVC.Framework.Controllers;

namespace Battery.Framework.Controllers
{
    public class ContextController : BaseController
    {
      
        #region 基础属性

        private WebWorkContext _workContext;

        public WebWorkContext WorkContext
        {
            get
            {
                if (_workContext == null)
                {
                    _workContext = (WebWorkContext)EngineContext.Current.Resolve<IWorkContext>();
                }
                return _workContext;
            }
        }
        private WebAppContext _appContext;

        public WebAppContext AppContext
        {
            get
            {
                if (_appContext == null)
                {
                    _appContext = (WebAppContext)EngineContext.Current.Resolve<IAppContext>();
                }
                return _appContext;
            }
        }

        public App CurrentApp
        {
            get
            {
                if (_appContext == null)
                {
                    _appContext = (WebAppContext)EngineContext.Current.Resolve<IAppContext>();
                }
                return (App)_appContext.CurrentApp;
            }
        }
        public User CurrentUser
        {
            get
            {
                if (_workContext == null)
                {
                    _workContext = (WebWorkContext)EngineContext.Current.Resolve<IWorkContext>();
                }
                return (User)_workContext.CurrentUser;
            }
        }
        public bool IsLogin
        {
            get
            {
                if (_workContext == null)
                {
                    _workContext = (WebWorkContext)EngineContext.Current.Resolve<IWorkContext>();
                }
                return _workContext.IsLogin;
            }
        }

        IWebHelper webHelper = EngineContext.Current.Resolve<IWebHelper>();

        public virtual string UrlReferrer
        {
            get
            {
                return GetFormParaValue("backurl") ?? webHelper.GetUrlReferrer();
            }
        }

        public bool IsTest
        {
            get
            {
                return string.IsNullOrEmpty(ConfigHelper.GetBranch("Test.Enabled")) == false && ConfigHelper.GetBranch("Test.Enabled") == "1";
            }
        }
        public string FileDomain
        {
            get
            {
                return CDNConfig.FileDomain;
            }
        }
        public string ShopDomain
        {
            get
            {
                return XT.MVC.Core.Common.ConfigHelper.GetBranch("ShopDomain");
            }
        }
        public string MainDomain
        {
            get
            {
                return XT.MVC.Core.Common.ConfigHelper.GetBranch("AppDomain");
            }
        }
        public string FilePath
        {
            get
            {
               return ConfigHelper.GetAppConfig("XT.FilePath"); ;
            }
        }

        private ILogger _logger = EngineContext.Current.Resolve<ILogger>("DefaultLogger");

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #endregion

        #region 辅助方法

        [NonAction]
        public virtual void BaseLogin(UserType identity,int shopId, int userId, string userName, string nickName,string avator, string openId)
        {
            if (identity == UserType.App && string.IsNullOrEmpty(openId)) throw new Exception("UserType.App 必须填写OpenId");
            else if (identity == UserType.App)
            {
                //userId = 0;
                //userName = string.Empty;
                //nickName = string.Empty;
            }
            else if (identity == UserType.App || identity == UserType.Sys) shopId = 0;
            var appContext = EngineContext.Current.Resolve<IAppContext>();
            //注册cookie
            string cookieKey = ConstVar.GetIdentityCookieKey(identity);
            string cookieSecretKey = ConstVar.GetSecretKeyKey(identity);
            string userJson = Newtonsoft.Json.JsonConvert.SerializeObject(new User()
            {
                NickName = nickName,
                UserId = userId,
                UserName = userName,
                UserType = identity,
                OpenId = identity == UserType.App ? openId : string.Empty,
                IsLogin = true,
                ShopId = shopId,
                Avator = avator
            });
            string _uCookie = HttpUtility.UrlEncode(DesHelper.EncryptString(cookieSecretKey, userJson));

            HttpCookie uCookie = new HttpCookie(cookieKey);
            uCookie.Value = _uCookie;
            uCookie.Expires = DateTime.Now.AddYears(1);
            //测试内容，请及时更改
            if (identity == UserType.App && IsTest == false)
            {
                uCookie.Domain = ConfigHelper.GetAppConfig("XT.BaseDomain");
            }

            HttpContext.Response.Cookies.Add(uCookie);
        }

        [NonAction]
        public virtual void BaseLoginOut()
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            if (workContext.IsLogin)
            {
                User user = (User)workContext.CurrentUser;
                string cookieKey = ConstVar.GetIdentityCookieKey(user.UserType);
                HttpCookie uCookie = HttpContext.Request.Cookies[cookieKey];
                if (uCookie != null)
                {
                    uCookie.Values.Clear();
                    uCookie.Expires = DateTime.Now.AddYears(-1);
                    if (user.UserType == UserType.App && IsTest == false)
                    {
                        uCookie.Domain = ConfigHelper.GetAppConfig("XT.BaseDomain");
                    }
                    HttpContext.Response.Cookies.Add(uCookie);
                }
                else
                {
                    EngineContext.Current.Resolve<ILogger>("DefaultLogger").Error(null, "BaseLoginOut:not find cookie");
                }
            }
        }

        [NonAction]
        public virtual string GetFormParaValue(string paraName)
        {
            if (HttpContext.Request.Form.AllKeys.Contains(paraName))
            {
                return HttpContext.Request.Form[paraName] ?? "";
            }
            else
            {
                return HttpContext.Request.QueryString[paraName] ?? "";
            }
        }

        [NonAction]
        public ActionResult Tip(string content)
        {
            return new ContentResult() { Content = "<title>跳转</title>" + content, ContentEncoding = Encoding.UTF8 };
        }

        #endregion
    }
}
