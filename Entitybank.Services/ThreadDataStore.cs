using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XData.Data.OData;

namespace XData.Data.Services
{
    public class RequestInfo
    {
        public string HttpMethod { get; set; }
        public string Url { get; set; }
        public string UrlReferrer { get; set; }
        public string UserAgent { get; set; }
        public string UserHostAddress { get; set; }
        public string Accept { get; set; }
    }

    public static class ThreadDataStore
    {
        private static ThreadLocal<RequestInfo> LocalRequest;

        public static RequestInfo RequestInfo
        {
            get => LocalRequest.Value;
            set { LocalRequest = new ThreadLocal<RequestInfo>(() => value); }
        }

        internal static XElement CreateSecurityEntry(this RequestInfo requestInfo, ODataQuerier<XElement> querier)
        {
            XElement securityEntry = new XElement("SecurityEntry");
            securityEntry.SetElementValue("HttpMethod", requestInfo.HttpMethod);
            securityEntry.SetElementValue("Url", requestInfo.Url);
            securityEntry.SetElementValue("UrlReferrer", requestInfo.UrlReferrer);
            securityEntry.SetElementValue("UserAgent", requestInfo.UserAgent);
            securityEntry.SetElementValue("UserHostAddress", requestInfo.UserHostAddress);
            securityEntry.SetElementValue("Accept", requestInfo.Accept);
            XElement user = GetCurrentUser(querier);
            if (user != null)
            {
                securityEntry.SetElementValue("CreatedUserId", user.Element("Id").Value);
                securityEntry.SetElementValue("CreatorName", user.Element("Name").Value);
            }
            return securityEntry;
        }

        internal static XElement GetCurrentUser(ODataQuerier<XElement> querier)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                string userName = Thread.CurrentPrincipal.Identity.Name;
                IEnumerable<XElement> elements = querier.GetCollection("User", "Id,UserName,Name,GenderName", "LoweredUserName eq @p1", null,
                    new Dictionary<string, object>() { { "@p1", userName.ToLower() } });
                if (elements.Count() == 0) return null;
                return elements.First();
            }
            return null;
        }


    }

}
