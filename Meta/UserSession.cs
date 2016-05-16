using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joy.Common.Meta
{
    [Serializable]
    public class UserSession
    {
        public string UserId { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Ip { get; set; }
        public string FullCode { get; set; }
        public string  Token { get; set; }
        public string[] Actions { get; set; }

        public UserSession(string userId, string loginName, string password,string role, string ip, string fullcode,string token)
        {
            this.UserId = userId;
            this.LoginName = loginName;
            this.Password = password;
            this.Role = role;
            this.Token = token;
            this.Ip = ip;
            this.FullCode = fullcode;
            this.Actions = new string[] { };
        }
        public UserSession()
            :this(string.Empty,string.Empty,string.Empty,string.Empty,string.Empty,string.Empty,string.Empty)
        {
        }
        public bool HasAction(string actionId)
        {
            if (this.Actions == null || this.Actions.Length == 0) return false;
            else return this.Actions.Contains<string>(actionId);
        }
    }
}
