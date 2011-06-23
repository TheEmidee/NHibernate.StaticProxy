using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.StaticProxy.Tests.Config;

namespace NHibernate.StaticProxy.Tests
{
    public class ModelMapperStaticProxyConfigurationAttribute : StaticProxyConfigurationAttribute
    {
        public override IEnumerable<HbmMapping> HbmMappings
        {
            get
            {
                return new HbmMappingProvider().HbmMappings;
            }
        }
    }
}