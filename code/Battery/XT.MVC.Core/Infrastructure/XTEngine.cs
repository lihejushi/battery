using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Autofac;
using XT.MVC.Core.Infrastructure.DependencyManagement;

namespace XT.MVC.Core.Infrastructure
{
    public class XTEngine : IEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion

        #region Ctor

        /// <summary>
        /// 创建一个实例的内容引擎使用默认设置和配置。
		/// </summary>
		public XTEngine() 
            : this(new ContainerConfigurer())
		{
		}

		public XTEngine(ContainerConfigurer configurer)
		{
            var config = ConfigurationManager.GetSection("XTConfig") as XTConfig;
            InitializeContainer(configurer, config);
		}
        
        #endregion

        #region Utilities

        /// <summary>
        /// 运行启动任务
        /// </summary>
        private void RunStartupTasks()
        {
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        private void InitializeContainer(ContainerConfigurer configurer, XTConfig config)
        {
            var builder = new ContainerBuilder();

            _containerManager = new ContainerManager(builder.Build());
            configurer.Configure(this, _containerManager, config);
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// 初始化组件和插件的运行环境。
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize(XTConfig config)
        {
            //bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();
            //if (databaseInstalled)
            //{
            //    //startup tasks
            //    RunStartupTasks();
            //}


            //startup tasks
            if (!config.IgnoreStartupTasks)
            {
                RunStartupTasks();
            }
        }

        public T Resolve<T>(string key = "", ILifetimeScope scope = null) where T : class
        {
            return ContainerManager.Resolve<T>(key, scope);
        }

        //public T Resolve<T>() where T : class
        //{
        //    return ContainerManager.Resolve<T>();
        //}

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public T[] ResolveAll<T>(string key = "", ILifetimeScope scope = null)
        {
            return ContainerManager.ResolveAll<T>(key, scope);
        }

		#endregion

        #region Properties

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
    }
}
