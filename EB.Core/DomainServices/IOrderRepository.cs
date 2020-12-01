using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.DomainServices
{
    public interface IOrderRepository
    {
        //Create Data
        Order CreateOrder(Order order);

        //Read Data
        IEnumerable<Order> ReadAllOrders();
        Order ReadOrderById(int id);

        /* Updating orders could get messy - maybe better to only create and delete
        //Update Data
        Order UpdateOrderInRepo(Order order);
        */

        //Delete Data
        Order DeleteOrderInRepo(int id);
    }
}
