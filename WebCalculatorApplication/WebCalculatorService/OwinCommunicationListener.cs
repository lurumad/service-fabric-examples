using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Owin;

namespace WebCalculatorService
{
    public class OwinCommunicationListener : ICommunicationListener
    {
        private readonly Action<IAppBuilder> startup;
        private readonly ServiceContext context;
        private readonly ServiceEventSource eventSource;
        private IDisposable serverHandle;
        private string listeningAddress;

        public OwinCommunicationListener(
            Action<IAppBuilder> startup,
            ServiceContext context,
            ServiceEventSource eventSource)
        {
            this.startup = startup;
            this.context = context;
            this.eventSource = eventSource;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var serviceEndpoint = context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            var port = serviceEndpoint.Port;
            listeningAddress = $"http://+:{port}/";

            serverHandle = WebApp.Start(listeningAddress, builder => startup.Invoke(builder));

            var resultAddress = listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            eventSource.Message($"Listening on {resultAddress}");

            return Task.FromResult(resultAddress);
        }
        
        public Task CloseAsync(CancellationToken cancellationToken)
        {
            StopWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            StopWebServer();
        }

        private void StopWebServer()
        {
            serverHandle?.Dispose();
        }
    }
}
