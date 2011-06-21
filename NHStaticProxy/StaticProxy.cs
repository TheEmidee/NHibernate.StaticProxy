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

namespace NHStaticProxy
{
    [Serializable]
    [IntroduceInterface(typeof(IPostSharpNHibernateProxy), OverrideAction = InterfaceOverrideAction.Ignore)]
    [MulticastAttributeUsage(MulticastTargets.Class, Inheritance = MulticastInheritance.Strict)]
    public class StaticProxy : InstanceLevelAspect, IPostSharpNHibernateProxy
    {
        [NonSerialized]
        private readonly IList<object> mappedMembers = new List<object>();

        [NonSerialized]
        private IStaticProxyLazyInitializer interceptor;

        private static IList<HbmMapping> mappings;

        public override bool CompileTimeValidate(Type type)
        {
            if (mappings == null)
            {
                Message.Write(SeverityType.Error, "CUSTOM02", string.Format("Impossible to find an assembly attribute derived from {0} in the assembly {1}.", typeof(StaticProxyConfigurationAttribute).FullName, type.Assembly.FullName));
                return false;
            }
            
            return true;
        }

        public override void CompileTimeInitialize(Type type, AspectInfo aspectInfo)
        {
            SetHbmMappings(type);

            if (mappings == null)
                return;

            SetMappedMembers(type);
        }

        private void SetHbmMappings(Type type)
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

        private void SetMappedMembers(Type type)
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

        private IEnumerable<object> MappedMembers(Type type)
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
                proxy.InterceptGet(args);
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