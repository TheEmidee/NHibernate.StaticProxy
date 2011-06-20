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
            Assert.IsType<ProxyFactoryFactory>(Environment.BytecodeProvider.ProxyFactoryFactory);
        }

        [Fact]
        public void ProxyValidatorIsSetProperly()
        {
            Assert.IsType<ProxyValidator>(Environment.BytecodeProvider.ProxyFactoryFactory.ProxyValidator);
        }
    }
}