using Battery.Framework.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Framework.Core
{
    public static class ConstVar
    {
        public const string CookieSecretKey = "e8aeb3ae-0d3d-4d49-8b7f-3a7ae0daea0f";
        public const string CookieKeyPrefix = "ChinaTelcom.";

        public const string WX_ViewBag_UserInfo = "WX.UserInfo";
        public const string WX_ViewBag_OAuth2 = "WX.OAuth2";

        /// <summary>
        /// 获取身份标示
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static UserType GetIdentity(string identity)
        {
            switch (identity)
            {
                case "Sys":
                    return UserType.Sys;
                case "Shop":
                    return UserType.Shop;
                case "User":
                    return UserType.App;
                case "Zyt":
                    return UserType.Zyt;
                default:
                    return UserType.App;
            }
        }

        /// <summary>
        /// 身份标示的cookie key
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetIdentityCookieKey(UserType identity)
        {
            return CookieKeyPrefix + identity.ToString();
        }
        public static string GetIdentityCookieKey(string identity)
        {
            return CookieKeyPrefix + GetIdentity(identity).ToString();
        }

        public static string GetSecretKeyKey(UserType identity)
        {
            return identity.ToString() + CookieSecretKey;
        }

        //public const string RegAgreement_CacheKey = "XT.RegAgreement";
    }
}
