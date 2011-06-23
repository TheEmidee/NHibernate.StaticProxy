using NHibernate.Engine;
using PostSharp.Aspects;

namespace NHibernate.StaticProxy
{
    public class StaticProxyLazyInitializer : AbstractLazyInitializer, IStaticProxyLazyInitializer
    {
        private readonly System.Type persistentClass;

        public StaticProxyLazyInitializer(string entityName, System.Type persistentClass, object identifier, ISessionImplementor session)
            : base(entityName, identifier, session)
        {
            this.persistentClass = persistentClass;
        }

        public override System.Type PersistentClass { get { return persistentClass; } }

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