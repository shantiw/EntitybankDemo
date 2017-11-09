using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XData.Data.Objects;
using XData.Data.OData;
using XData.Data.Schema;

namespace XData.Data.Services
{
    // name: ConnectionStringName
    public sealed class ConfigService
    {
        private PrimarySchemaProvider PrimarySchemaProvider = new PrimarySchemaProvider();
        private SchemaDeltaProvider SchemaDeltaProvider = new SchemaDeltaProvider();

        private ConfigUpdater ConfigUpdater = new ConfigUpdater();

        XDataSourceProvider XDataSourceProvider = new XDataSourceProvider();

        public void UpdateSchema(string name)
        {
            SchemaDeltaProvider.Update(name);
            PrimarySchemaProvider.Update(name, SchemaSource.DbSchemaProvider);
        }

        public void UpdateDatabase(string name)
        {
            ConfigUpdater.UpdateDatabase(name);
        }

        public void UpdateDateFormatters()
        {
            ConfigUpdater.UpdateDateFormatters();
        }

        public void UpdateDataConverters()
        {
            ConfigUpdater.UpdateDataConverters();
        }

        public void UpdateDataSources(string name)
        {
            XDataSourceProvider.Update(name);
        }

        public void UpdateAll()
        {
            string name = ConfigurationManager.ConnectionStrings[0].Name;
            UpdateSchema(name);
            UpdateDatabase(name);
            UpdateDateFormatters();
            UpdateDataConverters();
            UpdateDataSources(name);
        }

    }
}
