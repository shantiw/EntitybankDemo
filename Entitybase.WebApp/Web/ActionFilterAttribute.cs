using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XData.Data.Services;

namespace XData.Web.Filters
{
    public class ActionFilterAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SetLocalRequest(filterContext.RequestContext.HttpContext.Request);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        private static void SetLocalRequest(HttpRequestBase request)
        {
            RequestInfo requestInfo = new RequestInfo
            {
                Accept = request.Headers["Accept"],
                HttpMethod = request.HttpMethod,
                Url = request.Url.AbsoluteUri,
                UrlReferrer = request.UrlReferrer?.AbsoluteUri,
                UserAgent = request.UserAgent,
                UserHostAddress = request.UserHostAddress
            };

            ThreadDataStore.RequestInfo = requestInfo;
        }


    }
}
