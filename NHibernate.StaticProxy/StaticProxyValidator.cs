using System.Collections.Generic;
using System.Reflection;
using NHibernate.Proxy;

namespace NHibernate.StaticProxy
{
    public class StaticProxyValidator : IProxyValidator
    {
        public ICollection<string> ValidateType(System.Type type)
        {
            return null;
        }

        public bool IsProxeable(MethodInfo method)
        {
            return true;
        }
    }
}