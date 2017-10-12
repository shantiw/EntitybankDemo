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
            string result = response.Content.ReadAsStringAsync().Result;
            return ReturnResult(response, result);
        }

        public async Task<string> GetAsync(string relativeUri)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await HttpClient.GetAsync(relativeUri);
            string result = await response.Content.ReadAsStringAsync();
            return ReturnResult(response, result);
        }

        public string Put(string relativeUri, string value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpContent content = new StringContent(value.ToString(), Encoding.UTF8, "application/json");
            var response = HttpClient.PutAsync(relativeUri, content).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return ReturnResult(response, result);
        }

        public async Task<string> PutAsync(string relativeUri, string value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");
            var response = await HttpClient.PutAsync(relativeUri, content);
            string result = await response.Content.ReadAsStringAsync();
            return ReturnResult(response, result);
        }

        public string Post(string relativeUri, string value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync(relativeUri, content).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return ReturnResult(response, result);
        }

        public async Task<string> PostAsync(string relativeUri, string value)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync(relativeUri, content);
            string result = await response.Content.ReadAsStringAsync();
            return ReturnResult(response, result);
        }

        public string Delete(string relativeUri)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = HttpClient.DeleteAsync(relativeUri).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return ReturnResult(response, result);
        }

        public async Task<string> DeleteAsync(string relativeUri)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await HttpClient.DeleteAsync(relativeUri);
            string result = await response.Content.ReadAsStringAsync();
            return ReturnResult(response, result);
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
            string result = response.Content.ReadAsStringAsync().Result;
            return ReturnResult(response, result);
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
            string result = await response.Content.ReadAsStringAsync();
            return ReturnResult(response, result);
        }

        protected static string ReturnResult(HttpResponseMessage response, string result)
        {
            return result;
        }


    }
}
