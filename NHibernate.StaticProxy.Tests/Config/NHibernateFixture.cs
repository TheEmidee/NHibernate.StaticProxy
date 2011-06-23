using System;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;

namespace NHibernate.StaticProxy.Tests.Config
{
    public class NHibernateFixture<THbmMappingProvider> : IDisposable
        where THbmMappingProvider : IHbmMappingProvider
    {
        public ISessionFactory SessionFactory { get { return NHConfigurator<THbmMappingProvider>.SessionFactory; } }

        public ISession Session { get { return SessionFactory.GetCurrentSession(); } }

        public Configuration Configuration { get { return NHConfigurator<THbmMappingProvider>.Configuration; } }

        #region IDisposable Members

        public void Dispose()
        {
            TearDownNHibernateSession();
        }

        #endregion

        public void SetupNHibernateSession()
        {
            TestConnectionProvider.CloseDatabase();
            SetupContextualSession();
            BuildSchema();
        }

        public void TearDownNHibernateSession()
        {
            TearDownContextualSession();
            TestConnectionProvider.CloseDatabase();
        }

        private void SetupContextualSession()
        {
            ISession session = SessionFactory.OpenSession();
            CurrentSessionContext.Bind(session);
        }

        private void TearDownContextualSession()
        {
            ISessionFactory sessionFactory = NHConfigurator<THbmMappingProvider>.SessionFactory;

            if (sessionFactory != null)
            {
                ISession session = CurrentSessionContext.Unbind(sessionFactory);

                if (session != null)
                    session.Close();
            }
        }

        private void BuildSchema()
        {
            Configuration cfg = NHConfigurator<THbmMappingProvider>.Configuration;
            var schemaExport = new SchemaExport(cfg);
            schemaExport.Create(true, true);
        }
    }
}