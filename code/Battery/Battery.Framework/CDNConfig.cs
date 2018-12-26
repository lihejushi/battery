using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Common;

namespace Battery.Framework
{
    public static class CDNConfig
    {
        const string ConfigPath = "\\App_Data\\CDN.Config";
        public static bool IsEnabled
        {
            get { return ConfigHelper.GetAppConfig("CDN.Enabled") == "1"; }
        }

        public static string CDNUrl
        {
            get
            {
                return ConfigHelper.GetBranchValue(ConfigPath, "CDN.Url");
            }
        }

        public static string ResourcesUrl
        {
            get
            {
                return IsEnabled ? CDNUrl : MainDomain;
            }
        }

        public static string FileDomain
        {
            get
            {
                return ConfigHelper.GetAppConfig("XT.FileDomain");
            }
        }
        public static string BaseDomain
        {
            get
            {
                return ConfigHelper.GetAppConfig("XT.BaseDomain");
            }
        }
        public static string MainDomain
        {
            get
            {
                return ConfigHelper.GetBranch("AppDomain");
            }
        }

        public static string FileServerUrl
        {
            get
            {
                return ConfigHelper.GetAppConfig("XT.UploadFile.Uri");
            }
        }
    }
}
