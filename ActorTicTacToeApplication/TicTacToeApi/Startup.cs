using System.Web.Http;
using Owin;

namespace TicTacToeApi
{
    public static class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);
        }
    }
}
