using System;
using System.Collections.Generic;

namespace SuperFramework.SuperHost
{
    public interface IApplicationHost
    {
        AppDomain AppDomain { get; }
        IEnumerable<Type> GetType(Func<Type, bool> predicate);
    }
}
