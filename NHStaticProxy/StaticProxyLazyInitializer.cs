using System;
using NHibernate.Engine;
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

        public object InterceptGet(ILocationBinding binding)
        {
            InitializeIfNeeded();

            return binding.GetValue(ref target, null);
        }

        public void InterceptSet(ILocationBinding binding, object value)
        {
            InitializeIfNeeded();

            binding.SetValue(ref target, null, value);
        }
    }
}