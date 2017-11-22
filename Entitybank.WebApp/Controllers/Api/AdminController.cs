using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace XData.Web.Http.Controllers
{
    [RoutePrefix("Admin/Api")]
    public class AdminController : ApiController
    {
        [Route("Users")]
        public void Post([FromBody]JToken value)
        {

        }

        [Route("Users")]
        public void Put([FromBody]JToken value)
        {
            // Request.Headers.Referrer
        }
    }
}
