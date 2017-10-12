using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XData.Data.OData
{
    // name: ConnectionStringName
    public abstract class XDataSourceFinder
    {
        protected readonly string Name;

        public XDataSourceFinder(string name)
        {
            Name = name;
        }

        public abstract XElement Find(IEnumerable<KeyValuePair<string, string>> keyValues);

    }
}
