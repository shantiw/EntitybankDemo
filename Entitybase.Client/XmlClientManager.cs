using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XData.Net.Http;

namespace XData.Http.Client
{
    public static class XmlClientManager
    {
        private static readonly Dictionary<string, XmlClient> XmlClients = new Dictionary<string, XmlClient>();

        public static XmlClient GetClient(string baseAddress)
        {
            if (!XmlClients.ContainsKey(baseAddress))
            {
                XmlClients[baseAddress] = new XmlClient(baseAddress);
            }

            return XmlClients[baseAddress];
        }
    }
}
