using System.Web.Mvc;
using System.Web.Routing;

namespace TixGurusWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Events", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
