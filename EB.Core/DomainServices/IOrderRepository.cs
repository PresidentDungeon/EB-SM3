using EB.Core.Entities;
using ProductShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.DomainServices
{
    public interface IOrderRepository
    {
        Order AddOrder(Order order);
        FilterList<Order> ReadAllOrders(Filter filter);
        FilterList<Order> ReadAllOrdersByCustomer(int id, Filter filter);
        Order ReadOrderByID(int id);
        Order ReadOrderByIDUser(int orderID, int userID);
        Order UpdateOrder(Order order);
        Order DeleteOrder(int id);
    }
}
