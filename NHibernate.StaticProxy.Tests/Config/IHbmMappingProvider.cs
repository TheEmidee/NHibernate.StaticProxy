using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.StaticProxy.Tests.Config
{
    public interface IHbmMappingProvider
    {
        IEnumerable<HbmMapping> HbmMappings { get; }
    }
}