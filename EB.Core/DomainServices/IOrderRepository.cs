using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.DomainServices
{
    public interface IOrderRepository
    {
        Order AddOrder(Order order);

        IEnumerable<Order> ReadAllOrders();
        IEnumerable<Order> ReadAllOrdersByCustomer(int id);
        Order ReadOrderByID(int id);

        Order DeleteOrder(int id);
    }
}
