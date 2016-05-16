using Joy.Common.Logging;
using Joy.Common.Meta;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Joy.Common.Exception
{
    public class CommonExceptionInterceptionBehavior : IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var methodInfo = getNext();
            var result = methodInfo.Invoke(input, getNext);
            if (result.Exception != null)
            {
                LoggerHelper.Log(input.MethodBase.ReflectedType, LoggerLevel.Info, string.Format("\r\n[Exception]{0}.{1}\r\n{2}\r\n", input.MethodBase.ReflectedType, result.Exception.Message, result.Exception.StackTrace));
                if (input.MethodBase.GetCustomAttribute<IgnoreExceptionAttribute>() != null) return input.CreateMethodReturn(null);
            }
            return result;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
