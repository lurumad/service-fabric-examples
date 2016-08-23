// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Fabric;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace Common
{
    public class OwinCommunicationListener : ICommunicationListener
    {
        private readonly ServiceContext serviceContext;

        /// <summary>
        /// OWIN server handle.
        /// </summary>
        private IDisposable serverHandle;

        private readonly IOwinAppBuilder startup;
        private string publishAddress;
        private string listeningAddress;
        private readonly string appRoot;

        public OwinCommunicationListener(IOwinAppBuilder startup, ServiceContext serviceContext)
            : this(null, startup, serviceContext)
        {
        }

        public OwinCommunicationListener(string appRoot, IOwinAppBuilder startup, ServiceContext serviceContext)
        {
            this.startup = startup;
            this.appRoot = appRoot;
            this.serviceContext = serviceContext;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            Trace.WriteLine("Initialize");

            var serviceEndpoint = serviceContext.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            var port = serviceEndpoint.Port;

            if (serviceContext is StatefulServiceContext)
            {
                var statefulInitParams = (StatefulServiceContext) serviceContext;

                listeningAddress = string.Format(
                    CultureInfo.InvariantCulture,
                    "http://+:{0}/{1}/{2}/{3}/",
                    port,
                    statefulInitParams.PartitionId,
                    statefulInitParams.ReplicaId,
                    Guid.NewGuid());
            }
            else if (serviceContext is StatelessServiceContext)
            {
                listeningAddress = string.Format(
                    CultureInfo.InvariantCulture,
                    "http://+:{0}/{1}",
                    port,
                    string.IsNullOrWhiteSpace(appRoot)
                        ? ""
                        : appRoot.TrimEnd('/') + '/');
            }
            else
            {
                throw new InvalidOperationException();
            }

            publishAddress = listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            Trace.WriteLine($"Opening on {publishAddress}");

            try
            {
                Trace.WriteLine($"Starting web server on {listeningAddress}");

                serverHandle = WebApp.Start(listeningAddress, appBuilder => startup.Configuration(appBuilder));

                return Task.FromResult(publishAddress);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);

                StopWebServer();

                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            Trace.WriteLine("Close");

            StopWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            Trace.WriteLine("Abort");

            StopWebServer();
        }

        private void StopWebServer()
        {
            if (serverHandle == null)
            {
                return;
            }

            try
            {
                serverHandle.Dispose();
            }
            catch (ObjectDisposedException)
            {
                // no-op
            }
        }
    }
}