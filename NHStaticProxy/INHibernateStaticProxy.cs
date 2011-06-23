using NHibernate.Proxy;

namespace NHStaticProxy
{
    public interface INHibernateStaticProxy : INHibernateProxy
    {
        void SetInterceptor(IStaticProxyLazyInitializer postSharpInitializer);
    }
}