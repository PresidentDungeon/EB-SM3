using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.ApplicationServices
{
    public interface IOrderService
    {
        Order AddOrder(Order order);
        List<Order> ReadAllOrders();
        List<Order> ReadAllOrdersByCustomer(int id);
        Order ReadOrderByID(int id);
        Order DeleteOrder(int id);
    }
}
