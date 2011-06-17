using NHibernate.Proxy;

namespace NHStaticProxy
{
    public interface IPostSharpNHibernateProxy : INHibernateProxy
    {
        void SetInterceptor(LazyInitializer postSharpInitializer);
    }
}