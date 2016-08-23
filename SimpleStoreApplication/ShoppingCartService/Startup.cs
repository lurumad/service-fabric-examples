using System.Web.Http;
using Common;
using Microsoft.Practices.Unity;
using Microsoft.ServiceFabric.Data;
using Owin;
using ShoppingCartService.Controllers;
using Unity.WebApi;

namespace ShoppingCartService
{
    public class Startup : IOwinAppBuilder
    {
        private readonly IReliableStateManager stateManager;

        public Startup(IReliableStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            var container = new UnityContainer();
            container.RegisterType<ShoppingCartController>(
                new TransientLifetimeManager(),
                new InjectionConstructor(stateManager));
                
            config.DependencyResolver = new UnityDependencyResolver(container);

            appBuilder.UseWebApi(config);
        }
    }
}
