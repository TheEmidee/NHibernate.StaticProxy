using System;
using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace NHStaticProxy
{
    public class ProxyFactoryFactory : IProxyFactoryFactory
    {
        public IProxyFactory BuildProxyFactory()
        {
            return new ProxyFactory();
        }

        public IProxyValidator ProxyValidator
        {
            get { return new ProxyValidator(); }
        }

        public bool IsInstrumented(Type entityClass)
        {
            return true;
        }

        public bool IsProxy(object entity)
        {
            INHibernateProxy proxy = entity as INHibernateProxy;

            return proxy != null && proxy.HibernateLazyInitializer != null;
        }
    }
}