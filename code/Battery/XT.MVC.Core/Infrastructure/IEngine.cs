using Autofac;
using System;
using XT.MVC.Core.Infrastructure.DependencyManagement;

namespace XT.MVC.Core.Infrastructure
{
    /// <summary>
    /// 运行环境
    /// </summary>
    public interface IEngine
    {
        ContainerManager ContainerManager { get; }
        
        /// <summary>
        /// 初始化插件运行环境
        /// </summary>
        /// <param name="config"></param>
        void Initialize(XTConfig config);

        T Resolve<T>(string key = "", ILifetimeScope scope = null) where T : class;

        object Resolve(Type type);

        T[] ResolveAll<T>(string key = "", ILifetimeScope scope = null);
    }
}
