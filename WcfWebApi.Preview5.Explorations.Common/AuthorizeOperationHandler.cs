using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using Microsoft.ApplicationServer.Http.Description;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace WcfWebApi.Preview5.Explorations.Common
{
    public class AuthorizeOperationHandler : HttpOperationHandler
    {
        private readonly AuthorizeAttribute _attr;

        public AuthorizeOperationHandler(AuthorizeAttribute attr)
        {
            _attr = attr;
        }

        protected override IEnumerable<HttpParameter> OnGetInputParameters()
        {
            yield return new HttpParameter("principal",typeof(IPrincipal));
        }

        protected override IEnumerable<HttpParameter> OnGetOutputParameters()
        {
            yield break;
        }

        protected override object[] OnHandle(object[] input)
        {
            var p = input[0] as IPrincipal;
            Console.WriteLine(string.Format("Checking authorization. Require {0}, found {1}",_attr.Name, p.Identity.Name));
            if (!_attr.Name.Equals(p.Identity.Name))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            Console.WriteLine("Allowing it");
            return new object[0];
        }
    }

    public class AuthorizeAttribute : Attribute
    {
        /*
        private readonly string[] _roles;
        public string[] Roles { get { return _roles; } }
        public AuthorizeAttribute(string[] roles)
        {
            _roles = roles;
        }
        */
        public AuthorizeAttribute(string name)
        {
            _name = name;
        }
        private readonly string _name;
        public String Name { get { return _name; } }
    }

}
