using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using XData.Data.Services;

namespace XData.Web.Http.Controllers
{
    [RoutePrefix("mgmt")]
    [AllowAnonymous]
    public class UpdateController : ApiController
    {
        [Route("$now")]
        public DateTime GetNow()
        {
            return DateTime.Now;
        }

        [Route("$utcnow")]
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }

        [Route("update")]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response;
            if (UpdateAll())
            {
                response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("OK")
                };
            }
            else
            {
                response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized")
                };
            }

            return response;
        }

        private bool UpdateAll()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains("mgmtToken"))
            {
                string mgmtToken = ConfigurationManager.AppSettings["mgmtToken"];
                JToken jToken = JToken.Parse(mgmtToken);

                string format = jToken["Format"].Value<string>().Trim();
                string tolerance = jToken["Tolerance"].Value<string>() ?? "0";
                tolerance = tolerance.Trim();
                int iTolerance = int.Parse(tolerance);

                int min = 0;
                int max = 0;
                if (iTolerance < 0)
                {
                    min = iTolerance;
                }
                else if (tolerance.StartsWith("+"))
                {
                    max = iTolerance;
                }

                IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
                if (pairs.Any(p => p.Key == "token"))
                {
                    string token = pairs.First(p => p.Key == "token").Value;
                    DateTime now = GetUtcNow();
                    for (int i = min; i <= max; i++)
                    {
                        if (now.AddSeconds(i).ToString(format) == token)
                        {
                            new ConfigService().UpdateAll();
                            return true;
                        }
                    }
                }
            }
            else
            {
                new ConfigService().UpdateAll();
                return true;
            }

            return false;
        }


    }
}
