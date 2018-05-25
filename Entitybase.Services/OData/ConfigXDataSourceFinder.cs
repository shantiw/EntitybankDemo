using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XData.Data.Helpers;

namespace XData.Data.OData
{
    public class ConfigXDataSourceFinder : XDataSourceFinder
    {
        protected readonly static IEnumerable<string> DefaultExcludedAttributes = new List<string>() { "entity", "select", "filter", "orderby", "pageSize", "pageIndex", "count", "default", "expand" };

        protected readonly IEnumerable<string> ExcludedAttributes;
        protected readonly string Separator;

        protected readonly XmlProvider XmlProvider;

        public ConfigXDataSourceFinder(string name, string excludedAttributes, string separator) : base(name)
        {
            Separator = separator;

            string[] excluded = new string[0];
            if (!string.IsNullOrWhiteSpace(excludedAttributes))
            {
                excluded = excludedAttributes.Split(',');
                for (int i = 0; i < excluded.Length; i++)
                {
                    excluded[i] = excluded[i].Trim();
                }
            }
            ExcludedAttributes = excluded.Union(DefaultExcludedAttributes);

            XmlProvider = new DirectoryXmlProvider(Path.Combine(Name, "datasources"), ".config", ExcludedAttributes, Separator);
        }

        public override XElement Find(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            IEnumerable<XElement> elements = XmlProvider.FindElements(keyValues);
            if (elements == null) return null;

            int count = elements.Count();
            if (count == 0) throw new ApplicationException(string.Format("Not found the datasource by {0}.", GetKeyValueString(keyValues)));
            if (count > 1) throw new ApplicationException(string.Format("Ambiguous datasources have been found by {0}.", GetKeyValueString(keyValues)));

            return new XElement(elements.First());
        }

        private string GetKeyValueString(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            return string.Join(",", keyValues.Select(x => string.Format("{0}:\"{1}\"", x.Key, x.Value)));
        }


    }
}
