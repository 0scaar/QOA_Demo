using System.Web.Mvc;
using System.Web.Routing;

namespace QOA.DEMO.ClienteWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(name: "FuentesBootstrap", url: "fonts/{archivo}", defaults: new { controller = "Assets", action = "Font" });
            routes.MapRoute(name: "Default", url: "{controller}/{action}/{id}", defaults: new { controller = "Login", action = "Ingresar", id = UrlParameter.Optional });
        }
    }
}
