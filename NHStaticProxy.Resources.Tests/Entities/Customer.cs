using Iesi.Collections.Generic;

namespace NHStaticProxy.Resources.Tests.Entities
{
    [StaticProxy]
    public class Customer
    {
        public Customer()
        {
            Orders = new HashedSet<Order>();
        }

        public int Id { get; set; }
        
        public string Name { get; set; }

        public string NotMapped { get; set; }
        
        public ISet<Order> Orders { get; set; }
        
        public void AddOrder(Order order)
        {
            Orders.Add(order);
            order.Customer = this;
        }
    }
}
