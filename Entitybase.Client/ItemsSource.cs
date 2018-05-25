using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XData.Http.Client;

namespace XData.Client.Models
{
    public class ItemsSource
    {
        public string Name { get; set; }

        public IEnumerable<XElement> Items
        {
            get => XmlClient.GetCollection(new Dictionary<string, string>() { { "name", Name } }).Elements();
        }

        private XmlClient XmlClient;

        public ItemsSource()
        {
            string baseAddress = ConfigurationManager.AppSettings["BaseAddress"];
            XmlClient = XmlClientManager.GetClient(baseAddress);
        }
    }
}
