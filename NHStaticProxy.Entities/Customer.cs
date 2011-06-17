using Iesi.Collections.Generic;

namespace NHStaticProxy.Entities
{
    [StaticProxy]
    public class Customer
    {
        public Customer()
        {
            Orders = new HashedSet<Order>();
        }
        
        public string Name { get; set; }
        public int Id { get; set; }
        public ISet<Order> Orders { get; set; }
        
        public void AddOrder(Order order)
        {
            Orders.Add(order);
            order.Customer = this;
        }
    }
}
