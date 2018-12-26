using Battery.Framework.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using XT.MVC.Core;
using XT.MVC.Core.Common;
using XT.MVC.Core.Domain.Apps;
using XT.MVC.Core.Web;

namespace Battery.Framework
{
    public class WebAppContext : IAppContext
    {
        static string AdminDomainRegex = @"^http://192\.168\.0\.1/sysmanager(/|$)|^http://192\.168\.0\.1/(\d+)/manager(/|$)|^http://192\.168\.0\.1/company(/|$)";//判断域名是否是后台管理地址
        static string SysMRegex = @"^http://192\.168\.0\.1/sysmanager(/|$)";//判断域名是否是系统管理后台
        static string CompanyMRegex = @"^http://192\.168\.0\.1/company(/|$)";//判断域名是否是系统管理后台

        App _app = null;
        private readonly IWebHelper _webHelper;
        private readonly HttpContextBase _httpContext;
        public WebAppContext(HttpContextBase httpContext, IWebHelper webHelper)
        {
            this._webHelper = webHelper;
            this._httpContext = httpContext;
            AdminDomainRegex = ConfigHelper.GetBranch("TK.AdminDomainRegex");
            SysMRegex = ConfigHelper.GetBranch("TK.SysMRegex");
            CompanyMRegex = ConfigHelper.GetBranch("TK.CompanyMRegex");

#if DEBUG
            this._app = ParseWebHost();
#elif RELEASE
            this._app = ParseWebHost();
#else
            this._app = ParseWebHost();
#endif
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

            //判断是否是企业
            if (Regex.Match(segment, CompanyMRegex, RegexOptions.IgnoreCase).Success)
            {
                app.Platform = PlatformType.Company;
            }
            else
            {
                app.AppId = 0;
                app.Platform = PlatformType.ChinaTelcom;
            }

            if (app.AppId == 0)
            {
                app.AppName = ConfigHelper.GetBranch("XT.AppName");
            }
            else
            {

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
