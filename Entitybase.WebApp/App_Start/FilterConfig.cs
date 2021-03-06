﻿using System.Web;
using System.Web.Mvc;

namespace XData.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            //
            filters.Add(new XData.Web.Filters.AuthorizationFilterAttribute());
            filters.Add(new XData.Web.Filters.ActionFilterAttribute());
        }
    }
}
