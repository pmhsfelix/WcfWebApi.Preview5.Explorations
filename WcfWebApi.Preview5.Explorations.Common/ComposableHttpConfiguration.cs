using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace WcfWebApi.Preview5.Explorations.Common
{
    public class ComposableHttpConfiguration : HttpConfiguration
    {

        private readonly
            ICollection<Action<ICollection<HttpOperationHandler>, ServiceEndpoint, HttpOperationDescription>> _actions =
                new List<Action<ICollection<HttpOperationHandler>, ServiceEndpoint, HttpOperationDescription>>();
        public ComposableHttpConfiguration()
        {
            base.RequestHandlers = (coll, ep, desc) =>
            {
                foreach (var action in _actions)
                {
                    action(coll, ep, desc);
                }
            };
        }

        public void AddRequestHandlers(Action<ICollection<HttpOperationHandler>, ServiceEndpoint, HttpOperationDescription> requestHandlerDelegate)
        {
            _actions.Add(requestHandlerDelegate);
        }
    }
}
