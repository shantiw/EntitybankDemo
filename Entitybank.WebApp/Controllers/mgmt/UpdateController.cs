using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using XData.Data.Services;

namespace XData.Web.Http.Controllers
{
    [RoutePrefix("mgmt/update")]
    public class UpdateController : ApiController
    {
        private ConfigService ConfigService = new ConfigService();

        [Route()]
        public string Get()
        {
            ConfigService.UpdateAll();
            return "Success";
        }
    }
}
