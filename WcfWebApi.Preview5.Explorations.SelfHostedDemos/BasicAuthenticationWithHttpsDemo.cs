using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;
using WcfWebApi.Preview5.Explorations.Common;

namespace WcfWebApi.Preview5.Explorations.SelfHostedDemos
{
    class BasicAuthenticationWithHttpsDemo
    {
        [ServiceContract]
        class TheService
        {
            [Authorize("Pedro")]
            [WebGet(UriTemplate="")]
            HttpResponseMessage Get(IPrincipal principal)
            {
                return new HttpResponseMessage
                           {
                               Content = new StringContent("hello " + principal.Identity.Name)
                           };
            }
        }

        public static void Run()
        {
            var conf = new HttpConfiguration
                           {
                               RequestHandlers =
                                   (coll, ep, desc) =>
                                       {
                                           if (
                                               desc.InputParameters.Any(
                                                   p => p.ParameterType == typeof (IPrincipal)))
                                               coll.Add(new PrincipalFromSecurityContext());
                                       }
                           }
                .EnableAuthorizeAttribute();
            using (var host = new HttpServiceHost(typeof(TheService), conf, new string[0]))
            {
                var ep = host.AddHttpEndpoint(typeof (TheService), "https://localhost:8435/greet");
                ep.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode =
                    UserNamePasswordValidationMode.Custom;
                host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new MyCustomValidator();

                host.Open();
                Console.WriteLine("Service is opened at {0}, press any key to continue",ep.Address);
                Console.ReadKey();
            }
        }

        class MyCustomValidator : UserNamePasswordValidator
        {
            public override void Validate(string userName, string password)
            {
                if (!Object.Equals(userName, password)) throw new FaultException("Unknown Username or Incorrect Password");
            }
        }


        class PrincipalFromSecurityContext : HttpOperationHandler<HttpRequestMessage, IPrincipal>
        {
            public PrincipalFromSecurityContext()
                : base("principal")
            {
            }

            protected override IPrincipal OnHandle(HttpRequestMessage input)
            {
                return new GenericPrincipal(ServiceSecurityContext.Current.PrimaryIdentity, new string[0]);
            }
        }

        // Old alternative
        class PrincipalFromBasicAuthenticationOperationHandler : HttpOperationHandler<HttpRequestMessage, IPrincipal>
        {
            public PrincipalFromBasicAuthenticationOperationHandler()
                : base("principal")
            {
            }

            protected override IPrincipal OnHandle(HttpRequestMessage input)
            {   
                if (input.Headers.Authorization == null || input.Headers.Authorization.Scheme != "Basic")
                {
                    // If properly configured, this should never happen:
                    // this OperationHandler should only be used when 
                    // Basic authorization is required
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
                var encoded = input.Headers.Authorization.Parameter;
                var encoding = Encoding.GetEncoding("iso-8859-1");
                var userPass = encoding.GetString(Convert.FromBase64String(encoded));
                int sep = userPass.IndexOf(':');
                var username = userPass.Substring(0, sep);
                var identity = new GenericIdentity(username, "Basic");
                return new GenericPrincipal(identity, new string[] { });
            }
        }
    }
}
