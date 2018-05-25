using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XData.Net.Http;

namespace XData.Http.Client
{
    public static class XmlHttpClientManager
    {       
        private static readonly Dictionary<string, XmlHttpClient> ApiClients = new Dictionary<string, XmlHttpClient>();

        public static XmlHttpClient GetClient(string baseAddress)
        {
            if (!ApiClients.ContainsKey(baseAddress))
            {
                ApiClients[baseAddress] = new XmlHttpClient(baseAddress);
            }

            return ApiClients[baseAddress];
        }
    }
}
