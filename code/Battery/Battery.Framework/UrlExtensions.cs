using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using XT.MVC.Core.Common;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Web;

namespace Battery.Framework
{
    public static class UrlExtensions
    {
        /// <summary>
        /// 获取地址
        /// </summary>
        /// <param name="appId">0 时，默认返回主站地址</param>
        /// <returns></returns>
        public static string GetDomain(this UrlHelper helper, int appId)
        {
            if (appId != 0) return string.Format(ConfigHelper.GetBranch("ShopDomain"), appId);

            return ConfigHelper.GetBranch("AppDomain");
        }
        /// <summary>
        /// 获取商家登录地址
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static string GetShopLoginUrl(this UrlHelper helper, int appId)
        {
            if (appId != 0) return string.Format(ConfigHelper.GetBranch("ShopLoginUrl"), appId);
            return ConfigHelper.GetBranch("AppDomain");
        }
    }
}
