#region using
using Joy.Common.Caching;
using Joy.Common.Encrypt;
using Joy.Common.Meta;
using Joy.Common.Object;
using System;
using System.Collections;
using System.Configuration;
using Joy.Common.Logging;
#endregion

namespace Joy.Common.Authentication.Impl
{
    public class MemCachedAuthorize:IAuthorize
    {
        public string DesKey { get; set; }
        public string Profix { get; set; }
        public int ExpiredTime { get; set; }
        public MemCachedAuthorize()
        {
            this.DesKey = ConfigurationManager.AppSettings["joy.common.authorize.desKey"]??"s0M13p69";
            this.Profix = ConfigurationManager.AppSettings["joy.common.authorize.profix"]??"joy";
            this.ExpiredTime = (ConfigurationManager.AppSettings["joy.common.authorize.timeout"] == null) ? 300 : int.Parse(ConfigurationManager.AppSettings["joy.common.authorize.timeout"].ToString());
        }

        public UserSession CheckSession(string sessionKey)
        {
            if (sessionKey == null || sessionKey.Length == 0)
            {
                this.Log(LoggerLevel.Debug, "SessionKey is null or Empty.");
                return null;
            }
            try
            {
                string userJson = EncryptHelper.DecryptDES(sessionKey, this.DesKey);
                if (userJson.Length == 0)
                {
                    this.Log(LoggerLevel.Debug, "SessionKey decrypt deskey failed.");
                    return null;
                }
                UserSession session = userJson.JsonToObject<UserSession>();
                string memKey = string.Format("{0}.{1}.{2}", this.Profix, session.Role, session.UserId);
                UserSession mSession = MemCachedTool.Get<UserSession>(memKey);
                if (mSession != null && session.Token.Equals(mSession.Token))
                {
                    MemCachedTool.Set(memKey, mSession, this.ExpiredTime);
                    return mSession;
                }
                else
                {
                    if (mSession==null)  this.Log(LoggerLevel.Debug, "SessionKey not found in memcached.");
                    else this.Log(LoggerLevel.Debug, "Session.Token is mismatch.");
                    return null;
                }
            }
            catch
            {
                return null; 
            }
        }

        public string GetNewSessionKey(string loginName, string password, string userId, string role, string ip, string fullcode, params string[] actions)
        {
            string token = Guid.NewGuid().ToString().ToLower().Replace("-", "");
            UserSession session = new UserSession(userId, loginName, password, role, ip,fullcode, token);
            session.Actions = actions;
            string memKey = string.Format("{0}.{1}.{2}", this.Profix, role, userId);
            string sessionKey = EncryptHelper.EncryptDES(session.ToJson(), this.DesKey);
            UserSession oldSession = MemCachedTool.Pop<UserSession>(memKey);
            bool success = MemCachedTool.Add(memKey, session, this.ExpiredTime);
            if (success) return sessionKey;
            else return string.Empty;
        }

        public UserSession CancelSession(string sessionKey)
        {
            string json = EncryptHelper.DecryptDES(sessionKey, this.DesKey);
            UserSession session = json.JsonToObject<UserSession>();
            string memKey = string.Format("{0}.{1}.{2}", this.Profix, session.Role, session.UserId);
            UserSession mSession = MemCachedTool.Get<UserSession>(memKey);
            if (mSession.Token.Equals(session.Token)) MemCachedTool.Remove(memKey);
            return session;
        }
    }
}
