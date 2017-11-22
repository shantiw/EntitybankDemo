using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace XData.Web.Controllers
{
    [RoutePrefix("Admin/SecurityLog")]
    [Route("{action=Index}")]
    public class SecurityLogController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}