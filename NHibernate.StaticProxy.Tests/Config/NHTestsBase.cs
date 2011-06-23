using NHibernate;
using NHibernate.Cfg;
using Xunit;

namespace NHibernate.StaticProxy.Tests.Config
{
    public class NHTestsBase<THbmMappingProvider> : IUseFixture<NHibernateFixture<THbmMappingProvider>>
        where THbmMappingProvider : IHbmMappingProvider
    {
        private NHibernateFixture<THbmMappingProvider> fixture;

        protected ISessionFactory SessionFactory { get { return fixture.SessionFactory; } }

        protected Configuration Configuration { get { return fixture.Configuration; } }

        public ISession Session { get { return fixture.Session; } }

        #region IUseFixture<NHibernateFixture<THbmMappingProvider>> Members

        public void SetFixture(NHibernateFixture<THbmMappingProvider> data)
        {
            if (fixture != null)
                fixture.TearDownNHibernateSession();

            fixture = data;

            if (fixture != null)
                fixture.SetupNHibernateSession();
        }

        #endregion
    }
}