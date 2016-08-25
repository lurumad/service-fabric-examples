using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Testability.Scenario;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Testability
{
    class Program
    {
        static void Main(string[] args)
        {
            var clusterConnection = "localhost:19000";
            Console.WriteLine("Starting Chaos Test Scenario...");

            try
            {
                RunChaosTestScenarioAsync(clusterConnection).Wait();
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("Chaos Test Scenario did not complete: ");

                foreach (Exception ex in ae.InnerExceptions)
                {
                    if (ex is FabricException)
                    {
                        Console.WriteLine("HResult: {0} Message: {1}", ex.HResult, ex.Message);
                    }
                }
            }

            Console.WriteLine("Chaos Test Scenario completed.");

            Console.ReadKey();
        }

        static async Task RunChaosTestScenarioAsync(string clusterConnection)
        {
            var maxClusterStabilizationTimeout = TimeSpan.FromSeconds(180);
            uint maxConcurrentFaults = 3;
            var enableMoveReplicaFaults = true;

            var fabricClient = new FabricClient(clusterConnection);

            var timeToRun=  TimeSpan.FromMinutes(2);

            var scenarioParameters = new ChaosTestScenarioParameters(
                maxClusterStabilizationTimeout,
                maxConcurrentFaults,
                enableMoveReplicaFaults,
                timeToRun);

            var chaosTestScenario = new ChaosTestScenario(
                fabricClient,
                scenarioParameters);

            try
            {
                await chaosTestScenario.ExecuteAsync(CancellationToken.None);
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }
    }
}
