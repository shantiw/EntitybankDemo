using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XData.Web.Models;

namespace XData.Web.Http.Controllers
{
    [RoutePrefix("Api/Account")]
    public class AccountController : ApiController
    {
        [Route("Password")]
        public void Put([FromBody]JToken value)
        {
            string password = (string)value["Password"];
            string newPassword = (string)value["NewPassword"];
            WebSecurity.ChangePassword(password, newPassword);
        }
    }
}
