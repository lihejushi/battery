using Battery.Framework.Core.AccessType;
using Battery.Framework.Core.Domain;
using System;
using System.Web;
using XT.MVC.Core;
using XT.MVC.Core.Domain.Users;
using XT.MVC.Core.Encrypt;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Logging;
using XT.MVC.Core.Web;

namespace Battery.Framework.Core
{
    public class WebWorkContext : IWorkContext
    {
        private User user = null;

        private readonly HttpContextBase _httpContext;
        private readonly IAppContext _appContext;
        private readonly IAccessTypeHelper _accessTypeHelper;
        private readonly ICookieManager _cookieHelper;

        private bool _isLogin = false;
        private UserType _uType = UserType.App;
        private string _uCookie = string.Empty;

        public WebWorkContext(HttpContextBase httpContext, IAppContext appContext, ICookieManager cookieHelper, IAccessTypeHelper accessTypeHelper)
        {
            this._httpContext = httpContext;
            this._appContext = appContext;
            this._accessTypeHelper = accessTypeHelper;
            this._cookieHelper = cookieHelper;
            ParseWork();
        }

        #region IWorkContext 成员

        public bool IsLogin
        {
            get { return _isLogin; }
        }

        public IUser CurrentUser
        {
            get { return user; }
        }

        public void Refresh(IUser _user)
        {
            if (_user == null)
            {
                ParseWork();
            }
            else
            {
                user = (User)_user;
                _isLogin = ((User)_user).IsLogin;
            }
        }

        

        #endregion

        private void DecryptCookie()
        {

            string salt = _appContext.CurrentApp.AppId.ToString();
            string cookieSecretKey = ConstVar.GetSecretKeyKey(_uType);

            try
            {
                if (string.IsNullOrEmpty(_uCookie) == false)
                {
                    user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(DesHelper.DecryptString(cookieSecretKey, _httpContext.Server.UrlDecode(_uCookie)));
                    _isLogin = user.IsLogin;
                    return;
                }
            }
            catch (Exception ex)
            {
                EngineContext.Current.Resolve<ILogger>("DefaultLogger").Error(ex, @"用户信息解密失败
失败原因：{0}
                ", ex.Message);
                //解密失败
            }
            _isLogin = false;
            user = new User()
            {
                NickName = string.Empty,
                UserId = 0,
                UserName = string.Empty,
                UserType = UserType.App,
                IsLogin = false,
                OpenId = string.Empty,
                ShopId = 0
            };
        }

        private void ParseWork()
        {
            App app = (App)_appContext.CurrentApp;
            if (app.IsAdmin)
            {
                //如果是管理员平台，则判断管理员账号
                if (app.Platform == PlatformType.ChinaTelcom)
                {
                    _uCookie = _cookieHelper.GetCookieValue(ConstVar.GetIdentityCookieKey(UserType.Sys));
                    _uType = UserType.Sys;
                }
                else if (app.Platform == PlatformType.Shop)
                {
                    _uCookie = _cookieHelper.GetCookieValue(ConstVar.GetIdentityCookieKey(UserType.Shop));
                    _uType = UserType.Shop;
                }
            }

            if (string.IsNullOrEmpty(_uCookie))
            {
                //非管理员账号，即为app用户
                _uCookie = _cookieHelper.GetCookieValue(ConstVar.GetIdentityCookieKey(UserType.App));
                _uType = UserType.App;
            }

            DecryptCookie();
        }

        //private Dictionary<string, object> userInfos = new Dictionary<string, object>();

        //public T GetUserInfo<T>(int userId)
        //{
        //    string key = typeof(T).ToString() + "." + userId.ToString();

        //    return default(T);
        //}
    }
}
