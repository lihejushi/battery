using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;

namespace Support.File.Code.Config
{
    public class FileConfigurationSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            //解析配置文件信息，返回对象
            Dictionary<string, string> PushServiceList = new Dictionary<string, string>();
            if (section != null)
                foreach (XmlNode item in section.SelectNodes("Add"))
                {
                    string name = item.Attributes["Name"].InnerText;
                    string type = item.Attributes["Type"].InnerText;
                    PushServiceList.Add(name, type);
                }
            return PushServiceList;
        }
    }
}