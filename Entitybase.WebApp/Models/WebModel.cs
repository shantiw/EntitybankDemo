using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using XData.Data.Services;

namespace XData.Web.Models
{
    internal class WebModel
    {
        public bool IsUnique(string entity, string property, object value, HttpRequestBase request)
        {
            bool isUnique = false;

            string referrer = request.UrlReferrer.AbsolutePath;
            string[] ss = referrer.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (ss[ss.Length - 1] == "Create")   // .../Create
            {
                isUnique = !new JsonService(ToKeyValuePairs(request.QueryString)).Exists(entity, property, value);
            }
            else if (ss[ss.Length - 2] == "Edit") // .../Edit/1
            {
                string id = ss[ss.Length - 1];
                isUnique = new JsonService(ToKeyValuePairs(request.QueryString)).IsUnique(entity, property, value, id);
            }

            return isUnique;
        }

        private static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(NameValueCollection queryString)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            foreach (string key in queryString.Keys)
            {
                list.Add(new KeyValuePair<string, string>(key, queryString[key]));
            }
            return list;
        }


    }
}