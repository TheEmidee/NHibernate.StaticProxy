using System;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.StaticProxy
{
    public abstract class StaticProxyConfigurationAttribute : Attribute
    {
        public abstract IEnumerable<HbmMapping> HbmMappings { get; }
    }
}