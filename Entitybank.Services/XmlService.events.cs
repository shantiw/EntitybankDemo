using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XData.Data.Objects;
using XData.Data.Xml;

namespace XData.Data.Services
{
    public partial class XmlService
    {
        private void RegisterEvents()
        {
            Modifier.Validating += Modifier_Validating;
            Modifier.Database.Inserting += Database_Inserting;
            Modifier.Database.Inserted += Database_Inserted;
            Modifier.Database.Deleting += Database_Deleting;
            Modifier.Database.Updating += Database_Updating;
        }

        private void Modifier_Validating(object sender, ValidatingEventArgs args)
        {
        }

        private void Database_Inserting(object sender, InsertingEventArgs args)
        {
        }

        private void Database_Inserted(object sender, InsertedEventArgs args)
        {
        }

        private void Database_Deleting(object sender, DeletingEventArgs args)
        {
        }

        private void Database_Updating(object sender, UpdatingEventArgs args)
        {
        }


    }
}
