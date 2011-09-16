using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace WcfWebApi.Preview5.Explorations.SelfHostedDemos
{
    class CustomParameterConversionHost
    {
        class AComplexType
        {
            public string Value { get; set; }
        }

        [ServiceContract]
        class TheService
        {
            [WebGet(UriTemplate = "simple/{prm}")]
            HttpResponseMessage GetSimple(string prm)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(prm)
                };
            }

            [WebGet(UriTemplate = "complex/{prm}")]
            HttpResponseMessage GetComplex(AComplexType prm)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(prm.Value)
                };
            }
        }

        class TheOperationHandler : HttpOperationHandler
        {
            private readonly HttpOperationDescription _desc;

            public TheOperationHandler(HttpOperationDescription desc)
            {
                _desc = desc;
            }

            protected override IEnumerable<HttpParameter> OnGetInputParameters()
            {
                return _desc.InputParameters
                    .Where(prm => prm.ParameterType == typeof(AComplexType))
                    .Select(prm => new HttpParameter(prm.Name, typeof(string)));
            }

            protected override IEnumerable<HttpParameter> OnGetOutputParameters()
            {
                return _desc.InputParameters
                    .Where(prm => prm.ParameterType == typeof(AComplexType));
            }

            protected override object[] OnHandle(object[] input)
            {
                return input.Select(prm => new AComplexType { Value = (prm as string).ToUpper() }).ToArray();
            }
        }

        public static void Run()
        {
            var config = new HttpConfiguration();
            config.RequestHandlers =  (coll, endpoint, desc) => coll.Add(new TheOperationHandler(desc));
            using (var host = new HttpServiceHost(typeof(TheService), config, "http://localhost:8080/conv"))
            {
                host.Open();
                Console.WriteLine("Host opened at {0}", host.Description.Endpoints[0].Address);
                Console.ReadKey();
            }
        }
    }
}
