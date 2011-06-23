using System.IO;
using Microsoft.Xml.Serialization.GeneratedAssembly;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.StaticProxy.Examples
{
    public class MappingDocumentParser
    {
        private readonly HbmMappingSerializer serializer = new HbmMappingSerializer();
        private readonly HbmMapping mapping;

        public MappingDocumentParser(Stream stream)
        {
            mapping = (HbmMapping)serializer.Deserialize(stream);
        }

        public HbmMapping Mapping
        {
            get { return mapping; }
        }
    }
}