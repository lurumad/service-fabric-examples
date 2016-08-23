using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace WebCalculatorService
{
    internal sealed class WebCalculatorService : StatelessService
    {
        public WebCalculatorService(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(
                    context => new OwinCommunicationListener(
                        Startup.ConfigureApp,
                        context,
                        ServiceEventSource.Current)),
            };
        }
    }
}
