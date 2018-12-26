using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using XT.MVC.Core.Infrastructure;

namespace XT.MVC.Core.Plugins
{
    public class PluginDescriptor : IComparable<PluginDescriptor>
    {
        public PluginDescriptor()
        {
            this.SupportedVersions = new List<string>();
            this.LimitedToApps = new List<int>();
        }


        public PluginDescriptor(Assembly referencedAssembly, FileInfo originalAssemblyFile,
            Type pluginType)
            : this()
        {
            this.ReferencedAssembly = referencedAssembly;
            this.OriginalAssemblyFile = originalAssemblyFile;
            this.PluginType = pluginType;
        }
        /// <summary>
        /// 插件文件名称
        /// </summary>
        public virtual string PluginFileName { get; set; }

        /// <summary>
        /// 插件类型
        /// </summary>
        public virtual Type PluginType { get; set; }

        /// <summary>
        /// 插件依赖项
        /// </summary>
        public virtual Assembly ReferencedAssembly { get; internal set; }

        /// <summary>
        /// 依赖项原始文件
        /// </summary>
        public virtual FileInfo OriginalAssemblyFile { get; internal set; }

        /// <summary>
        /// 插件分组
        /// </summary>
        public virtual string Group { get; set; }

        /// <summary>
        /// 插件友好名称
        /// </summary>
        public virtual string FriendlyName { get; set; }

        /// <summary>
        /// 插件系统名称
        /// </summary>
        public virtual string SystemName { get; set; }

        /// <summary>
        /// 插件版本
        /// </summary>
        public virtual string Version { get; set; }

        /// <summary>
        /// 插件支持版本
        /// </summary>
        public virtual IList<string> SupportedVersions { get; set; }

        /// <summary>
        /// 插件作者
        /// </summary>
        public virtual string Author { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// 获取或设置列表的标识符存储在这个插件可用。如果是空的,那么这个插件可用在所有商店
        /// </summary>
        public virtual IList<int> LimitedToApps { get; set; }

        /// <summary>
        /// 判断插件是否安装
        /// </summary>
        public virtual bool Installed { get; set; }
        /// <summary>
        /// 实例化插件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Instance<T>() where T : class, IPlugin
        {
            object instance;
            if (!EngineContext.Current.ContainerManager.TryResolve(PluginType, null, out instance))
            {
                instance = EngineContext.Current.ContainerManager.ResolveUnregistered(PluginType);
            }
            var typedInstance = instance as T;
            if (typedInstance != null)
                typedInstance.PluginDescriptor = this;
            return typedInstance;
        }

        public IPlugin Instance()
        {
            return Instance<IPlugin>();
        }

        public int CompareTo(PluginDescriptor other)
        {
            if (DisplayOrder != other.DisplayOrder)
                return DisplayOrder.CompareTo(other.DisplayOrder);
            else
                return FriendlyName.CompareTo(other.FriendlyName);
        }

        public override string ToString()
        {
            return FriendlyName;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PluginDescriptor;
            return other != null && 
                SystemName != null &&
                SystemName.Equals(other.SystemName);
        }

        public override int GetHashCode()
        {
            return SystemName.GetHashCode();
        }
    }
}
