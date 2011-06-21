using FluentNHibernate.Mapping;
using NHStaticProxy.FluentNHibernate.Example.Entities;

namespace NHStaticProxy.FluentNHibernate.Example.Mappings
{
    public class OrderMapping : ClassMap<Order>
    {
        public OrderMapping()
        {
            Id(o => o.Id).GeneratedBy.Native();

            Map(o => o.Name);

            References(o => o.Customer);
        }
    }
}