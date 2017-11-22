using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using XData.Web.Http.Filters;
using XData.Web.Models;

namespace XData.Web.Http.Controllers
{
    [RoutePrefix("xml")]
    [CompressionActionFilter]
    public class XmlController : ApiController
    {
        private XmlModel Model = new XmlModel();

        [Route("$now")]
        public HttpResponseMessage GetNow()
        {
            return Model.GetNow(Request);
        }

        [Route("$utcnow")]
        public HttpResponseMessage GetUtcNow()
        {
            return Model.GetUtcNow(Request);
        }

        [Route()]
        public HttpResponseMessage Get()
        {
            return Model.Get(Request);
        }

        [Route()]
        public HttpResponseMessage Post([FromBody]XElement value)
        {
            return Model.Create(value, Request);
        }

        [Route()]
        public void Delete([FromBody]XElement value)
        {
            Model.Delete(value, Request);
        }

        [Route()]
        public void Put([FromBody]XElement value)
        {
            Model.Update(value, Request);
        }


    }
}
