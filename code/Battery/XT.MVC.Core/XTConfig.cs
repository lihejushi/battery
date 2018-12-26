using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;

namespace XT.MVC.Core
{
    public partial class XTConfig : IConfigurationSectionHandler
    {
        /// <summary>
        /// 创建一个配置节处理程序。
        /// </summary>
        /// <param name="parent">父级对象.</param>
        /// <param name="configContext">配置上下文对象.</param>
        /// <param name="section">xml节点.</param>
        /// <returns>XTConfig</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new XTConfig();
            var dynamicDiscoveryNode = section.SelectSingleNode("DynamicDiscovery");
            if (dynamicDiscoveryNode != null && dynamicDiscoveryNode.Attributes != null)
            {
                var attribute = dynamicDiscoveryNode.Attributes["Enabled"];
                if (attribute != null)
                    config.DynamicDiscovery = Convert.ToBoolean(attribute.Value);
            }

            var engineNode = section.SelectSingleNode("Engine");
            if (engineNode != null && engineNode.Attributes != null)
            {
                var attribute = engineNode.Attributes["Type"];
                if (attribute != null)
                    config.EngineType = attribute.Value;
            }

            var startupNode = section.SelectSingleNode("Startup");
            if (startupNode != null && startupNode.Attributes != null)
            {
                var attribute = startupNode.Attributes["IgnoreStartupTasks"];
                if (attribute != null)
                    config.IgnoreStartupTasks = Convert.ToBoolean(attribute.Value);
            }


            var themeNode = section.SelectSingleNode("Themes");
            if (themeNode != null && themeNode.Attributes != null)
            {
                var attribute = themeNode.Attributes["basePath"];
                if (attribute != null)
                    config.ThemeBasePath = attribute.Value;
            }
            return config;
        }

        /// <summary>
        /// 配置组件检查和加载程序集的bin目录
        /// </summary>
        public bool DynamicDiscovery { get; private set; }

        /// <summary>
        /// 默认<see cref="IEngine"/>类型
        /// </summary>
        public string EngineType { get; private set; }

        /// <summary>
        /// 是否忽略启动任务
        /// </summary>
        public bool IgnoreStartupTasks { get; private set; }

        /// <summary>
        /// 主题基础路径
        /// </summary>
        public string ThemeBasePath { get; private set; }
    }
}