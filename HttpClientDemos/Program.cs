using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientDemos
{

    class StubMessageHandler : HttpMessageHandler
    {
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine(request);
            Console.WriteLine(request.Content.ReadAsString());
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine(request);
            Console.WriteLine(request.Content.ReadAsString());
            return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.NotImplemented));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient(new StubMessageHandler());
            dynamic json = new JsonObject();
            json.prop1 = "value1";
            json.prop2 = "value2";
            json.cprop3 = new JsonObject();
            json.cprop3.prop1 = "inner value1";
            json.cprop3.prop2 = "inner value2";
            json.aprop4 = new JsonArray() {"abc"};
            
            var cont = new ObjectContent<JsonValue>(json,"application/json");
            client.Post("http://www.example.com",cont);
        }
    }
}
