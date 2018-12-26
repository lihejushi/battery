using System.Web.Routing;

namespace XT.MVC.Framework.Routes
{
    public interface IRoutePublisher
    {
        void RegisterRoutes(RouteCollection routeCollection);
    }
}
