using PostSharp.Aspects;

namespace NHibernate.StaticProxy
{
    public interface IStaticProxyLazyInitializer : NHibernate.Proxy.ILazyInitializer
    {
        object InterceptGet(ILocationBinding binding);
        void InterceptSet(ILocationBinding binding, object value);
    }
}