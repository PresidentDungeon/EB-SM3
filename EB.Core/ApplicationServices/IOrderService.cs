using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.ApplicationServices
{
    public interface IOrderService
    {
        //Create
        Order CreateOrder(Order order);

        //Read
        List<Order> GetAllOrders();
        Order GetOrderById(int id);

        //Delete
        Order DeleteOrder(int id);
    }
}
