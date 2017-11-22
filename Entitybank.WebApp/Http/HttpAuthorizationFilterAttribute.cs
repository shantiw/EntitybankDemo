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

namespace XData.Web.Http.Filters
{
    public class HttpAuthorizationFilterAttribute : AuthorizeAttribute //AuthorizationFilterAttribute
    {
    }
}