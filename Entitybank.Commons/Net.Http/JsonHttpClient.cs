using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace XData.Net.Http
{
    public class JsonHttpClient
    {
        protected HttpClient HttpClient = new HttpClient();

        public JsonHttpClient(string baseAddress)
        {
            HttpClient.BaseAddress = new Uri(baseAddress);
        }

        public JsonHttpClient(Uri baseAddress)
        {
            HttpClient.BaseAddress = baseAddress;
        }

        public string Get(string relativeUri)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = HttpClient.GetAsync(relativeUri).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<string> GetAsync(string relativeUri)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await HttpClient.GetAsync(relativeUri);
            return await response.Content.ReadAsStringAsync();
        }

        public string Put(string relativeUri, string value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/json");
            var response = HttpClient.PutAsync(relativeUri, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<string> PutAsync(string relativeUri, string value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");
            var response = await HttpClient.PutAsync(relativeUri, content);
            return await response.Content.ReadAsStringAsync();
        }

        public string Post(string relativeUri, string value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync(relativeUri, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<string> PostAsync(string relativeUri, string value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync(relativeUri, content);
            return await response.Content.ReadAsStringAsync();
        }

        public string Delete(string relativeUri)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = HttpClient.DeleteAsync(relativeUri).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<string> DeleteAsync(string relativeUri)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await HttpClient.DeleteAsync(relativeUri);
            return await response.Content.ReadAsStringAsync();
        }

        public string Delete(string relativeUri, string value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, relativeUri)
            {
                Content = content
            };

            var response = HttpClient.SendAsync(request).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<string> DeleteAsync(string relativeUri, string value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, relativeUri)
            {
                Content = content
            };

            var response = await HttpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }


    }
}
