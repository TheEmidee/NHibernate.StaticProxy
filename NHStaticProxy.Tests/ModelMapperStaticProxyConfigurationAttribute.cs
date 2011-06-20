using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHStaticProxy.Tests.Config;

namespace NHStaticProxy.Tests
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