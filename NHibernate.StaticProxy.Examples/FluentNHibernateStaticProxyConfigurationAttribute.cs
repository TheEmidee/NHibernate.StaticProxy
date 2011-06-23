using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.StaticProxy.Examples
{
    public class FluentNHibernateStaticProxyConfigurationAttribute : StaticProxyConfigurationAttribute
    {
        public override IEnumerable<HbmMapping> HbmMappings
        {
            get
            {
                var cfg = new Configuration();

                var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                var di = Directory.CreateDirectory(tempFolder);

                //var fc = Fluently.Configure(cfg)
                //    .Database(SQLiteConfiguration.Standard.InMemory)
                //    .Mappings(mc => mc.FluentMappings.AddFromAssemblyOf<Customer>().ExportTo(tempFolder));

                //fc.BuildConfiguration();

                foreach (var file in di.EnumerateFiles())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");

                    using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    using (TextReader tr = new StreamReader(fs))
                    {
                        sb.Append(tr.ReadToEnd());
                    }

                    using (var tr = new StringReader(sb.ToString()))
                    using (XmlReader reader = XmlReader.Create(tr))
                    {
                          NamedXmlDocument namedDoc = cfg.LoadMappingDocument(reader, "Test");

                          yield return namedDoc.Document;
                    }
                }
            }
        }
    }
}