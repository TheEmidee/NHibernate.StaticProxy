using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.StaticProxy;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.StaticProxy.Tests.Config
{
    public static class NHConfigurator<TModelMapperProvider>
        where TModelMapperProvider : IHbmMappingProvider
    {
        private static readonly Configuration configuration;
        private static readonly ISessionFactory sessionFactory;

        static NHConfigurator()
        {
            configuration = new Configuration()
                .DataBaseIntegration(db =>
                {
                    db.ConnectionProvider<TestConnectionProvider>();
                    db.Driver<SQLite20Driver>();
                    db.Dialect<SQLiteDialect>();
                    db.LogSqlInConsole = true;
                    db.ConnectionString = @"Data Source=:memory:;Version=3;New=True;";
                });

            configuration.SetProperty(Environment.CurrentSessionContextClass, "thread_static");
            configuration.SetProperty("connection.release_mode", "on_close");
            configuration.Proxy(proxy => proxy.ProxyFactoryFactory<StaticProxyFactoryFactory>());

            var modelMapperProvider = (IHbmMappingProvider)Activator.CreateInstance<TModelMapperProvider>();

            foreach (var hbmMapping in modelMapperProvider.HbmMappings)
                configuration.AddDeserializedMapping(hbmMapping, null);

            IDictionary<string, string> props = configuration.Properties;

            if (props.ContainsKey(Environment.ConnectionStringName))
                props.Remove(Environment.ConnectionStringName);

            sessionFactory = configuration.BuildSessionFactory();
        }

        public static Configuration Configuration { get { return configuration; } }

        public static ISessionFactory SessionFactory { get { return sessionFactory; } }
    }
}