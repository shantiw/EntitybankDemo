using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using XData.Data.Services;

namespace XData.Web.Models
{
    internal class JsonModel
    {
        public HttpResponseMessage GetNow(HttpRequestMessage request)
        {
            const string FORMAT = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK";

            JsonService service = new JsonService(request.GetQueryNameValuePairs());
            DateTime now = service.GetNow();
            string json = string.Format("{{\"now\": {0}}}", now.ToString(FORMAT));
            return CreateHttpResponseMessage(json, request);
        }

        public HttpResponseMessage GetUtcNow(HttpRequestMessage request)
        {
            const string FORMAT = "yyyy-MM-ddTHH:mm:ss.FFFFFFFZ";

            JsonService service = new JsonService(request.GetQueryNameValuePairs());
            DateTime utcNow = service.GetUtcNow();
            string json = string.Format("{{\"utcnow\": {0}}}", utcNow.ToString(FORMAT));
            return CreateHttpResponseMessage(json, request);
        }

        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            IEnumerable<KeyValuePair<string, string>> keyValuePairs = GetKeyValuePairs(request);
            string json = new JsonService(keyValuePairs).Get();

            return CreateHttpResponseMessage(json, request);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs(HttpRequestMessage request)
        {
            IEnumerable<KeyValuePair<string, string>> nameValues = request.GetQueryNameValuePairs();
            if (nameValues.Any(p => p.Key == "key"))
            {
                string value = nameValues.First(p => p.Key == "key").Value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    List<KeyValuePair<string, string>> newList = new List<KeyValuePair<string, string>>();
                    newList.AddRange(nameValues.Where(p => p.Key != "key"));
                    return newList;
                }
                return nameValues;
            }

            if (request.Headers.Referrer == null) return nameValues;
            string referrer = request.Headers.Referrer.AbsolutePath;
            if (string.IsNullOrWhiteSpace(referrer)) return nameValues;

            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>(nameValues);
            string lowered = referrer.ToLower();
            if (lowered.EndsWith("/index"))
            {
                referrer = referrer.Substring(0, referrer.Length - "/index".Length);
                list.Add(new KeyValuePair<string, string>("key", referrer));
                return list;
            }

            if (lowered.EndsWith("/create"))
            {
                list.Add(new KeyValuePair<string, string>("key", referrer));
                return list;
            }

            int lastIndex = lowered.LastIndexOf('/');
            lowered = referrer.Substring(0, lastIndex).ToLower();
            if (lowered.EndsWith("/edit") || lowered.EndsWith("/delete") || lowered.EndsWith("/details"))
            {
                string key = referrer.Substring(0, lastIndex + 1) + "{id}";
                list.Add(new KeyValuePair<string, string>("key", key));
                string id = referrer.Substring(lastIndex + 1);
                list.Add(new KeyValuePair<string, string>("id", id));
                return list;
            }

            list.Add(new KeyValuePair<string, string>("key", referrer));
            return list;
        }

        public HttpResponseMessage Create(object value, HttpRequestMessage request)
        {
            string referrer = request.Headers.Referrer.AbsolutePath;
            string[] ss = referrer.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (ss[ss.Length - 1] == "Create")   // .../Create
            {
                string collection = ss[ss.Length - 2];
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(value.ToString());
                JsonService service = new JsonService(request.GetQueryNameValuePairs());
                service.CreateEx(obj, collection, out string json);
                return CreateHttpResponseMessage(json, request);
            }

            throw new UriFormatException(referrer);
        }

        public HttpResponseMessage Create(string entity, object value, HttpRequestMessage request)
        {
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(value.ToString());
            JsonService service = new JsonService(request.GetQueryNameValuePairs());
            service.Create(obj, entity, out string json);

            return CreateHttpResponseMessage(json, request);
        }

        public void Delete(HttpRequestMessage request)
        {
            string referrer = request.Headers.Referrer.AbsolutePath;
            string[] ss = referrer.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (ss[ss.Length - 2] == "Delete") // .../Delete/1
            {
                string collection = ss[ss.Length - 3];
                string id = ss[ss.Length - 1];
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(string.Format("{{\"id\":{0}}}", id));
                new JsonService(request.GetQueryNameValuePairs()).DeleteEx(obj, collection);
                return;
            }

            throw new UriFormatException(referrer);
        }

        public void Delete(string entity, object value, HttpRequestMessage request)
        {
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(value.ToString());
            new JsonService(request.GetQueryNameValuePairs()).Delete(obj, entity);
        }

        public void Update(object value, HttpRequestMessage request)
        {
            string referrer = request.Headers.Referrer.AbsolutePath;
            string[] ss = referrer.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (ss[ss.Length - 2] == "Edit") // .../Edit/1
            {
                string collection = ss[ss.Length - 3];
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(value.ToString());
                new JsonService(request.GetQueryNameValuePairs()).UpdateEx(obj, collection);
                return;
            }

            throw new UriFormatException(referrer);
        }

        public void Update(string entity, object value, HttpRequestMessage request)
        {
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(value.ToString());
            new JsonService(request.GetQueryNameValuePairs()).Update(obj, entity);
        }

        private static HttpResponseMessage CreateHttpResponseMessage(string json, HttpRequestMessage request)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, request.GetResponseEncoding(), "application/json")
            };
            return response;
        }



    }
}