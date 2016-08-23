using System.Web.Http;
using Owin;

namespace WebCalculatorService
{
    public static class Startup
    {
        public static void ConfigureApp(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();
            FormatterConfig.ConfigureFormatters(configuration.Formatters);
            RouteConfig.RegisterRoutes(configuration.Routes);

            app.UseWebApi(configuration);
        }
    }
}
