using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XT.MVC.Core;
using XT.MVC.Core.Caching;
using XT.MVC.Core.Common;
using XT.MVC.Core.Logging;
using XT.MVC.Core.Web;

namespace Battery.Framework.Tools
{
    public class NavModel
    {
        public int AppId { get; set; }
        public string NavImg { get; set; }
        public string NavContentType { get; set; }
        public string NavContentTypeTitle { get; set; }
        public string NavContentValue { get; set; }
        public string NavLinkType { get; set; }
        public string NavLinkTypeTitle { get; set; }
        public string NavLink { get; set; }
        public string NavLinkTitle { get; set; }
        public string NavValue { get; set; }
        public string NavValueTitle { get; set; }
        public string NavUrl { get; set; }
    }
    public class AppUrlHelper
    {
        public static string GetUrl(string navValue)
        {
            try
            {
                string appDomain = ConfigHelper.GetBranch("AppDomain");
                string shopDomain = ConfigHelper.GetBranch("ShopDomain");

                NavModel nav = JsonConvert.DeserializeObject<NavModel>(navValue);
                if (nav.NavContentType == "ZDY") return nav.NavContentValue;
                else if (nav.NavContentType == "APP")
                {
                    if (nav.NavLinkType == "Nav")
                    {
                        #region APP 导航地址解析
                        return appDomain + (nav.NavUrl ?? "");
                        #endregion
                    }
                    else
                    {
                        #region APP 内容地址解析
                        return string.Format(appDomain + (nav.NavUrl ?? ""), nav.NavValue);
                        #endregion
                    }
                }
                else if (nav.NavContentType == "SHOP")
                {
                    shopDomain = string.Format(shopDomain, nav.NavContentValue);
                    if (nav.NavLinkType == "Nav")
                    {
                        #region 店铺 导航地址解析
                        return string.Format(shopDomain + (nav.NavUrl ?? ""), nav.NavValue);
                        #endregion
                    }
                    else
                    {
                        #region 店铺 内容地址解析
                        return string.Format(shopDomain + (nav.NavUrl ?? ""), nav.NavValue);
                        #endregion
                    }
                    return shopDomain;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}