using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Battery
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
               constraints: new { id = @"\d+" },
               namespaces: new[] { "Battery.Controllers" }
           );
            routes.MapRoute(
                 name: "Default_NoId",
                 url: "{controller}/{action}",
                 defaults: new { controller = "Home", action = "Index" },
                 namespaces: new[] { "Battery.Controllers" }
             );
        }
    }
}
