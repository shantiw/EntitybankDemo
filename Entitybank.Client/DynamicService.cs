using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XData.Data.Xml;

namespace XData.Http.Client
{
    public class DynamicService
    {
        protected readonly XmlHttpClient XmlHttpClient;
        protected readonly Func<dynamic> CreateDynamic;

        public DynamicService(string baseAddress, Func<dynamic> createDynamic = null)
        {
            XmlHttpClient = XmlHttpClientManager.GetClient(baseAddress);
            CreateDynamic = createDynamic;

            //System.Dynamic.ExpandoObject
            //System.Net.Http.HttpRequestException
            //new XmlConverter().ToDynamic()
        }

        public DateTime GetNow()
        {
            return DateTime.Now;
        }

        public DateTime GetUtcNow()
        {
            return DateTime.Now;
        }

        public int Count(dynamic parameters)
        {
            return 0;
        }

        public dynamic GetDefault(dynamic parameters, out string entity)
        {
            entity = null;
            return null;
        }

        public dynamic Find(dynamic parameters, out string entity)
        {
            entity = null;
            return null;
        }

        public IEnumerable<dynamic> GetCollection(dynamic parameters, out string entity)
        {
            entity = null;
            return null;
        }

        public IEnumerable<dynamic> GetPage(dynamic parameters, out string entity, out int count)
        {
            entity = null;
            count = 0;
            return null;
        }

        public dynamic Create(string entity, dynamic value)
        {
            return null;
        }

        public IEnumerable<dynamic> Create(string entity, IEnumerable<dynamic> value)
        {
            return null;
        }

        public void Delete(string entity, dynamic value)
        {
        }

        public void Delete(string entity, IEnumerable<dynamic> value)
        {
        }

        public void Update(string entity, dynamic value)
        {

        }

        public void Update(string entity, IEnumerable<dynamic> value)
        {

        }


    }
}
