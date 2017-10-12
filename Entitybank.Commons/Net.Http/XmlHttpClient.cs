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

        public XmlHttpClient(string baseAddress)
        {
            HttpClient.BaseAddress = new Uri(baseAddress);
        }

        public XmlHttpClient(Uri baseAddress)
        {
            HttpClient.BaseAddress = baseAddress;
        }

        public XElement Get(string relativeUri)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

            var response = HttpClient.GetAsync(relativeUri).Result;
            string text = response.Content.ReadAsStringAsync().Result;
            XElement result = XElement.Parse(text);
            return ReturnResult(response, result);
        }

        public async Task<XElement> GetAsync(string relativeUri)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

            var response = await HttpClient.GetAsync(relativeUri);
            string text = await response.Content.ReadAsStringAsync();
            XElement result = XElement.Parse(text);
            return ReturnResult(response, result);
        }

        public XElement Put(string relativeUri, XElement value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/xml");
            var response = HttpClient.PutAsync(relativeUri, content).Result;
            string text = response.Content.ReadAsStringAsync().Result;
            XElement result = XElement.Parse(text);
            return ReturnResult(response, result);
        }

        public async Task<XElement> PutAsync(string relativeUri, XElement value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/xml");
            var response = await HttpClient.PutAsync(relativeUri, content);
            string text = await response.Content.ReadAsStringAsync();
            XElement result = XElement.Parse(text);
            return ReturnResult(response, result);
        }

        public XElement Post(string relativeUri, XElement value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/xml");
            var response = HttpClient.PostAsync(relativeUri, content).Result;
            string text = response.Content.ReadAsStringAsync().Result;
            XElement result = XElement.Parse(text);
            return ReturnResult(response, result);
        }

        public async Task<XElement> PostAsync(string relativeUri, XElement value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/xml");
            var response = await HttpClient.PostAsync(relativeUri, content);
            string text = await response.Content.ReadAsStringAsync();
            XElement result = XElement.Parse(text);
            return ReturnResult(response, result);
        }

        public XElement Delete(string relativeUri)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

            var response = HttpClient.DeleteAsync(relativeUri).Result;
            string text = response.Content.ReadAsStringAsync().Result;
            XElement result = XElement.Parse(text);
            return ReturnResult(response, result);
        }

        public async Task<XElement> DeleteAsync(string relativeUri)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

            var response = await HttpClient.DeleteAsync(relativeUri);
            string text = await response.Content.ReadAsStringAsync();
            XElement result = XElement.Parse(text);
            return ReturnResult(response, result);
        }

        public XElement Delete(string relativeUri, XElement value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/xml");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, relativeUri)
            {
                Content = content
            };

            var response = HttpClient.SendAsync(request).Result;
            string text = response.Content.ReadAsStringAsync().Result;
            XElement result = XElement.Parse(text);
            return ReturnResult(response, result);
        }

        public async Task<XElement> DeleteAsync(string relativeUri, XElement value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/xml");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, relativeUri)
            {
                Content = content
            };

            var response = await HttpClient.SendAsync(request);
            string text = await response.Content.ReadAsStringAsync();
            XElement result = XElement.Parse(text);
            return ReturnResult(response, result);
        }

        protected static XElement ReturnResult(HttpResponseMessage response, XElement result)
        {
            if (response.IsSuccessStatusCode)
            {
                if (result.Name.LocalName == "Error")
                {
                    result.SetAttributeValue("HttpStatusCode", response.StatusCode);
                }
            }
            else
            {
                result.SetAttributeValue("HttpStatusCode", response.StatusCode);
            }

            return result;
        }


    }
}
