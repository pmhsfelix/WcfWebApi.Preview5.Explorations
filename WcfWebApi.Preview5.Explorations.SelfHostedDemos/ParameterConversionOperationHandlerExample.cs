using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.ApplicationServer.Http;
using WcfWebApi.Preview5.Explorations.Common;

namespace WcfWebApi.Preview5.Explorations.SelfHostedDemos
{
    class ParameterConversionOperationHandlerExample
    {
        class AComplexType
        {
            public string Value { get; set; }
        }
        class AnotherComplexType
        {
            public int Value { get; set; }
        }

        [ServiceContract]
        class TheService
        {
            [WebGet(UriTemplate = "{prm1}/{prm2}")]
            HttpResponseMessage Get(AComplexType prm1, AnotherComplexType prm2)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(prm1.Value + prm2.Value)
                };
            }
        }

        public static void Run()
        {
            var config = new HttpConfiguration()
                .UseParameterConverterFor<AComplexType>(s => new AComplexType {Value = s.ToUpper()})
                .UseParameterConverterFor<AnotherComplexType>(s => new AnotherComplexType {Value = Int32.Parse(s)+1});

            using (var host = new HttpServiceHost(typeof(TheService), config, "http://localhost:8080/conv"))
            {
                host.Open();
                Console.WriteLine("Host opened at {0}", host.Description.Endpoints[0].Address);
                var client = new HttpClient();
                Console.WriteLine(client.Get("http://localhost:8080/conv/hello/3").Content.ReadAsString());
            }
        }
    }
}
