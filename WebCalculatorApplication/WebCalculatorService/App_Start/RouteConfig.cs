using System.Web.Http;

namespace WebCalculatorService
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                "CalculatorApi",
                "api/{action}",
                new { controller = "Default" });
        }
    }
}
