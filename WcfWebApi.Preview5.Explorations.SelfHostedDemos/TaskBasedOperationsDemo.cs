using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationServer.Http;

namespace WcfWebApi.Preview5.Explorations.SelfHostedDemos
{
    
    class TaskBasedOperationsDemo
    {
        [ServiceContract, TaskService]
        class TheService
        {
            [WebGet(UriTemplate = ""), TaskOperation]
            public Task<HttpResponseMessage> Get()
            {
                Console.WriteLine("here");
                var client = new HttpClient();
                client.MaxResponseContentBufferSize = 1*1024*1024;
                return client.GetAsync("http://pfelix.files.wordpress.com/2011/04/runtime2.png");
            }
        }

        public static void Run()
        {
            using (var host = new HttpServiceHost(typeof(TheService), "http://localhost:8080/async"))
            {
                host.AddDefaultEndpoints();
                var b = host.Description.Endpoints[0].Binding as HttpBinding;
                b.MaxBufferSize = 1*1024*1024;
                b.MaxReceivedMessageSize = 1*1024*1024;
                b.TransferMode = TransferMode.Streamed;
                host.Open();
                Console.WriteLine("Host opened at {0}", host.Description.Endpoints[0].Address);
                Console.ReadKey();
            }
        }
    }
}
