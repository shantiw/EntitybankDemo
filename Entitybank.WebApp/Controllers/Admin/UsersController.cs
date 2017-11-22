using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace XData.Web.Controllers
{
    [RoutePrefix("Admin/Users")]
    [Route("{action=Index}")]
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        [Route("Delete/{id}")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        [Route("Details/{id}")]
        public ActionResult Details(int id)
        {
            return View();
        }


    }
}
