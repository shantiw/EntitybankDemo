using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XData.Data.Objects;
using XData.Data.OData;
using XData.Data.Schema;

namespace XData.Data.Services
{
    // name: ConnectionStringName
    public abstract class DataService
    {
        protected readonly string Name;
        protected readonly IEnumerable<KeyValuePair<string, string>> KeyValues;

        protected readonly XElement Schema;

        protected DataService(string name, IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            Name = name;
            KeyValues = keyValues;

            Schema = GetSchema(name, keyValues);
        }

        public bool IsUnique(string entity, string property, object value, params object[] parameters)
        {
            List<KeyValuePair<string, object>> propertyValues = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(property, value)
            };

            List<KeyValuePair<string, object>> excludedKey = new List<KeyValuePair<string, object>>();
            XElement[] keyProperties = Schema.GetKeySchema(entity).Elements().ToArray();
            for (int i = 0; i < keyProperties.Length; i++)
            {
                string keyPropertyName = keyProperties[i].Attribute(SchemaVocab.Name).Value;
                excludedKey.Add(new KeyValuePair<string, object>(keyPropertyName, parameters[i]));
            }

            return IsUnique(entity, propertyValues, excludedKey);
        }

        public bool IsUnique(string entity, IEnumerable<KeyValuePair<string, object>> propertyValues, IEnumerable<KeyValuePair<string, object>> excludedKey)
        {
            string filter = GenerateFilter(propertyValues, out IReadOnlyDictionary<string, object> parameters);
            string select = string.Join(",", excludedKey.Select(p => p.Key));

            ODataQuerier<XElement> oDataQuerier = ODataQuerier<XElement>.Create(Name, Schema);
            IEnumerable<XElement> result = oDataQuerier.GetCollection(entity, select, filter, null, parameters);

            int count = result.Count();
            if (count == 0) return true;
            if (count > 1) return false;

            XElement element = result.First();
            foreach (KeyValuePair<string, object> pair in excludedKey)
            {
                object obj = pair.Value;

                string value;
                if (obj.GetType() == typeof(bool))
                {
                    value = ((bool)obj) ? "true" : "false";
                }
                else if (obj.GetType() == typeof(DateTime))
                {
                    value = new DotNETDateFormatter().Format((DateTime)obj);
                }
                else
                {
                    value = obj.ToString();
                }
                if (element.Element(pair.Key).Value == value) continue;

                return false;
            }
            return true;
        }

        // overload
        public bool Exists(string entity, string property, object value)
        {
            return Exists(entity, new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(property, value) });
        }

        public bool Exists(string entity, IEnumerable<KeyValuePair<string, object>> propertyValues)
        {
            string filter = GenerateFilter(propertyValues, out IReadOnlyDictionary<string, object> parameters);
            string select = propertyValues.First().Key;

            ODataQuerier<XElement> oDataQuerier = ODataQuerier<XElement>.Create(Name, Schema);
            IEnumerable<XElement> result = oDataQuerier.GetCollection(entity, select, filter, null, parameters);

            return result.Count() > 0;
        }

        private static string GenerateFilter(IEnumerable<KeyValuePair<string, object>> propertyValues, out IReadOnlyDictionary<string, object> parameters)
        {
            List<string> list = new List<string>();
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            KeyValuePair<string, object>[] arr = propertyValues.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                list.Add(string.Format("{0} eq @p{1}", arr[i].Key, i));
                paramDict.Add(string.Format("@p{0}", i), arr[i].Value);
            }
            parameters = paramDict;
            return string.Join(" and ", list);
        }

        protected static XElement GetSchema(string name, IEnumerable<KeyValuePair<string, string>> deltaKey)
        {
            SchemaProvider schemaProvider = new SchemaProvider(name);
            return schemaProvider.GetSchema(deltaKey);
        }


    }
}
