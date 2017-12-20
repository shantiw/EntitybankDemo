using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using XData.Web.Models;

namespace XData.Web.Filters
{
    public class AuthorizationFilterAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                MvcHandler handler = httpContext.Handler as MvcHandler;
                Route route = handler.RequestContext.RouteData.Route as Route;
                if (route.Url.StartsWith("Admin"))
                {
                    return WebSecurity.IsInRole("Administrators");

                }
            }

            return base.AuthorizeCore(httpContext);
        }


    }
}
