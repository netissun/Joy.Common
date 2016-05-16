using System;
using System.Linq;

namespace Joy.Common.Authentication
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeAttribute:Attribute
    {
        public string Roles { get; set; }
        public string Action { get; set; }
        
        public AuthorizeAttribute(string Roles, string Action)
        {
            this.Roles = Roles;
            this.Action = Action;
        }
        public AuthorizeAttribute(string Roles)
            :this(Roles,string.Empty)
        {

        }
        public AuthorizeAttribute()
            :this(string.Empty,string.Empty)
        {

        }

        public bool IsRole(string role)
        {
            return Roles.Split(',').Contains<string>(role);
        }

    }
}