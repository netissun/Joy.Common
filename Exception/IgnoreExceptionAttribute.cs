﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joy.Common.Exception
{
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnoreExceptionAttribute:Attribute
    {

    }
}
