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
            using (var host = new HttpServiceHost(typeof(TheService), new string[0]))
            {
                var mb = new HttpMemoryBinding();
                var ep = host.AddServiceEndpoint(typeof (TheService), mb, "http://dummy-http-scheme-uri");
                foreach (var op in ep.Contract.Operations)
                {
                    op.Behaviors.Find<OperationBehaviorAttribute>().AutoDisposeParameters = false;
                }
                host.Open();
                Console.WriteLine("Host opened at {0}", host.Description.Endpoints[0].Address);
                var client = new HttpClient(mb.GetHttpMemoryHandler());
                // Yes, it must be async. Apparently, sync is not supported with the memory channel
                var aresp = client.GetAsync("http://another-dummy-http-scheme-uri/hello");
                Console.WriteLine(aresp.Result.Content.ReadAsStringAsync().Result);
                Console.ReadKey();
            }
        }
    }
}
