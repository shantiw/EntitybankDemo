using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XData.Data.OData
{
    // name: ConnectionStringName
    public class XDataSourceProvider
    {
        private static Dictionary<string, XDataSourceFinder> Cache = new Dictionary<string, XDataSourceFinder>();
        private static object LockObj = new object();

        public void Update(string name)
        {
            lock (LockObj)
            {
                Cache[name] = CreateXDataSourceFinder(name);
            }
        }

        protected XDataSourceFinder CreateXDataSourceFinder(string name)
        {
            return new ConfigXDataSourceFinder(name, string.Empty, "|");
        }

        public XElement Get(string name, IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            if (!Cache.ContainsKey(name))
            {
                Update(name);
            }

            XDataSourceFinder finder = Cache[name];
            return finder.Find(keyValues);
        }


    }

}
