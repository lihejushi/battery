using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Common;

namespace Battery.Framework
{
    public static class DianDianConfig
    {
        const string ConfigPath = "\\App_Data\\DianDian.Config"; 
        //资源编号
        public static string No
        {
            get
            {
                return ConfigHelper.GetBranchValue(ConfigPath, "DianDian.No");
            }
        }
        //请求地址
        public static string DianSendMsgUrl
        {
            get
            {
                return ConfigHelper.GetBranchValue(ConfigPath,"DianSendMsgUrl");
            }
        }
        public static string partnerNo
        {
            get
            {
                return ConfigHelper.GetBranchValue(ConfigPath,"partnerNo");
            }
        }
        public static string apiVersion
        {
            get
            {
                return ConfigHelper.GetBranchValue(ConfigPath,"apiVersion");
            }
        }
        public static string channel
        {
            get
            {
                return ConfigHelper.GetBranchValue(ConfigPath,"channel");
            }
        }
        public static string channelCode
        {
            get
            {
                return ConfigHelper.GetBranchValue(ConfigPath,"channelCode");
            }
        }
        public static string campaignNo
        {
            get
            {
                return ConfigHelper.GetBranchValue(ConfigPath,"campaignNo");
            }
        }
        public static string registerPlatform
        {
            get
            {
                return ConfigHelper.GetBranchValue(ConfigPath,"registerPlatform");
            }
        }
        public static string DianPublicKey
        {
            get
            {
                return ConfigHelper.GetBranchValue(ConfigPath,"DianPublicKey");
            }
        }
    }
}
