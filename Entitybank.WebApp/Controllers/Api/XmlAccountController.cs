using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using XData.Web.Models;

namespace XData.Web.Http.Controllers
{
    [RoutePrefix("xml/Account")]
    public class XmlAccountController : ApiController
    {
        [Route()]
        public XElement Get()
        {
            return XmlSecurity.GetPublicKey();
        }

        [Route()]
        public XElement Post([FromBody]XElement value)
        {
            return XmlSecurity.Login(value);
        }

        [Route()]
        public void Delete()
        {
            XmlSecurity.Logout();
        }

        [Route()]
        public void Put([FromBody]XElement value)
        {
            XmlSecurity.ChangePassword(value);
        }

    }
}
