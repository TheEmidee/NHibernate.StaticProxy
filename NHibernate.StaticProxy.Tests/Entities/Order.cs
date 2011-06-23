namespace NHibernate.StaticProxy.Tests.Entities
{
    [StaticProxy]
    public class Order
    {
        public string Name { get; set; }

        public Customer Customer { get; set; }

        public int Id { get; set; }
    }
}