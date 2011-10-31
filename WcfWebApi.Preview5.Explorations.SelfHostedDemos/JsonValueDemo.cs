using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace WcfWebApi.Preview5.Explorations.SelfHostedDemos
{
    public class Model
    {
        public string x { get; set; }
        public int y { get; set; }
        public double z { get; set; }
    }
    class JsonValueDemo
    {

        [ServiceContract]
        class TheService
        {
            [WebInvoke(Method="post",UriTemplate = "")]
            public HttpResponseMessage Post(ObjectContent<Model> c)
            {
                Model m = c.ReadAs();
                Console.WriteLine("x = {0}, y = {1}, z = {2}",m.x,m.y,m.z);
                return new HttpResponseMessage()
                           {
                               StatusCode = HttpStatusCode.OK
                           };
            }

            [WebInvoke(Method = "post", UriTemplate = "v2")]
            [JsonExtract]
            public HttpResponseMessage Post2(HttpRequestMessage<JsonValue> req, string x, int y, double z)
            {
                Console.WriteLine("x = {0}, y = {1}, z = {2}", x, y, z);
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK
                };
            }
        }

        public static void Run()
        {
            var config = new HttpConfiguration();
            config.RequestHandlers = (coll, ep, desc) =>
                                         {
                                             if (
                                                 desc.Attributes.Any(a => a.GetType() == typeof(JsonExtractAttribute))
                                                 )
                                             {
                                                 coll.Add(new JsonExtractHandler(desc));    
                                             }
                                         };
            using (var sh = new HttpServiceHost(typeof(TheService), config, "http://localhost:8080"))
            {
                sh.Open();
                Console.WriteLine("host is opened");
                var client = new HttpClient();
                dynamic data = new JsonObject();
                data.x = "a string";
                data.y = "13";
                data.z = "3.14";
                var resp = client.Post("http://localhost:8080/v2", new ObjectContent<JsonValue>(data, "application/json"));
                Console.WriteLine(resp.StatusCode);
            }
        }
    }

    internal class JsonExtractAttribute : Attribute
    {
    }

    internal class JsonExtractHandler : HttpOperationHandler
    {
        private readonly HttpParameter[] _prms;

        public JsonExtractHandler(HttpOperationDescription desc)
        {
            _prms =
                desc.InputParameters.Where(
                    p => p.ParameterType.IsPrimitive 
                        || p.ParameterType == typeof(string)
                        ).ToArray();
        }

        protected override IEnumerable<HttpParameter> OnGetInputParameters()
        {
            yield return new HttpParameter("request", typeof(HttpRequestMessage<JsonValue>));
        }

        protected override IEnumerable<HttpParameter> OnGetOutputParameters()
        {
            return _prms;
        }

        protected override object[] OnHandle(object[] input)
        {
            var req = input[0] as HttpRequestMessage<JsonValue>;
            var value = req.Content.ReadAs();
            var output = new object[_prms.Length];
            for (int i = 0; i < _prms.Length; ++i)
            {
                JsonValue m = value.GetValue(_prms[i].Name);
                output[i] = m.ReadAs(_prms[i].ParameterType, null);
                if (output[i] == null && _prms[i].ParameterType.IsValueType)
                    throw new HttpResponseException(new HttpResponseMessage()
                                                        {
                                                            StatusCode = HttpStatusCode.BadRequest,
                                                            Content =
                                                                new StringContent(string.Format("{0} is required",
                                                                                                _prms[i].Name))
                                                        });
            }
            return output;
        }

    }
}
