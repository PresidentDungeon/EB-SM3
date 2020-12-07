using EB.Core.Entities;
using ProductShop.Core.Entities;

namespace EB.Core.ApplicationServices
{
    public interface IOrderService
    {
        //Create
        Order AddOrder(Order order);

        //Read
        FilterList<Order> ReadAllOrders(Filter filter);
        FilterList<Order> ReadAllOrdersByCustomer(int id, Filter filter);
        Order ReadOrderByID(int id);
        Order ReadOrderByIDUser(int orderID, int userID);

        //Update
        Order UpdateOrderStatus(int orderID);

        //Delete
        Order DeleteOrder(int id);
    }
}
