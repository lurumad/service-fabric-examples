using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace CalculatorService
{
    internal sealed class CalculatorService : StatelessService, ICalculatorService
    {
        public CalculatorService(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(context =>
                    new FabricTransportServiceRemotingListener(context, this))
            };
        }

        public Task<string> Add(int a, int b)
        {
            return Task.FromResult($"Instance {Context.InstanceId} returns {a + b}");
        }

        public Task<string> Subtract(int a, int b)
        {
            return Task.FromResult($"Instance {Context.InstanceId} returns {a - b}");
        }
    }
}
