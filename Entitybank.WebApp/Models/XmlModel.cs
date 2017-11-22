using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Xml.Linq;
using XData.Data.Services;

namespace XData.Web.Models
{
    internal class XmlModel
    {
        public HttpResponseMessage GetNow(HttpRequestMessage request)
        {
            const string FORMAT = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK";

            XmlService service = new XmlService(request.GetQueryNameValuePairs());
            DateTime now = service.GetNow();
            XElement element = new XElement("now", now.ToString(FORMAT));
            return CreateHttpResponseMessage(element, request);
        }

        public HttpResponseMessage GetUtcNow(HttpRequestMessage request)
        {
            const string FORMAT = "yyyy-MM-ddTHH:mm:ss.FFFFFFFZ";

            XmlService service = new XmlService(request.GetQueryNameValuePairs());
            DateTime utcNow = service.GetUtcNow();
            XElement element = new XElement("utcnow", utcNow.ToString(FORMAT));
            return CreateHttpResponseMessage(element, request);
        }

        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            IEnumerable<KeyValuePair<string, string>> nameValues = request.GetNameValues();
            XElement element = new XmlService(nameValues).Get();

            return CreateHttpResponseMessage(element, request);
        }

        public HttpResponseMessage Create(XElement value, HttpRequestMessage request)
        {
            XmlService service = new XmlService(request.GetQueryNameValuePairs());
            service.Create(value, out XElement element);

            return CreateHttpResponseMessage(element, request);
        }

        public void Delete(XElement value, HttpRequestMessage request)
        {
            new XmlService(request.GetQueryNameValuePairs()).Delete(value);
        }

        public void Update(XElement value, HttpRequestMessage request)
        {
            new XmlService(request.GetQueryNameValuePairs()).Update(value);
        }

        private static HttpResponseMessage CreateHttpResponseMessage(XElement element, HttpRequestMessage request)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(element.ToString(), request.GetResponseEncoding(), "application/xml")
            };
            return response;
        }


    }
}