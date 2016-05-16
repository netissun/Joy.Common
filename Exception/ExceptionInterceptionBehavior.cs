using Joy.Common.Logging;
using Joy.Common.Meta;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;

namespace Joy.Common.Exception
{
    public class ExceptionInterceptionBehavior:IInterceptionBehavior
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
                LoggerHelper.Log(input.MethodBase.ReflectedType, LoggerLevel.Error, string.Format("\r\n==========================\r\n{0}\r\n==========================\r\n",result.Exception.StackTrace ));
                return input.CreateMethodReturn(new ReturnValue(false, 0, result.Exception.Message));
            }
            return result;
        }

        public bool WillExecute
        {
            get { return true;  }
        }
    }
}
