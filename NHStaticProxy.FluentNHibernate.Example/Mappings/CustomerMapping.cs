using FluentNHibernate.Mapping;
using NHStaticProxy.FluentNHibernate.Example.Entities;

namespace NHStaticProxy.FluentNHibernate.Example.Mappings
{
    public class CustomerMapping : ClassMap<Customer>
    {
        public CustomerMapping()
        {
            Id(c => c.Id).GeneratedBy.Native();

            Map(c => c.Name);

            Map(c => c.PropertyWithField).Access.CamelCaseField(Prefix.None);

            Map(c => c.fieldOnly).Access.Field();

            HasMany(c => c.Orders).Inverse();
        }
    }
}