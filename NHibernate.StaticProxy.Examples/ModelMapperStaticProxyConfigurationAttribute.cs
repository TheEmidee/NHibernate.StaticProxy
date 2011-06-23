using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.StaticProxy.Examples.Entities;

namespace NHibernate.StaticProxy.Examples
{
    public class ModelMapperStaticProxyConfigurationAttribute : StaticProxyConfigurationAttribute
    {
        public override IEnumerable<HbmMapping> HbmMappings
        {
            get
            {
                var mapper = new ModelMapper();

                mapper.Class<Customer>(customer =>
                {
                    customer.Id(mt => mt.Id,
                          idm => idm.Generator(Generators.Native));

                    customer.Property(mt => mt.Name);

                    customer.Property(p => p.PropertyWithField,
                                      m => m.Access(Accessor.Field));

                    customer.Property("fieldOnly", m => { });

                    customer.Set(p => p.Orders,
                                 cm =>
                                 {
                                     cm.Cascade(Cascade.Persist);
                                     cm.Inverse(true);
                                 },
                                 m => m.OneToMany());
                });

                mapper.Class<Order>(order =>
                {
                    order.Id(mt => mt.Id,
                          idm =>
                          {
                              idm.Access(Accessor.Field);
                              idm.Generator(Generators.Native);
                          });

                    order.Property(mt => mt.Name);
                    order.ManyToOne(p => p.Customer);
                });

                var c = new Configuration();
                c.AddMapping(mapper.CompileMappingForAllExplicitAddedEntities());

                yield return mapper.CompileMappingForAllExplicitAddedEntities();
            }
        }
    }
}