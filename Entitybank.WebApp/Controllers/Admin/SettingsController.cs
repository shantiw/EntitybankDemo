using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XData.Web.Controllers
{
    [RoutePrefix("Admin/Settings")]
    [Route("{action=Index}")]
    public class SettingsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Route("Edit")]
        public ActionResult Edit()
        {
            return View();
        }
    }
}
