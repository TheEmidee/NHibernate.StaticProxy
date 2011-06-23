using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.StaticProxy.Examples.Entities;

namespace NHibernate.StaticProxy.Examples
{
    public class ResourcesStaticProxyConfigurationAttribute : StaticProxyConfigurationAttribute
    {
        public override IEnumerable<HbmMapping> HbmMappings
        {
            get
            {
                var assembly = typeof(Customer).Assembly;

                return from ressource in assembly.GetManifestResourceNames().Where(x => x.EndsWith(".hbm.xml"))
                       let stream = assembly.GetManifestResourceStream(ressource)
                       let parser = new MappingDocumentParser(stream)
                       select parser.Mapping;
            }
        }
    }
}