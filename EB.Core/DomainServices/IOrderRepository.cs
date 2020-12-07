using EB.Core.Entities;
using ProductShop.Core.Entities;

namespace EB.Core.DomainServices
{
    public interface IOrderRepository
    {
        //Create Data
        Order AddOrder(Order order);

        //Read Data
        FilterList<Order> ReadAllOrders(Filter filter);
        FilterList<Order> ReadAllOrdersByCustomer(int id, Filter filter);
        Order ReadOrderByID(int id);
        Order ReadOrderByIDUser(int orderID, int userID);

        //Update Data
        Order UpdateOrder(Order order);

        //Delete Data
        Order DeleteOrder(int id);
    }
}
