using Battery.DAL.Shop;
using Battery.Framework.Core.Domain;
using Battery.Service;
using System;
using System.Text.RegularExpressions;
using System.Web;
using XT.MVC.Core;
using XT.MVC.Core.Common;
using XT.MVC.Core.Domain.Apps;
using XT.MVC.Core.Web;

namespace Battery.Framework.Core
{
    public class WebAppContext : IAppContext
    {
        static string AdminDomainRegex = @"^http://192\.168\.0\.1/sysmanager(/|$)|^http://192\.168\.0\.1/(\d+)/manager(/|$)";//判断域名是否是后台管理地址
        static string SysMRegex = @"^http://192\.168\.0\.1/sysmanager(/|$)";//判断域名是否是系统管理后台
        static string ShopMRegex = @"^http://192\.168\.0\.1/(\d+)/manager(/|$)";//判断域名是否是营业厅管理后台地址
        static string ShopVRegex = @"^http://192\.168\.0\.1/(\d+)(/|$)";//判断域名是否营业厅地址
        static string ZytMRegex = @"^http://192\.168\.0\.1/(\d+)/manager(/|$)";//判断域名是否是营业厅管理后台地址
        static string ZytVRegex = @"^http://192\.168\.0\.1/(\d+)(/|$)";//判断域名是否营业厅地址

        App _app = null;
        private readonly IWebHelper _webHelper;
        private readonly HttpContextBase _httpContext;
        public WebAppContext(HttpContextBase httpContext, IWebHelper webHelper)
        {
            this._webHelper = webHelper;
            this._httpContext = httpContext;
            AdminDomainRegex = GlobalConfig.Get("MP.AdminDomainRegex");
            SysMRegex = GlobalConfig.Get("MP.SysMRegex");
            ShopMRegex = GlobalConfig.Get("MP.ShopMRegex");
            ShopVRegex = GlobalConfig.Get("MP.ShopVRegex");
            ZytMRegex = GlobalConfig.Get("MP.ZytMRegex");
            ZytVRegex = GlobalConfig.Get("MP.ZytVRegex");

            this._app = ParseWebHost();
        }

        private App ParseWebHost()
        {
            App app = new App()
            {
                Platform = PlatformType.ChinaTelcom,
                AppId = 0
            };
            string segment = this._httpContext.Request.Url.AbsoluteUri;
            segment = Regex.Replace(segment, @"(:(\d)+)", "");
            //判断是否是管理页面
            if (Regex.IsMatch(segment, AdminDomainRegex, RegexOptions.IgnoreCase)) app.IsAdmin = true;
            //如果是管理页面并且包含测试id，则默认测试id
            if (GlobalConfig.Get("Test.AppID") != "" && app.IsAdmin == true)
            {
                app.AppId = Convert.ToInt32(GlobalConfig.Get("Test.AppID"));
                app.Platform = PlatformType.Shop;
            }
            else                                  
            {
                //判断是否是营业厅地址，不是营业地址默认为系统地址
                Match match = null;
                if ((match = Regex.Match(segment, ShopVRegex, RegexOptions.IgnoreCase)) != null && match.Success)
                {
                    app.AppId = Convert.ToInt32(match.Groups[1].Value);
                    app.Platform = PlatformType.Shop;
                }
                //else if ((match = Regex.Match(segment, ZytVRegex, RegexOptions.IgnoreCase)) != null && match.Success)
                //{
                //    app.AppId = Convert.ToInt32(match.Groups[1].Value);
                //    app.Platform = PlatformType.Zyt;
                //}
                else
                {
                    app.AppId = 0;
                    app.Platform = PlatformType.ChinaTelcom;
                }
            }
            
            
            if (app.AppId == 0)
            {
                app.AppName = GlobalConfig.Get("XT.AppName");
            }
            else
            {
                var info = ShopDAL.GetShopModel(app.AppId);

                app.AppName = info != null ? info.ShopName : string.Empty;
            }

            return app;
        }

        public virtual IApp CurrentApp
        {
            get { return _app; }
        }

        public void Refresh(IApp app)
        {
            if (app == null)
            {
               this._app = ParseWebHost();
            }
            else
            {
                _app = (App)app;
            }
        }
    }
}
