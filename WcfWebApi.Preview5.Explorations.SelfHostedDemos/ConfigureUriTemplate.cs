using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;

namespace WcfWebApi.Preview5.Explorations.SelfHostedDemos
{
    
    class ConfigureUriTemplate
    {
        class OverrideUriConfiguration<TService> : HttpConfiguration
        {
            private readonly IDictionary<MethodInfo,string> _map = new Dictionary<MethodInfo, string>();
            public OverrideUriConfiguration<TService> Map(string template, Expression<Action<TService>> expr)
            {
                var body = expr.Body as MethodCallExpression;
                if (body == null) throw new Exception("Invalid method expression");
                _map.Add(body.Method, template);
                return this;
            }

            protected override void OnConfigureEndpoint(HttpEndpoint endpoint)
            {
                foreach (var op in endpoint.Contract.Operations)
                {
                    var bh = op.Behaviors.Find<WebGetAttribute>();
                    if (bh == null) continue;
                    string template = null;
                    if (_map.TryGetValue(op.SyncMethod, out template))
                    {
                        bh.UriTemplate = template;
                    }
                }
                base.OnConfigureEndpoint(endpoint);
            }
        }

        [ServiceContract]
        class TheService
        {
            [WebGet()]
            public HttpResponseMessage Add(int a, int b)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent((a+b).ToString())
                };
            }
        }

        public static void Run()
        {
            var config = new OverrideUriConfiguration<TheService>()
                .Map("add/{a}/{b}", s => s.Add(default(int),default(int)));

            using (var host = new HttpServiceHost(typeof(TheService), config, "http://localhost:8080/"))
            {
                host.Open();
                Console.WriteLine("Host opened at {0}", host.Description.Endpoints[0].Address);
                Console.ReadKey();
            }
        }
    }
}
