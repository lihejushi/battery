using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Common;

namespace Battery.Framework.Domain
{
    public class ComWxConfig
    { 
        public static string AppId
        {
            get
            {
                return ConfigHelper.GetBranchValue("\\App_Data\\CompanyPay.Config", "WX.AppId");
            }
        }
        public static string Domin
        {
            get
            {
                return ConfigHelper.GetBranchValue("\\App_Data\\CompanyPay.Config", "Domin");
            }
        }
        
        public static string SecretKey
        {
            get
            {
                return ConfigHelper.GetBranchValue("\\App_Data\\CompanyPay.Config","WX.SecretKey");
            }
        }
        public static string Token
        {
            get
            {
                return ConfigHelper.GetBranchValue("\\App_Data\\CompanyPay.Config","WX.Token");
            }
        }
        /// <summary>
        /// 若未开启加密验证，则无需设置
        /// </summary>
        public static string EncodingAESKey
        {
            get
            {
                return ConfigHelper.GetBranchValue("\\App_Data\\CompanyPay.Config","WX.EncodingAESKey");
            }
        }

        public static string OAout2_State
        {
            get
            {
                return ConfigHelper.GetBranchValue("\\App_Data\\CompanyPay.Config","WX.OAout2_State");
            }
        }

        /// <summary>
        /// 商户号
        /// </summary>
        public static string MchId
        {
            get
            {
                return ConfigHelper.GetBranchValue("\\App_Data\\CompanyPay.Config","WX.MchId");
            }
        }

        /// <summary>
        /// 商户支付密钥
        /// </summary>
        public static string PaySecretKey
        {
            get
            {
                return ConfigHelper.GetBranchValue("\\App_Data\\CompanyPay.Config","WX.PaySecretKey");
            }
        }

        /// <summary>
        /// 商户支付密钥
        /// </summary>
        public static string CertUri
        {
            get
            {
                return ConfigHelper.GetBranchValue("\\App_Data\\CompanyPay.Config","WX.CertUri");
            }
        } 
    }
}
