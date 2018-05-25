using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XData.Net.Http
{
    public class XmlHttpClient
    {
        protected HttpClient HttpClient = new HttpClient();

        public XmlHttpClient(string baseAddress) : this(new Uri(baseAddress))
        {
        }

        public XmlHttpClient(Uri baseAddress)
        {
            HttpClient.BaseAddress = baseAddress;
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");
        }

        public XElement Get(string relativeUri)
        {
            var response = HttpClient.GetAsync(relativeUri).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return Parse(result);
        }

        public async Task<XElement> GetAsync(string relativeUri)
        {
            var response = await HttpClient.GetAsync(relativeUri);
            string result = await response.Content.ReadAsStringAsync();
            return Parse(result);
        }

        public XElement Post(string relativeUri, XElement value)
        {
            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/xml");
            var response = HttpClient.PostAsync(relativeUri, content).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return Parse(result);
        }

        public async Task<XElement> PostAsync(string relativeUri, XElement value)
        {
            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/xml");
            var response = await HttpClient.PostAsync(relativeUri, content);
            string result = await response.Content.ReadAsStringAsync();
            return Parse(result);
        }

        public XElement Put(string relativeUri, XElement value)
        {
            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/xml");
            var response = HttpClient.PutAsync(relativeUri, content).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return Parse(result);
        }

        public async Task<XElement> PutAsync(string relativeUri, XElement value)
        {
            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/xml");
            var response = await HttpClient.PutAsync(relativeUri, content);
            string result = await response.Content.ReadAsStringAsync();
            return Parse(result);
        }

        public XElement Delete(string relativeUri)
        {
            var response = HttpClient.DeleteAsync(relativeUri).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return Parse(result);
        }

        public async Task<XElement> DeleteAsync(string relativeUri)
        {
            var response = await HttpClient.DeleteAsync(relativeUri);
            string result = await response.Content.ReadAsStringAsync();
            return Parse(result);
        }

        public XElement Delete(string relativeUri, XElement value)
        {
            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/xml");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, relativeUri)
            {
                Content = content
            };

            var response = HttpClient.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return Parse(result);
        }

        public async Task<XElement> DeleteAsync(string relativeUri, XElement value)
        {
            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/xml");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, relativeUri)
            {
                Content = content
            };

            var response = await HttpClient.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            return XElement.Parse(result);
        }

        protected XElement Parse(string content)
        {
            // void
            if (content == string.Empty) return new XElement("Content");

            try
            {
                return XElement.Parse(content);
            }
            catch
            {
                return new XElement("Content", content);
            }
        }


    }
}
