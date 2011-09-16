using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.ApplicationServer.Http;

namespace WcfWebApi.Preview5.Explorations.SelfHostedDemos
{
    class MemoryHost
    {
        [ServiceContract]
        class TheService
        {
            [WebGet(UriTemplate = "{prm}")]
            HttpResponseMessage Echo(string prm)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(prm.ToUpper())
                };
            }
        }
        public static void Run()
        {
            using (var host = new HttpServiceHost(typeof(TheService), "http://localhost:8080/first"))
            {
                var mb = new HttpMemoryBinding();
                host.AddServiceEndpoint(typeof (TheService), mb, "http://memoryhost");
                host.Open();
                Console.WriteLine("Host opened at {0}", host.Description.Endpoints[0].Address);
                var client = new HttpClient(mb.GetHttpMemoryHandler());
                // Yes, it must be async. Apparently, sync is not supported with the memory channel
                var aresp = client.GetAsync("http://memoryhost/hello");
                Console.WriteLine(aresp.Result.Content.ReadAsString());
                Console.ReadKey();
            }
        }
    }
}
