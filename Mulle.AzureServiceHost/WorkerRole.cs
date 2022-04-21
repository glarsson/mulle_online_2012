using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using Mulle.Lib.Repositories;
using Mulle.Lib.Entities;

namespace Mulle.AzureServiceHost
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("WorkerRole1 entry point called", "Information");

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.


            ServiceHost host = new ServiceHost(typeof(NetworkService));

            // Read config parameters
            string hostName = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["port"].IPEndpoint.Address.ToString();
            int port = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["port"].IPEndpoint.Port;
            int mexport = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["mexport"].IPEndpoint.Port;

            // Create Metadata
            ServiceMetadataBehavior metadatabehavior = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(metadatabehavior);

            Binding mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
            string mexendpointurl = string.Format("net.tcp://{0}:{1}/NetworkServiceMetadata", hostName, 8001);
            host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, mexendpointurl, new Uri(mexendpointurl));

            // Define the binding
            var netTcpBinding = new NetTcpBinding();
            netTcpBinding.Security.Mode = SecurityMode.None;
            netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            netTcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            netTcpBinding.MaxBufferPoolSize = 0;
            netTcpBinding.MaxReceivedMessageSize = int.MaxValue;
            netTcpBinding.MaxBufferSize = int.MaxValue;
            netTcpBinding.ReliableSession.Enabled = true;
            netTcpBinding.ReliableSession.Ordered = true;
            netTcpBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
                        
            // Create end point
            string endpointurl = string.Format("net.tcp://{0}:{1}/NetworkService", hostName, 9001);
            host.AddServiceEndpoint(typeof(INetworkService), netTcpBinding, endpointurl, new Uri(endpointurl));
                        
            // Open the host
            host.Open();

            // Trace output
            Trace.WriteLine("WCF Listening At: " + endpointurl);
            Trace.WriteLine("WCF MetaData Listening At: " + mexendpointurl);


            return base.OnStart();
        }
    }
}
