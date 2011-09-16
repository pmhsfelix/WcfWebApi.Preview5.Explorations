using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace WcfWebApi.Preview5.Explorations.Common
{
    public class ParameterConversionOperationHandler<T> : HttpOperationHandler
    {
        private readonly HttpOperationDescription _desc;
        private readonly Func<string, T> _conv;

        public ParameterConversionOperationHandler(HttpOperationDescription desc, Func<string, T> conv)
        {
            _desc = desc;
            _conv = conv;
        }

        protected override IEnumerable<HttpParameter> OnGetInputParameters()
        {
            return _desc.InputParameters
                .Where(prm => prm.ParameterType == typeof(T))
                .Select(prm => new HttpParameter(prm.Name, typeof(string)));
        }

        protected override IEnumerable<HttpParameter> OnGetOutputParameters()
        {
            return _desc.InputParameters
                .Where(prm => prm.ParameterType == typeof(T));
        }

        protected override object[] OnHandle(object[] input)
        {
            var res = new object[input.Length];
            for (var i = 0; i < input.Length; ++i)
            {
                res[i] = _conv(input[i] as string);
            }
            return res;
        }
    }

    public static class ParameterConversionExtensions
    {
        public static HttpConfiguration UseParameterConverterFor<T>(this HttpConfiguration cfg, Func<string, T> conv)
        {
            cfg.AddRequestHandlers((coll, ep, desc) =>
                                       {
                                           if (desc.InputParameters.Any(p => p.ParameterType == typeof (T)))
                                           {
                                               coll.Add(new ParameterConversionOperationHandler<T>(desc, conv));
                                           }
                                       });
            return cfg;
        }
    }
}
