using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace ComPerWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {

        private const String ServerEndpointName = "MainWorkerEndpoint";
        private TcpListener _tcpListener;

        public override void Run()
        {
            var ipEndpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[ServerEndpointName].IPEndpoint;
            Trace.TraceInformation("WorkerRole open at IPEndpoint: {0}", ipEndpoint);

            try
            {
                _tcpListener = new TcpListener(ipEndpoint);
                _tcpListener.Start();
            }
            catch (Exception e)
            {
                Trace.TraceError("WorkerRole could not start! Error: {0}", e);
                return;
            }

            AcceptIncomingClientAsync().Wait();
        }

        private async Task AcceptIncomingClientAsync()
        {
            while (true)
            {
                try
                {
                    var acceptedTcpClient = await _tcpListener.AcceptTcpClientAsync();
                    var tcpClientServer = new ComPerServer(acceptedTcpClient);
                    tcpClientServer.Start();
                }
                catch (Exception e)
                {
                    Trace.TraceInformation("Caught exception while in AcceptIncomingClientAsync: {0}", e);
                }
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            RoleEnvironment.Changing += (sender, args) =>
            {
                if (args.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
                {
                    args.Cancel = true;
                }
            };

            return base.OnStart();
        }
    }
}
