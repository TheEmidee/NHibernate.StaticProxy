using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace NHibernate.StaticProxy
{
    public class StaticProxyFactoryFactory : IProxyFactoryFactory
    {
        public IProxyFactory BuildProxyFactory()
        {
            return new StaticProxyFactory();
        }

        public IProxyValidator ProxyValidator
        {
            get { return new StaticProxyValidator(); }
        }

        public bool IsInstrumented(System.Type entityClass)
        {
            return true;
        }

        public bool IsProxy(object entity)
        {
            var proxy = entity as INHibernateProxy;

            return proxy != null && proxy.HibernateLazyInitializer != null;
        }
    }
}