using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Proxy;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace NHibernate.StaticProxy
{
    [Serializable]
    [IntroduceInterface(typeof(INHibernateStaticProxy), OverrideAction = InterfaceOverrideAction.Ignore)]
    [MulticastAttributeUsage(MulticastTargets.Class, Inheritance = MulticastInheritance.Strict)]
    public class StaticProxyAttribute : InstanceLevelAspect, INHibernateStaticProxy
    {
        [NonSerialized]
        private readonly IList<object> mappedMembers = new List<object>();

        [NonSerialized]
        private IStaticProxyLazyInitializer interceptor;

        private static IList<HbmMapping> mappings;

        public override bool CompileTimeValidate(System.Type type)
        {
            if (mappings == null)
            {
                Message.Write(SeverityType.Error, "NoStaticProxyConfigurationAttribute", string.Format("Impossible to find an assembly attribute derived from {0} to weave the type {1}.", typeof(StaticProxyConfigurationAttribute).FullName, type.FullName));
                return false;
            }

            if (!mappedMembers.Any())
                Message.Write(SeverityType.Warning, "NoMappings", string.Format("No mappings were found for the type: {0}.", type.FullName));
            
            return true;
        }

        public override void CompileTimeInitialize(System.Type type, AspectInfo aspectInfo)
        {
            SetHbmMappings(type);

            if (mappings == null)
                return;

            SetMappedMembers(type);
        }

        private void SetHbmMappings(System.Type type)
        {
            if (mappings == null)
            {
                object[] attributes = type.Assembly.GetCustomAttributes(typeof(StaticProxyConfigurationAttribute), true);

                if (attributes.Length == 0)
                    return;

                var attribute = attributes[0] as StaticProxyConfigurationAttribute;

                mappings = attribute.HbmMappings.ToList();
            }
        }

        private void SetMappedMembers(System.Type type)
        {
            var propertyMappings = from mapping in mappings
                                   let hbmClasses = from hbmClass in mapping.RootClasses where hbmClass.Name == type.Name select hbmClass
                                   from rootClass in hbmClasses
                                   where rootClass != null
                                   from propertyMapping in rootClass.Items.OfType<IEntityPropertyMapping>()
                                   select propertyMapping;

            foreach (var propertyMapping in propertyMappings)
            {
                if (propertyMapping.Access != null && propertyMapping.Access == "field")
                {
                    var fieldInfo = type.GetField(propertyMapping.Name, BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    mappedMembers.Add(new LocationInfo(fieldInfo));
                }
                else
                    mappedMembers.Add(new LocationInfo(type.GetProperty(propertyMapping.Name)));
            }
        }

        private IEnumerable<object> MappedMembers(System.Type type)
        {
            return mappedMembers;
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
        
        [OnLocationGetValueAdvice, MethodPointcut("MappedMembers")]
        public void OnLocationGetValue(LocationInterceptionArgs args)
        {
            var proxy = GetProxy(args);

            if (proxy != null)
            {
                args.Value = proxy.InterceptGet(args.Binding);
                return;
            }

            args.ProceedGetValue();
        }

        [OnLocationSetValueAdvice(Master = "OnLocationGetValue")]
        public void OnLocationSetValue(LocationInterceptionArgs args)
        {
            var proxy = GetProxy(args);

            if (proxy != null)
            {
                proxy.InterceptSet(args.Binding, args.Value);
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