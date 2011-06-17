using System;
using NHibernate.Engine;
using NHibernate.Mapping;
using PostSharp.Aspects;

namespace NHStaticProxy
{
    public class StaticProxyLazyInitializer : AbstractLazyInitializer, IStaticProxyLazyInitializer
    {
        private readonly Type persistentClass;

        public StaticProxyLazyInitializer(string entityName, Type persistentClass, object identifier, ISessionImplementor session)
            : base(entityName, identifier, session)
        {
            this.persistentClass = persistentClass;
        }

        public override Type PersistentClass { get { return persistentClass; } }

        private void InitializeIfNeeded()
        {
            if (!IsUninitialized)
                return;

            Initialize();
        }

        public void InterceptGet(LocationInterceptionArgs eventArgs)
        {
            InitializeIfNeeded();

            eventArgs.Value = eventArgs.Binding.GetValue(ref _target, null);
        }

        public void InterceptSet(LocationInterceptionArgs eventArgs)
        {
            InitializeIfNeeded();

            eventArgs.Binding.SetValue(ref _target, null, eventArgs.Value);
        }
    }
}