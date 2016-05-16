using Joy.Common.Meta;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Joy.Common.Authentication
{
    public class AuthenticationInterceptionBehavior:IInterceptionBehavior
    {

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var methodInfo = getNext();
            AuthorizeAttribute attr = input.MethodBase.GetCustomAttribute<AuthorizeAttribute>();
            if (attr != null)
            {
                ParameterInfo[] pros = input.MethodBase.GetParameters();
                RequestContext ctx=null;
                for(int i=0;i<pros.Length;i++)
                {
                    if (pros[i].ParameterType.Equals(typeof(RequestContext)))
                    {
                        object ctxValue = input.Arguments[i];
                        if (ctxValue == null) return input.CreateMethodReturn(new ReturnValue(false, null, ErrorMessage.MissingSessionKey));
                        ctx = ctxValue as RequestContext;
                        if (!attr.Roles.Equals(string.Empty) && (!ctx.ContainsKey("Role") || !attr.IsRole(ctx["Role"].ToString()))) return input.CreateMethodReturn(new ReturnValue(false, null, ErrorMessage.RoleNotMatch));
                        if (ctx["Role"].ToString().ToUpper().Equals("STAFF") && !attr.Action.Equals(string.Empty) && (!ctx.ContainsKey("Actions") || !(ctx["Actions"] as string[]).Contains<string>(attr.Action))) return input.CreateMethodReturn(new ReturnValue(false, null, ErrorMessage.NoPermission));
                        break;
                    }
                }
                if (ctx == null) return input.CreateMethodReturn(new ReturnValue(false, null, ErrorMessage.MissingSessionKey));
            }
            var result = methodInfo.Invoke(input, getNext);
            return result;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}