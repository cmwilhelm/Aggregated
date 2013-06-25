using System;
using System.ServiceProcess;
using System.Threading;

namespace Aggregated.Service
{
    internal class Service : ServiceBase
    {
        private static readonly ManualResetEventSlim WaitHandle = new ManualResetEventSlim();

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += (s, e) => WaitHandle.Set();

            var service = new Service();

            if (Environment.UserInteractive)
            {
                service.OnStart(args);

                WaitHandle.Wait();

                service.OnStop();
            }
            else
            {
                Run(service);
            }
        }
    }
}
