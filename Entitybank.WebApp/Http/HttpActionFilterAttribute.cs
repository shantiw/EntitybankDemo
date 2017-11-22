using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Xml.Linq;
using XData.Data.Services;

namespace XData.Web.Http.Filters
{
    public class HttpActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            SetLocalRequest(actionContext.Request);

            base.OnActionExecuting(actionContext);
        }

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            SetLocalRequest(actionContext.Request);

            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        private static void SetLocalRequest(HttpRequestMessage request)
        {
            RequestInfo requestInfo = new RequestInfo
            {
                Accept = request.Headers.Accept.ToString(),
                HttpMethod = request.Method.Method,
                Url = request.RequestUri.AbsoluteUri,
                UrlReferrer = request.Headers.Referrer.AbsoluteUri,
                UserAgent = request.Headers.UserAgent.ToString(),
                //UserHostAddress = null,
                //UserHostName = null
            };

            ThreadDataStore.RequestInfo = requestInfo;
        }


    }
}
