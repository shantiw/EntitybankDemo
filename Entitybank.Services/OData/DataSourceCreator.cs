using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using XData.Data.Helpers;

namespace XData.Data.OData
{
    internal class DataSourceCreator
    {
        protected XElement Datasource;

        protected Dictionary<string, string> Variables = new Dictionary<string, string>();
        protected Dictionary<string, object> Parameters = new Dictionary<string, object>();

        public DataSourceCreator(string name, IEnumerable<KeyValuePair<string, string>> keyValues)
            : this(new XDataSourceProvider().Get(name, keyValues), keyValues)
        {
        }

        protected DataSourceCreator(XElement datasource, IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            Datasource = datasource;

            //
            foreach (KeyValuePair<string, string> pair in keyValues)
            {
                Variables.Add(pair.Key, pair.Value);
            }

            //
            foreach (XElement dict in datasource.Elements("dict"))
            {
                string name = dict.Attribute("name").Value;
                string key = Variables[name];

                XElement pair = dict.Elements("pair").FirstOrDefault(x => x.Attribute("key").Value == key);
                if (pair == null)
                {
                    Variables[name] = string.Empty;
                }
                else
                {
                    Variables[name] = pair.Attribute("value").Value;
                }
            }

            //
            foreach (XElement param in datasource.Elements("param"))
            {
                string name = param.Attribute("name").Value;
                string value = param.Attribute("value").Value;
                string dataType = param.Attribute("dataType").Value;

                value = Compute(value);

                object oValue;
                Type type = Type.GetType(dataType);
                if (type == typeof(string))
                {
                    oValue = value;
                }
                else if (string.IsNullOrWhiteSpace(value))
                {
                    oValue = null;
                }
                else if (type == typeof(DateTime))
                {
                    oValue = DateTime.Parse(value);
                }
                else if (type == typeof(Guid))
                {
                    oValue = Guid.Parse(value);
                }
                else if (type == typeof(bool))
                {
                    if (value == "true" || value == "1")
                    {
                        oValue = true;
                    }
                    else if (value == "false" || value == "0")
                    {
                        oValue = false;
                    }
                    else
                    {
                        oValue = bool.Parse(value); // throw
                    }
                }
                else
                {
                    oValue = Convert.ChangeType(value, type);
                }
                Parameters.Add(name, oValue);
            }
        }

        public virtual DataSource Create()
        {
            string entity = Datasource.Attribute("entity").Value;
            string select = GetSelect(Datasource);
            string filter = GetFilter(Datasource);

            XAttribute xAttribute = Datasource.Attribute("count");
            if (xAttribute != null)
            {
                return new CountDataSource() { Entity = entity, Filter = filter, Parameters = Parameters };
            }

            xAttribute = Datasource.Attribute("default");
            if (xAttribute != null)
            {
                return new DefaultGetterDataSource() { Entity = entity, Select = select };
            }

            SomeDataSource someDataSource;
            string orderby = GetOrderby(Datasource);
            string pageIndex = GetPageIndex();
            string pageSize = GetPageSize();
            if (pageSize == null && pageIndex == null)
            {
                someDataSource = new CollectionDataSource() { Entity = entity, Select = select, Filter = filter, Orderby = orderby };
            }
            else
            {
                long lPageIndex = long.Parse(pageIndex ?? "0");
                long lPageSize = long.Parse(pageSize ?? "0");
                someDataSource = new PagingDataSource() { Entity = entity, Select = select, Filter = filter, Orderby = orderby, PageIndex = lPageIndex, PageSize = lPageSize };
            }

            someDataSource.Expands = GetExpands(Datasource);
            someDataSource.Parameters = Parameters;
            return someDataSource;
        }

        protected Expand[] GetExpands(XElement element)
        {
            List<Expand> list = new List<Expand>();

            foreach (XElement xExpand in element.Elements("expand"))
            {
                string property = xExpand.Attribute("property").Value;
                string select = GetSelect(xExpand);
                string filter = GetFilter(xExpand);
                string orderby = GetOrderby(xExpand);

                Expand expand = new Expand(property, select, filter, orderby)
                {
                    Children = GetExpands(xExpand)
                };

                list.Add(expand);
            }

            return list.ToArray();
        }

        protected virtual string GetSelect(XElement element)
        {
            string select = GetValue("select", element);
            if (select == null) return null;
            if (select.Trim() == "*") select = null;

            return select;
        }

        protected virtual string GetFilter(XElement element)
        {
            XElement filter = element.Element("filter");
            if (filter == null)
            {
                string val = GetValue("filter", element);
                return HandleNullParameter(val);
            }

            List<string> items = new List<string>();
            foreach (XElement item in filter.Elements())
            {
                string val = Compute(item.Attribute("value").Value);
                val = HandleNullParameter(val);
                if (val != null) items.Add(val);
            }

            string result = string.Join(" and ", items);
            return (result == string.Empty) ? null : result;
        }

        protected string HandleNullParameter(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;

            foreach (KeyValuePair<string, object> pair in Parameters.Where(p => p.Value == null))
            {
                if (filter.Contains(pair.Key)) return null;
            }
            return filter;
        }

        protected virtual string GetOrderby(XElement element)
        {
            XElement orderby = element.Element("orderby");
            if (orderby == null)
            {
                return GetValue("orderby", element);
            }

            List<string> items = new List<string>();
            foreach (XElement item in orderby.Elements())
            {
                string val = Compute(item.Attribute("value").Value);
                if (val != null) items.Add(val);
            }

            string result = string.Join(",", items);
            return (result == string.Empty) ? null : result;
        }

        protected virtual string GetPageIndex()
        {
            string pageIndex = GetValue("pageIndex", Datasource);

            return pageIndex;
        }

        protected virtual string GetPageSize()
        {
            string pageSize = GetValue("pageSize", Datasource);

            return pageSize;
        }

        protected virtual string GetValue(string name, XElement element)
        {
            XAttribute attr = element.Attribute(name);
            if (attr == null) return null;

            return Compute(attr.Value);
        }

        private string Compute(string value)
        {
            if (!value.Contains("{{")) return value;

            bool anyEmpty = false;
            string pattern = @"{{.+?}}";
            string result = Regex.Replace(value, pattern, new MatchEvaluator(m =>
            {
                string val = m.Value;
                val = val.Substring(2, val.Length - 4);
                val = Eval(val).ToString();
                if (string.IsNullOrEmpty(val)) anyEmpty = true;
                return val;
            }));

            return anyEmpty ? null : result;
        }

        private string _var_str = null;

        private object Eval(string expr)
        {
            if (Variables.ContainsKey(expr)) return Variables[expr];

            if (_var_str == null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> pair in Variables)
                {
                    sb.Append(string.Format("var {0}=\"{1}\";", pair.Key, pair.Value));
                }
                _var_str = sb.ToString();
            }

            object obj = new JavaScriptEvaluator().Eval(_var_str + expr);
            return obj;
        }


    }
}
