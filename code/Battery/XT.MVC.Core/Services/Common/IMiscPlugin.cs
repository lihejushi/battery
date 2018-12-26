using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using XT.MVC.Core.Plugins;

namespace XT.MVC.Core.Services.Common
{
    public partial interface IMiscPlugin : IPlugin
    {
        /// <summary>
        /// Gets a route for plugin configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);
    }
}
