using NHibernate.Cfg;
using NHStaticProxy.Tests.Config;
using Xunit;

namespace NHStaticProxy.Tests
{
    public class CanCreateNHibernateSessionFactoryWithLazyLoadingAndNoVirtuals : NHTestsBase<HbmMappingProvider>
    {
        [Fact]
        public void SessionFactoryCreatedSuccessfully()
        {
            Assert.NotNull(Session);
        }

        [Fact]
        public void ProxyFactoryFactoryIsSetProperly()
        {
            Assert.IsType<StaticProxyFactoryFactory>(Environment.BytecodeProvider.ProxyFactoryFactory);
        }

        [Fact]
        public void ProxyValidatorIsSetProperly()
        {
            Assert.IsType<StaticProxyValidator>(Environment.BytecodeProvider.ProxyFactoryFactory.ProxyValidator);
        }
    }
}