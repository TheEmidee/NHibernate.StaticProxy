using System;
using NHibernate.Engine;
using NHibernate.Proxy;

namespace NHibernate.StaticProxy
{
    public class StaticProxyFactory : AbstractProxyFactory
    {
        public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            var instance = (INHibernateStaticProxy)Activator.CreateInstance(PersistentClass);
            var initializer = new StaticProxyLazyInitializer(EntityName, PersistentClass, id, session);

            instance.SetInterceptor(initializer);

            return instance;
        }
    }
}