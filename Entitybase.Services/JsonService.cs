using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XData.Data.Dynamic;
using XData.Data.OData;
using XData.Data.Modification;
using XData.Data.Schema;
using System.Xml.Linq;
using System.Dynamic;
using XData.Data.Objects;

namespace XData.Data.Services
{
    // name: ConnectionStringName
    public partial class JsonService : DataService
    {
        public Database<dynamic> Database { get; private set; }
        public Modifier<dynamic> Modifier { get; private set; }

        protected ODataQuerier<string> ODataQuerier;

        public JsonService(IEnumerable<KeyValuePair<string, string>> keyValues)
            : this(ConfigurationManager.ConnectionStrings[0].Name, keyValues)
        {
        }

        public JsonService(string name, IEnumerable<KeyValuePair<string, string>> keyValues)
            : base(name, keyValues)
        {
            Modifier = DynModifierFactory.Create(name);
            Database = Modifier.Database;
            ODataQuerier = ODataQuerier<string>.Create(Name, Schema);
        }

        public DateTime GetNow()
        {
            return ODataQuerier.GetNow();
        }

        public DateTime GetUtcNow()
        {
            return ODataQuerier.GetUtcNow();
        }

        public string Get()
        {
            DataSource dataSource = new DataSourceCreator(Name, KeyValues).Create();
            if (dataSource.GetType() == typeof(PagingDataSource))
            {
                PagingDataSource ds = dataSource as PagingDataSource;

                IEnumerable<string> jsonCollection;
                if (ds.Expands == null || ds.Expands.Length == 0)
                {
                    jsonCollection = ODataQuerier.GetPagingCollection(ds.Entity, ds.Select, ds.Filter, ds.Orderby, ds.Skip, ds.Top, ds.Parameters);
                }
                else
                {
                    jsonCollection = ODataQuerier.GetPagingCollection(ds.Entity, ds.Select, ds.Filter, ds.Orderby, ds.Skip, ds.Top, ds.Expands, ds.Parameters);
                }
                string json = string.Format("[{0}]", string.Join(",", jsonCollection));

                int count = ODataQuerier.Count(ds.Entity, ds.Filter, ds.Parameters);
                json = string.Format("{{\"@count\":{0},\"value\":{1}}}", count, json);
                return json;
            }
            else if (dataSource.GetType() == typeof(CollectionDataSource))
            {
                CollectionDataSource ds = dataSource as CollectionDataSource;

                IEnumerable<string> jsonCollection;
                if (ds.Expands == null || ds.Expands.Length == 0)
                {
                    jsonCollection = ODataQuerier.GetCollection(ds.Entity, ds.Select, ds.Filter, ds.Orderby, ds.Parameters);
                }
                else
                {
                    jsonCollection = ODataQuerier.GetCollection(ds.Entity, ds.Select, ds.Filter, ds.Orderby, ds.Expands, ds.Parameters);
                }
                return string.Format("[{0}]", string.Join(",", jsonCollection));
            }
            else if (dataSource.GetType() == typeof(DefaultGetterDataSource))
            {
                DefaultGetterDataSource ds = dataSource as DefaultGetterDataSource;
                return ODataQuerier.GetDefault(dataSource.Entity, ds.Select);
            }
            else if (dataSource.GetType() == typeof(CountDataSource))
            {
                CountDataSource ds = dataSource as CountDataSource;
                int count = ODataQuerier.Count(ds.Entity, ds.Filter, ds.Parameters);
                return string.Format("{{\"Count\": {0}}}", count);
            }

            throw new NotSupportedException(dataSource.GetType().ToString());
        }

        public void CreateEx(dynamic obj, string collection, out string keys)
        {
            string entity = Schema.GetEntitySchemaByCollection(collection).Attribute(SchemaVocab.Name).Value;
            Create(obj, entity, out keys);
        }

        public void Create(dynamic obj, string entity, out string keys)
        {
            Modifier.Create(obj, entity, Schema, out IEnumerable<Dictionary<string, object>> result);
            keys = result.CreateReturnKeysToJson();
        }

        public void DeleteEx(dynamic obj, string collection)
        {
            string entity = Schema.GetEntitySchemaByCollection(collection).Attribute(SchemaVocab.Name).Value;
            XElement keySchema = Schema.GetKeySchema(entity);
            if (keySchema.Elements(SchemaVocab.Property).Count() != 1)
                throw new NotSupportedException(string.Join(",", keySchema.Elements(SchemaVocab.Property).Select(x => x.Attribute(SchemaVocab.Name).Value)));

            XElement key = keySchema.Elements(SchemaVocab.Property).First();
            string keyName = key.Attribute(SchemaVocab.Name).Value;
            string dataType = key.Attribute(SchemaVocab.DataType).Value;

            obj[keyName] = obj["id"];
            obj.Remove("id");

            Delete(obj, entity);
        }

        public void Delete(dynamic obj, string entity)
        {
            Modifier.Delete(obj, entity, Schema);
        }

        public void UpdateEx(dynamic obj, string collection)
        {
            string entity = Schema.GetEntitySchemaByCollection(collection).Attribute(SchemaVocab.Name).Value;
            Update(obj, entity);
        }

        public void Update(dynamic obj, string entity)
        {
            Modifier.Update(obj, entity, Schema);
        }


    }
}
