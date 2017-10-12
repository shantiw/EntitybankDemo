using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace System.Net.Http
{
    public static class HttpRequestMessageExtensions
    {
        public static string GetResponseMediaType(this HttpRequestMessage request)
        {
            IEnumerable<MediaTypeWithQualityHeaderValue> mediaTypes = request.Headers.Accept
                .Where(p => p.MediaType.Contains("/xml") || p.MediaType.Contains("/json"))
                .OrderByDescending(p => p.Quality ?? 1);
            if (mediaTypes.Count() > 0)
            {
                MediaTypeWithQualityHeaderValue mediaType = mediaTypes.First();
                if (mediaType.MediaType.Contains("/xml")) return "application/xml";
            }
            return "application/json";
        }

        private static EncodingInfo[] EncodingInfos = Encoding.GetEncodings();

        public static Encoding GetResponseEncoding(this HttpRequestMessage request)
        {
            IEnumerable<StringWithQualityHeaderValue> charsets = request.Headers.AcceptCharset.OrderByDescending(p => p.Quality ?? 1);
            foreach (StringWithQualityHeaderValue charset in charsets)
            {
                EncodingInfo info = EncodingInfos.FirstOrDefault(p => p.Name == charset.Value);
                if (info != null) return info.GetEncoding();
            }
            return Encoding.UTF8;
        }

        public static IEnumerable<KeyValuePair<string, string>> GetNameValues(this HttpRequestMessage request)
        {
            IEnumerable<KeyValuePair<string, string>> nameValues = request.GetQueryNameValuePairs();
            if (HttpContext.Current.Request.UrlReferrer == null) return nameValues;

            string referrer = HttpContext.Current.Request.UrlReferrer.AbsolutePath;
            if (string.IsNullOrWhiteSpace(referrer)) return nameValues;

            if (nameValues.Any(p => p.Key == "key")) return nameValues;

            string[] rArray = referrer.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string last = rArray.Last().ToLower();

            string key = null;
            string id = null;
            if (last == "index")
            {
                key = referrer.Substring(0, referrer.Length - last.Length - 1);
            }
            else if (last == "create")
            {
                key = referrer;
            }
            else if (rArray.Length > 1)
            {
                string prev = rArray[rArray.Length - 2];
                if (prev == "Edit" || prev == "Delete" || prev == "Details")
                {
                    id = last;
                    key = referrer.Substring(0, referrer.Length - last.Length) + "{id}";
                }
            }

            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>(nameValues);
            if (key != null) list.Add(new KeyValuePair<string, string>("key", key));
            if (id != null) list.Add(new KeyValuePair<string, string>("id", id));

            return list;
        }


    }
}
