using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XData.Data.Services;

namespace XData.Web.Http.Controllers
{
    [RoutePrefix("Admin/Api")]
    public class AdminController : ApiController
    {
        [Route("Membership")]
        public MembershipSettings GetMembership()
        {
            return new SettingsService().GetMembershipSettings();
        }

        [Route("InitPwd")]
        public InitialPasswordSettings GetInitialPassword()
        {
            return new SettingsService().GetInitialPasswordSettings();
        }

        [Route("Membership")]
        public void PutMembership([FromBody]MembershipSettings value)
        {
            new SettingsService().SetMembershipSettings(value);
        }

        [Route("InitPwd")]
        public void PutInitialPassword([FromBody]InitialPasswordSettings value)
        {
            new SettingsService().SetInitialPasswordSettings(value);
        }

        [Route("Users")]
        public void Post([FromBody]JToken value)
        {
            int employeeId = value["EmployeeId"].Value<int>();
            string userName = value["UserName"].Value<string>();

            bool isDisabled = value["IsDisabled"] != null;

            List<int> roleIds = new List<int>();
            JToken jRoleId = value["RoleId"];
            if (jRoleId != null)
            {
                if (jRoleId is JValue)
                {
                    int roleId = jRoleId.Value<int>();
                    roleIds.Add(roleId);
                }
                else
                {
                    JArray jArray = jRoleId as JArray;
                    foreach (JToken jToken in jArray)
                    {
                        int roleId = jToken.Value<int>();
                        roleIds.Add(roleId);
                    }
                }
            }

            new UsersService().Create(employeeId, userName, isDisabled, roleIds);
        }

        [Route("Users/Password")]
        public void PutPassword([FromBody]JToken value)
        {
            int id = value["Id"].Value<int>();
            string password = value["Password"].Value<string>();

            new UsersService().ResetPassword(id, password);
        }

        [Route("Users/Status")] // enable/disable
        public void PutStatus([FromBody]JToken value)
        {
            int id = value["Id"].Value<int>();
            bool isDisabled = value["IsDisabled"].Value<bool>();

            new UsersService().SetStatus(id, isDisabled);
        }

        [Route("Users/LockoutStatus")]
        public void PutLockoutStatus([FromBody]JToken value)
        {
            int id = value["Id"].Value<int>();

            new UsersService().Unlock(id);
        }

        [Route("Users/Roles")]
        public void PutRoles([FromBody]JToken value)
        {
            int? id = null;
            List<int> roleIds = new List<int>();

            JArray jArray = value as JArray;
            foreach (JToken jToken in jArray)
            {
                string name = jToken["name"].Value<string>();
                if (name == "Id")
                {
                    id = jToken["value"].Value<int>();
                }
                else if (name == "RoleId")
                {
                    int roleId = jToken["value"].Value<int>();
                    roleIds.Add(roleId);
                }
            }

            new UsersService().GrantRoles((int)id, roleIds);
        }


    }
}
