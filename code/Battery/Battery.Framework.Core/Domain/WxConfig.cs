using Battery.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Common;

namespace Battery.Framework.Core.Domain
{
    public class WxConfig
    {
        public static string AppId
        {
            get
            {
                return GlobalConfig.Get("WX.AppId");
            }
        }

        public static string SecretKey
        {
            get
            {
                return GlobalConfig.Get("WX.SecretKey");
            }
        }
        public static string Token
        {
            get
            {
                return GlobalConfig.Get("WX.Token");
            }
        }
        /// <summary>
        /// 若未开启加密验证，则无需设置
        /// </summary>
        public static string EncodingAESKey
        {
            get
            {
                return GlobalConfig.Get("WX.EncodingAESKey");
            }
        }

        public static string OAout2_State
        {
            get
            {
                return GlobalConfig.Get("WX.OAout2_State");
            }
        }

        /// <summary>
        /// 商户号
        /// </summary>
        public static string MchId
        {
            get
            {
                return GlobalConfig.Get("WX.MchId");
            }
        }

        /// <summary>
        /// 商户支付密钥
        /// </summary>
        public static string PaySecretKey
        {
            get
            {
                return GlobalConfig.Get("WX.PaySecretKey");
            }
        }

        /// <summary>
        /// 支付通知地址
        /// </summary>
        public static string NotifyUrl
        {
            get
            {
                return GlobalConfig.Get("WX.Notifyurl");
            }
        }
    }
}
