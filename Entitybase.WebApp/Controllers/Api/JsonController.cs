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
    [RoutePrefix("Api")]
    public class JsonController : ApiController
    {
        private JsonModel Model = new JsonModel();

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
        public HttpResponseMessage Post([FromBody]JToken value)
        {
            return Model.Create(value, Request);
        }

        [Route("{entity}")]
        public HttpResponseMessage Post(string entity, [FromBody]JToken value)
        {
            return Model.Create(entity, value, Request);
        }

        [Route()]
        public void Put([FromBody]JToken value)
        {
            Model.Update(value, Request);
        }

        [Route("{entity}")]
        public void Put(string entity, [FromBody]JToken value)
        {
            Model.Update(entity, value, Request);
        }

        [Route()]
        public void Delete()
        {
            Model.Delete(Request);
        }

        [Route("{entity}")]
        public void Delete(string entity, [FromBody]JToken value)
        {
            Model.Delete(entity, value, Request);
        }


    }
}
