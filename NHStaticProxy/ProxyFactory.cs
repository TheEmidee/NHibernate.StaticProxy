using System;
using NHibernate.Engine;
using NHibernate.Proxy;

namespace NHStaticProxy
{
    public class ProxyFactory : AbstractProxyFactory
    {
        public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            var instance = (IPostSharpNHibernateProxy)Activator.CreateInstance(PersistentClass);
            var initializer = new StaticProxyLazyInitializer(EntityName, PersistentClass, id, session);

            instance.SetInterceptor(initializer);

            return instance;
        }
    }
}