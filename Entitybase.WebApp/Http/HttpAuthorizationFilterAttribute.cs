using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http;
using System.Threading;
using System.Net.Http;
using XData.Web.Models;

namespace XData.Web.Http.Filters
{
    public class HttpAuthorizationFilterAttribute : AuthorizeAttribute //AuthorizationFilterAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                if (actionContext.ControllerContext.RouteData.Route.RouteTemplate.StartsWith("Admin"))
                {
                    return WebSecurity.IsInRole("Administrators");
                }
            }

            return base.IsAuthorized(actionContext);
        }


    }
}
