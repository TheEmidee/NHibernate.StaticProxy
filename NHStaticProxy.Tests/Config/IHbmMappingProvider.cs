using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;

namespace NHStaticProxy.Tests.Config
{
    public interface IHbmMappingProvider
    {
        IEnumerable<HbmMapping> HbmMappings { get; }
    }
}