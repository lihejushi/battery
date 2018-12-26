using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Battery.BSS;
using Battery.Framework.Core.AccessType;
using Battery.Framework.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using XT.MVC.Core;
using XT.MVC.Core.Caching;
using XT.MVC.Core.Configuration;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Infrastructure.DependencyManagement;
using XT.MVC.Core.Logging;
using XT.MVC.Core.Plugins;
using XT.MVC.Core.Services.Configuration;
using XT.MVC.Core.Services.Widget;
using XT.MVC.Core.Web;
using XT.MVC.Framework.EmbeddedViews;
using XT.MVC.Framework.Routes;
using XT.MVC.Framework.Themes;
using XT.MVC.Framework.UI;

namespace Battery.Framework.Core
{
    public class XTDependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c => new HttpContextWrapper(HttpContext.Current) as HttpContextBase)
                .As<HttpContextBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerHttpRequest();

            //注入webapi
            //builder.Register(c => new HttpContextWrapper(HttpContext.Current) as HttpContextBase).As<HttpContextBase>().InstancePerApiRequest();
            //builder.Register(c => c.Resolve<HttpContextBase>().Request).As<HttpRequestBase>().InstancePerApiRequest();
            //builder.Register(c => c.Resolve<HttpContextBase>().Response).As<HttpResponseBase>().InstancePerApiRequest();
            //builder.Register(c => c.Resolve<HttpContextBase>().Server).As<HttpServerUtilityBase>().InstancePerApiRequest();
            //builder.Register(c => c.Resolve<HttpContextBase>().Session).As<HttpSessionStateBase>().InstancePerApiRequest();
            //builder.RegisterApiControllers(typeFinder.GetAssemblies().ToArray());

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerHttpRequest();
            builder.RegisterType<CookieManager>().As<ICookieManager>().InstancePerHttpRequest();
            builder.RegisterType<SessionManager>().As<ISessionManager>().InstancePerHttpRequest();

            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());


            //plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerHttpRequest();

            //cache manager
            //builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("xt_cache_static").SingleInstance();
            builder.RegisterType<XT.MVC.RedisCache.RedisCacheManager>().As<ICacheManager>().Named<ICacheManager>("xt_cache_static").SingleInstance();
            //builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("xt_cache_per_request").InstancePerHttpRequest();


            //work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerHttpRequest();
            //app context
            builder.RegisterType<WebAppContext>().As<IAppContext>().InstancePerHttpRequest();
            //builder.RegisterType<WxUser>().As<IWxUser>().InstancePerHttpRequest();

            //services
            builder.RegisterType<DefaultLogger>().As<ILogger>().Keyed<ILogger>("DefaultLogger").SingleInstance();
            //Api结果
            builder.RegisterType<ApiLogger>().As<ILogger>().Keyed<ILogger>("ApiLogger").SingleInstance();
            //BSS结果
            builder.RegisterType<DefaultBSS>().As<IBSS>().SingleInstance();


            builder.RegisterType<WidgetService>().As<IWidgetService>().InstancePerHttpRequest();
            builder.RegisterType<PageHeadBuilder>().As<IPageHeadBuilder>().InstancePerHttpRequest();

            //builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerHttpRequest();

            builder.RegisterType<AccessTypeHelper>().As<IAccessTypeHelper>().InstancePerHttpRequest();
            builder.RegisterType<ThemeProvider>().As<IThemeProvider>().InstancePerHttpRequest();
            builder.RegisterType<ThemeContext>().As<IThemeContext>().InstancePerHttpRequest();

            builder.RegisterType<EmbeddedViewResolver>().As<IEmbeddedViewResolver>().SingleInstance();
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();

            //注入设置延迟加载
            builder.RegisterSource(new SettingsSource());


            //builder.RegisterType<DefaultGame>().Named<IGame>("Default").InstancePerApiRequest();
            //builder.RegisterType<DefaultDevice>().Named<IDevice>("Default").InstancePerApiRequest();
        }

        public int Order
        {
            get { return 0; }
        }
    }

    /// <summary>
    /// autofac延迟加载
    /// </summary>
    public class SettingsSource : IRegistrationSource
    {
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        public IEnumerable<IComponentRegistration> RegistrationsFor(
                Autofac.Core.Service service,
                Func<Autofac.Core.Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)buildMethod.Invoke(null, null);
            }
        }

        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) =>
                {
                    //自动将[设置]加载进入
                    var currentAppId = c.Resolve<IAppContext>().CurrentApp.AppId;
                    return c.Resolve<ISettingService>().LoadSetting<TSettings>(currentAppId);
                })
                .InstancePerHttpRequest()
                .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents { get { return false; } }
    }
}
