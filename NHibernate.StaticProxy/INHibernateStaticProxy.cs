using NHibernate.Proxy;

namespace NHibernate.StaticProxy
{
    public interface INHibernateStaticProxy : INHibernateProxy
    {
        void SetInterceptor(IStaticProxyLazyInitializer postSharpInitializer);
    }
}