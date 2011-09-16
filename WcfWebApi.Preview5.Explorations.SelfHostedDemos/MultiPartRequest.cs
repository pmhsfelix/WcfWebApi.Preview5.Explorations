using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace WcfWebApi.Preview5.Explorations.SelfHostedDemos
{
    class MultiPartRequest
    {
        [ServiceContract]
        class TheService
        {
            [WebInvoke(Method = "POST",UriTemplate = "")]
            HttpResponseMessage Post(HttpRequestMessage req)
            {
                try
                {
                    if (!req.Content.IsMimeMultipartContent())
                    {
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                    }
                    var prov = new MultipartFormDataStreamProvider("c:/users/pedro/data/tmp");
                    var conts = req.Content.ReadAsMultipart(prov);
                    var sb = new StringBuilder("Files uploaded\n");
                    foreach (var me in prov.BodyPartFileNames)
                    {
                        sb.AppendFormat("{0}->{1};\n ", me.Key, me.Value);
                    }
                    return new HttpResponseMessage
                               {
                                   Content = new StringContent(sb.ToString())
                               };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(e.ToString())
                    };
                }
            }
        }

        public static void Run()
        {
            using (var host = new HttpServiceHost(typeof(TheService), "http://localhost:8080/upload"))
            {
                host.Open();
                Console.WriteLine("Host opened at {0}", host.Description.Endpoints[0].Address);
                Console.ReadKey();
            }
        }

    }
}
