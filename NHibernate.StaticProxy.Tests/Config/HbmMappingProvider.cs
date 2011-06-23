using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.StaticProxy.Tests.Entities;

namespace NHibernate.StaticProxy.Tests.Config
{
    public class HbmMappingProvider : IHbmMappingProvider
    {
        public IEnumerable<HbmMapping> HbmMappings
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
                    order.Table("Orders");
                    
                    order.Id(mt => mt.Id,
                          idm => idm.Generator(Generators.Native));

                    order.Property(mt => mt.Name);
                    order.ManyToOne(p => p.Customer);
                });

                yield return mapper.CompileMappingForAllExplicitAddedEntities();
            }
        }
    }
}