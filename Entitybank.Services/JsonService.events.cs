using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XData.Data.Dynamic;
using XData.Data.Objects;

namespace XData.Data.Services
{
    public partial class JsonService
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
            if (args.Entity == "User")
            {
                string userName = args.AggregNode["UserName"].ToString();
                args.AggregNode["LoweredUserName"] = userName.ToLower();
            }
            else if (args.Entity == "Role")
            {
                string roleName = args.AggregNode["RoleName"].ToString();
                args.AggregNode["LoweredRoleName"] = roleName.ToLower();
            }
        }

        private void Database_Inserted(object sender, InsertedEventArgs args)
        {
            if (args.Entity == "User")
            {

            }
        }

        private void Database_Deleting(object sender, DeletingEventArgs args)
        {
            if (args.Entity == "User")
            {

            }
        }

        private void Database_Updating(object sender, UpdatingEventArgs args)
        {
            if (args.Entity == "User")
            {
                string userName = args.AggregNode["UserName"].ToString();
                args.AggregNode["LoweredUserName"] = userName.ToLower();
            }
            else if (args.Entity == "Role")
            {
                string roleName = args.AggregNode["RoleName"].ToString();
                args.AggregNode["LoweredRoleName"] = roleName.ToLower();
            }
        }


    }
}
