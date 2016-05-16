using System;

namespace Joy.Common.Logging
{
    [AttributeUsage(AttributeTargets.Parameter|AttributeTargets.ReturnValue|AttributeTargets.Method)]
    public class IgnoreLoggerAttribute:Attribute
    {
    }
}
