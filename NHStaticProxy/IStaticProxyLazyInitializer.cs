using PostSharp.Aspects;

namespace NHStaticProxy
{
    public interface IStaticProxyLazyInitializer : NHibernate.Proxy.ILazyInitializer
    {
        void InterceptGet(LocationInterceptionArgs eventArgs);
        void InterceptSet(LocationInterceptionArgs eventArgs);
    }
}