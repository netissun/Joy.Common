using System.Collections.Generic;

namespace Joy.Common.Meta
{
    public class RequestContext:Dictionary<string,object>
    {
        public RequestContext(UserSession session)
        {
            this.Add("UserId", int.Parse(session.UserId));
            this.Add("LoginName", session.LoginName);
            this.Add("Role", session.Role);
            this.Add("Ip", session.Ip);
            this.Add("Actions", session.Actions);
            this.Add("Password", session.Password);
            this.Add("Token", session.Token);
            this.Add("FullCode", session.FullCode);
        }
    }
}
