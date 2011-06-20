using System.IO;
using Microsoft.Xml.Serialization.GeneratedAssembly;
using NHibernate.Cfg.MappingSchema;

namespace NHStaticProxy.Resources.Tests
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