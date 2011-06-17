using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Proxy;

namespace NHStaticProxy
{
    public class ProxyValidator : IProxyValidator
    {
        public ICollection<string> ValidateType(Type type)
        {
            return null;
        }

        public bool IsProxeable(MethodInfo method)
        {
            return true;
        }
    }
}