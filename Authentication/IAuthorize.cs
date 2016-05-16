using Joy.Common.Meta;
using System.Collections;

namespace Joy.Common.Authentication
{
    public interface IAuthorize
    {
        UserSession CheckSession(string sessionKey);
        string GetNewSessionKey(string loginName, string password, string userId, string role, string ip, string fullCode, params string[] actions);
        UserSession CancelSession(string sessionKey);
    }
}
