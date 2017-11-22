using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using XData.Web.Models;

namespace XData.Web.Controllers
{
    [RoutePrefix("Admin/Roles")]
    [Route("{action=Index}")]
    public class RolesController : Controller
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

        [Route("IsUnique")]
        public ActionResult IsUnique()
        {
            bool isUnique = false;
            string roleName = Request.QueryString["RoleName"];

            isUnique = new WebModel().IsUnique("Role", "LoweredRoleName", roleName.ToLower(), Request);
            var obj = new { valid = isUnique };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


    }
}
