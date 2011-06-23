using PostSharp.Aspects;

namespace NHStaticProxy
{
    public interface IStaticProxyLazyInitializer : NHibernate.Proxy.ILazyInitializer
    {
        object InterceptGet(ILocationBinding binding);
        void InterceptSet(ILocationBinding binding, object value);
    }
}