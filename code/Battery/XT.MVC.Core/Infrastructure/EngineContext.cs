using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace XT.MVC.Core.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the XT engine.
    /// </summary>
    public class EngineContext
    {
        #region Initialization Methods
        /// <summary>初始化引擎.</summary>
        /// <param name="forceRecreate">创建一个新工厂实例之前尽管工厂已经被初始化.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                var config = ConfigurationManager.GetSection("XTConfig") as XTConfig;
                Debug.WriteLine("Constructing engine " + DateTime.Now);
                Singleton<IEngine>.Instance = CreateEngineInstance(config);
                Debug.WriteLine("Initializing engine " + DateTime.Now);
                Singleton<IEngine>.Instance.Initialize(config);
            }
            return Singleton<IEngine>.Instance;
        }

        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }
        
        /// <summary>
        /// 创建一个工厂实例和添加一个http应用程序注入设施。
        /// </summary>
        /// <returns>A new factory</returns>
        public static IEngine CreateEngineInstance(XTConfig config)
        {
            if (config != null && !string.IsNullOrEmpty(config.EngineType))
            {
                var engineType = Type.GetType(config.EngineType);
                if (engineType == null)
                    throw new ConfigurationErrorsException("The type '" + engineType + "' could not be found. Please check the configuration at /configuration/XT/engine[@engineType] or check for missing assemblies.");
                if (!typeof(IEngine).IsAssignableFrom(engineType))
                    throw new ConfigurationErrorsException("The type '" + engineType + "' doesn't implement 'XT.MVC.Core.Infrastructure.IEngine' and cannot be configured in /configuration/XT/engine[@engineType] for that purpose.");
                return Activator.CreateInstance(engineType) as IEngine;
            }

            return new XTEngine();
        }

        #endregion

        /// <summary>获取服务访问使用的单例模.</summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Initialize(false);
                }
                return Singleton<IEngine>.Instance;
            }
        }
    }
}
