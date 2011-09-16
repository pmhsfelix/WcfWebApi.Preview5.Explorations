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
    [ServiceContract]
    class FirstService
    {
        [WebGet(UriTemplate="time")]
        HttpResponseMessage GetTime()
        {
            return new HttpResponseMessage
                       {
                           Content = new StringContent(DateTime.Now.ToLongTimeString())
                       };
        }
    }

    class FirstHost
    {
        public static void Run()
        {
            using (var host = new HttpServiceHost(typeof(FirstService), "http://localhost:8080/first"))
            {
                host.Open();
                Console.WriteLine("Host opened at {0}",host.Description.Endpoints[0].Address);
                Console.ReadKey();
            }
        }
    }
}
