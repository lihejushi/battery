using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Common;

namespace Battery.Framework.Domain
{
    public class WxConfig
    {
        public static string AppId
        {
            get
            {
                return ConfigHelper.GetBranch("WX.AppId");
            }
        }

        public static string SecretKey
        {
            get
            {
                return ConfigHelper.GetBranch("WX.SecretKey");
            }
        }
        public static string Token
        {
            get
            {
                return ConfigHelper.GetBranch("WX.Token");
            }
        }
        /// <summary>
        /// 若未开启加密验证，则无需设置
        /// </summary>
        public static string EncodingAESKey
        {
            get
            {
                return ConfigHelper.GetBranch("WX.EncodingAESKey");
            }
        }

        public static string OAout2_State
        {
            get
            {
                return ConfigHelper.GetBranch("WX.OAout2_State");
            }
        }

        /// <summary>
        /// 商户号
        /// </summary>
        public static string MchId
        {
            get
            {
                return ConfigHelper.GetBranch("WX.MchId");
            }
        }

        /// <summary>
        /// 商户支付密钥
        /// </summary>
        public static string PaySecretKey
        {
            get
            {
                return ConfigHelper.GetBranch("WX.PaySecretKey");
            }
        }

        /// <summary>
        /// 商户支付密钥
        /// </summary>
        public static string CertUri
        {
            get
            {
                return ConfigHelper.GetBranch("WX.CertUri");
            }
        }

        /// <summary>
        /// 支付通知地址
        /// </summary>
        public static string NotifyUrl
        {
            get
            {
                return ConfigHelper.GetBranch("WX.Notifyurl");
            }
        }
    }
}
