using Iesi.Collections.Generic;

namespace NHibernate.StaticProxy.Tests.Entities
{
    [StaticProxy]
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }

    public class Customer2 : BaseEntity
    {
        public Customer2()
        {
            Orders = new HashedSet<Order2>();
        }

        public string Name { get; set; }

        public ISet<Order2> Orders { get; set; }

        public void AddOrder(Order2 order)
        {
            Orders.Add(order);
            order.Customer = this;
        }
    }

    public class Order2 : BaseEntity
    {
        public string Name { get; set; }
        public Customer2 Customer { get; set; }
    }
}