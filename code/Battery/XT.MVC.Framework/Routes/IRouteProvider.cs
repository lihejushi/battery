using System.Web.Routing;

namespace XT.MVC.Framework.Routes
{
    public interface IRouteProvider
    {
        void RegisterRoutes(RouteCollection routes);

        int Priority { get; }
    }
}
