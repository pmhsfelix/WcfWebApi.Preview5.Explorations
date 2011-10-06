using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace WcfWebApi.Preview5.Explorations.IisHostedDemos
{
    [ServiceContract]
    public class TheService
    {
        [WebGet(UriTemplate = "")]
        HttpResponseMessage Get()
        {
            return new HttpResponseMessage
            {
                Content = new StringContent("hello Web")
            };
        }
    }
}