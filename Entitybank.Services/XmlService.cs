using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XData.Data.OData;
using XData.Data.Schema;
using XData.Data.Xml;

namespace XData.Data.Services
{
    public partial class XmlService : DataService
    {
        public XmlDatabase Database { get; private set; }

        protected readonly XmlModifier Modifier;

        public XmlService(IEnumerable<KeyValuePair<string, string>> keyValues)
            : this(ConfigurationManager.ConnectionStrings[0].Name, keyValues)
        {
        }

        protected XmlService(string name, IEnumerable<KeyValuePair<string, string>> keyValues)
            : base(name, keyValues)
        {
            Modifier = XmlModifier.Create(name);
            Database = Modifier.Database;

            RegisterEvents();
        }

        public static DateTime GetNow()
        {
            string name = ConfigurationManager.ConnectionStrings[0].Name;
            return ODataQuerier.GetNow(name);
        }

        public static DateTime GetUtcNow()
        {
            string name = ConfigurationManager.ConnectionStrings[0].Name;
            return ODataQuerier.GetUtcNow(name);
        }

        protected const string XSI = "http://www.w3.org/2001/XMLSchema-instance";

        public XElement Get()
        {
            DataSource dataSource = new DataSourceCreator(Name, KeyValues).Create();

            ODataQuerier<XElement> oDataQuerier = ODataQuerier<XElement>.Create(Name, Schema);

            if (dataSource.GetType() == typeof(PagingDataSource))
            {
                PagingDataSource ds = dataSource as PagingDataSource;

                IEnumerable<XElement> xCollection;
                XElement xsd;
                if (ds.Expands == null || ds.Expands.Length == 0)
                {
                    xCollection = oDataQuerier.GetCollection(ds.Entity, ds.Select, ds.Filter, ds.Orderby, ds.Skip, ds.Top, GetParameterValues(ds.Parameters), ds.Parameters, out xsd);
                }
                else
                {
                    xCollection = oDataQuerier.GetCollection(ds.Entity, ds.Select, ds.Filter, ds.Orderby, ds.Skip, ds.Top, ds.Expands, GetParameterValues(ds.Parameters), ds.Parameters, out xsd);
                }
                string collection = GetCollection(Schema, ds.Entity);
                XElement element = new XElement(collection, xCollection);
                element.SetAttributeValue(XNamespace.Xmlns + "i", XSI);

                int count = oDataQuerier.Count(ds.Entity, ds.Filter, GetParameterValues(ds.Parameters), ds.Parameters);

                return Pack(element, count, xsd);
            }
            else if (dataSource.GetType() == typeof(CollectionDataSource))
            {
                CollectionDataSource ds = dataSource as CollectionDataSource;

                IEnumerable<XElement> xCollection;
                XElement xsd;
                if (ds.Expands == null || ds.Expands.Length == 0)
                {
                    xCollection = oDataQuerier.GetCollection(ds.Entity, ds.Select, ds.Filter, ds.Orderby, GetParameterValues(ds.Parameters), ds.Parameters, out xsd);
                }
                else
                {
                    xCollection = oDataQuerier.GetCollection(ds.Entity, ds.Select, ds.Filter, ds.Orderby, ds.Expands, GetParameterValues(ds.Parameters), ds.Parameters, out xsd);
                }
                string collection = GetCollection(Schema, ds.Entity);
                XElement element = new XElement(collection, xCollection);
                element.SetAttributeValue(XNamespace.Xmlns + "i", XSI);

                return Pack(element, null, xsd);
            }
            else if (dataSource.GetType() == typeof(DefaultGetterDataSource))
            {
                DefaultGetterDataSource ds = dataSource as DefaultGetterDataSource;
                XElement element = oDataQuerier.GetDefault(dataSource.Entity, ds.Select, out XElement xsd);
                element.SetAttributeValue(XNamespace.Xmlns + "i", XSI);
                return Pack(element, null, xsd);
            }
            else if (dataSource.GetType() == typeof(CountDataSource))
            {
                CountDataSource ds = dataSource as CountDataSource;
                int count = oDataQuerier.Count(ds.Entity, ds.Filter, GetParameterValues(ds.Parameters), ds.Parameters);
                return new XElement("Count", count);
            }

            throw new NotSupportedException(dataSource.GetType().ToString());
        }

        protected static IEnumerable<KeyValuePair<string, string>> GetParameterValues(IReadOnlyDictionary<string, object> parameters)
        {
            List<KeyValuePair<string, string>> parameterValues = new List<KeyValuePair<string, string>>();
            foreach (KeyValuePair<string, object> pair in parameters)
            {
                parameterValues.Add(new KeyValuePair<string, string>(pair.Key, null));
            }
            return parameterValues;
        }

        protected static string GetCollection(XElement schema, string entity)
        {
            XElement entitySchema = schema.Elements(SchemaVocab.Entity).FirstOrDefault(x => x.Attribute(SchemaVocab.Name).Value == entity);
            return entitySchema.Attribute(SchemaVocab.Collection).Value;
        }

        protected static XElement Pack(XElement element, int? count, XElement xsd)
        {
            if (count == null && xsd == null) return element;

            XElement xml = new XElement("xml");
            if (xsd != null) xml.Add(new XElement("schema", xsd));
            xml.Add(new XElement("element", element));
            if (count != null) xml.Add(new XElement("count", count));
            return xml;
        }

        public void Create(XElement element, out XElement keys)
        {
            keys = Modifier.CreateAndReturnKeys(element, Schema);
        }

        public void Delete(XElement element)
        {
            Modifier.Delete(element, Schema);
        }

        public void Update(XElement element)
        {
            Modifier.Update(element, Schema);
        }


    }
}
