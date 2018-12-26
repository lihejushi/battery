using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Support.File.Code.Config
{
    public class FileServiceManage
    {
        static Dictionary<string, string> FileServiceDic = new Dictionary<string, string>();
        static Dictionary<string, IFileServiceConfig> ServiceConfig = new Dictionary<string, IFileServiceConfig>();
        static FileServiceManage()
        {
            FileServiceDic = (Dictionary<string, string>)ConfigurationSettings.GetConfig("ServiceConfig");
            InstantiationFileService();
        }

        /// <summary>
        /// 初始化服务列表
        /// </summary>
        private static void InstantiationFileService()
        {
            try
            {
                foreach (var item in FileServiceDic)
                {
                    string AssemblyString = item.Value.Split(',')[1];
                    string type = item.Value.Split(',')[0];
                    IFileServiceConfig v = (IFileServiceConfig)Assembly.Load(AssemblyString).CreateInstance(type, false);
                    ServiceConfig.Add(item.Key.ToLower(), v);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="name">文件服务名称</param>
        /// <param name="defaultService">默认服务</param>
        /// <returns></returns>
        public static IFileServiceConfig GetService(string name, IFileServiceConfig defaultService)
        {
            if (ServiceConfig.ContainsKey(name.ToLower()))
            {
                return ServiceConfig[name.ToLower()];
            }
            else
            {
                return defaultService;
            }
        }
    }
}