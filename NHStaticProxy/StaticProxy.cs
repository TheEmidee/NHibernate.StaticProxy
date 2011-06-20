using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Proxy;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;

namespace NHStaticProxy
{
    [Serializable]
    [IntroduceInterface(typeof(IPostSharpNHibernateProxy), OverrideAction = InterfaceOverrideAction.Ignore)]
    [MulticastAttributeUsage(MulticastTargets.Class, Inheritance = MulticastInheritance.Strict)]
    public class StaticProxy : InstanceLevelAspect, IPostSharpNHibernateProxy
    {
        [NonSerialized]
        private bool hasErrors = false;

        [NonSerialized]
        private IEnumerable<PropertyInfo> mappedProps = Enumerable.Empty<PropertyInfo>();

        [NonSerialized]
        private IStaticProxyLazyInitializer interceptor;

        private static IList<HbmMapping> mappings;

        public override bool CompileTimeValidate(Type type)
        {
            return !hasErrors;
        }

        public override void CompileTimeInitialize(Type type, AspectInfo aspectInfo)
        {
            if (mappings == null)
            {
                object[] attributes = type.Assembly.GetCustomAttributes(typeof(StaticProxyConfigurationAttribute), true);

                if (attributes.Length == 0)
                {
                    hasErrors = true;
                    Message.Write(SeverityType.Error, "CUSTOM02", string.Format("Impossible to find an assembly attribute derived from StaticProxyConfigurationAttribute in the assembly {0}.", type.Assembly.FullName));
                    return;
                }

                var attribute = attributes[0] as StaticProxyConfigurationAttribute;

                mappings = attribute.HbmMappings.ToList();
            }

            mappedProps = from mapping in mappings
                          let hbmClasses = from hbmClass in mapping.RootClasses where hbmClass.Name == type.Name select hbmClass
                          from rootClass in hbmClasses where rootClass != null
                          from item in rootClass.Items
                          let propertyMapping = item as IEntityPropertyMapping
                          where propertyMapping != null
                          select type.GetProperty(propertyMapping.Name);

            if (mappedProps.Any())
                return;

            hasErrors = true;
            Message.Write(SeverityType.Warning, "CUSTOM02", string.Format("Impossible to find a mapping file for the type {0}.", type.Name));
        }

        private IEnumerable<PropertyInfo> MappedProperties(Type type)
        {
            return mappedProps;
        }

        private IStaticProxyLazyInitializer GetProxy(AdviceArgs args)
        {
            var proxy = args.Instance as INHibernateProxy;

            if (proxy == null)
                return null;

            var initializer = proxy.HibernateLazyInitializer;

            if (initializer == null)
                return null;

            return ((IStaticProxyLazyInitializer)initializer);
        }

        [OnLocationGetValueAdvice, MethodPointcut("MappedProperties")]
        public void OnLocationGetValue(LocationInterceptionArgs args)
        {
            var proxy = GetProxy(args);

            if (proxy != null)
            {
                proxy.InterceptGet(args);
                return;
            }

            args.ProceedGetValue();
        }

        [OnLocationSetValueAdvice, MethodPointcut("MappedProperties")]
        public void OnLocationSetValue(LocationInterceptionArgs args)
        {
            var proxy = GetProxy(args);

            if (proxy != null)
            {
                proxy.InterceptSet(args);
                return;
            }

            args.ProceedSetValue();
        }

        public void SetInterceptor(IStaticProxyLazyInitializer postSharpInitializer)
        {
            interceptor = postSharpInitializer;
        }

        public ILazyInitializer HibernateLazyInitializer
        {
            get { return interceptor; }
        }
    }
}