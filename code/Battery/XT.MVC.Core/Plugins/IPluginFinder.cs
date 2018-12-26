using System.Collections.Generic;

namespace XT.MVC.Core.Plugins
{
    /// <summary>
    /// 插件寻找类
    /// </summary>
    public interface IPluginFinder
    {
        /// <summary>
        /// 检查指定应用中指定插件是否有效
        /// </summary>
        /// <param name="pluginDescriptor">用于检查的插件描述</param>
        /// <param name="targetId">应用ID</param>
        /// <returns>true - 有效</returns>
        bool AuthenticateApp(PluginDescriptor pluginDescriptor, int targetId);

        /// <summary>
        /// 获取插件列表
        /// </summary>
        /// <typeparam name="T">插件类型.</typeparam>
        /// <param name="installedOnly">是否只获取已安装的插件</param>
        /// <param name="targetId">加载记录只允许在指定的应用;通过0加载所有记录</param>
        IEnumerable<T> GetPlugins<T>(bool installedOnly = true, int targetId = 0) where T : class, IPlugin;

        /// <summary>
        /// 获取插件描述符
        /// </summary>
        /// <param name="installedOnly">是否只获取已安装的插件</param>
        /// <param name="targetId">加载记录只允许在指定的应用;通过0加载所有记录</param>
        /// <returns>插件描述符</returns>
        IEnumerable<PluginDescriptor> GetPluginDescriptors(bool installedOnly = true, int targetId = 0);

        /// <summary>
        /// 获取插件描述符
        /// </summary>
        /// <typeparam name="T">指定的插件类型</typeparam>
        /// <param name="installedOnly">是否只获取已安装的插件</param>
        /// <param name="targetId">加载记录只允许在指定的应用;通过0加载所有记录</param>
        /// <returns>插件描述符</returns>
        IEnumerable<PluginDescriptor> GetPluginDescriptors<T>(bool installedOnly = true, int targetId = 0) where T : class, IPlugin;

        /// <summary>
        /// 根据插件的系统名称获取描述符
        /// </summary>
        /// <param name="systemName">插件的系统名称</param>
        /// <param name="installedOnly">是否只获取已安装的插件</param>
        /// <returns>插件描述符</returns>
        PluginDescriptor GetPluginDescriptorBySystemName(string systemName, bool installedOnly = true);

        /// <summary>
        /// 根据插件的系统名称获取描述符
        /// </summary>
        /// <typeparam name="T">指定的插件类型.</typeparam>
        /// <param name="systemName">插件的系统名称</param>
        /// <param name="installedOnly">是否只获取已安装的插件</param>
        /// <returns>>插件描述符</returns>
        PluginDescriptor GetPluginDescriptorBySystemName<T>(string systemName, bool installedOnly = true) where T : class, IPlugin;

        /// <summary>
        /// 重新加载插件
        /// </summary>
        void ReloadPlugins();
    }
}
