using EB.Core.Entities;

namespace EB.Core.ApplicationServices
{
    public interface ICustomerService
    {
        //Create
        Customer ValidateCustomer(Customer customer);
        Customer CreateCustomer(Customer customer);

        //Read
        Customer GetCustomerById(int id);

        //Update
        Customer UpdateCustomer(Customer customer);
    }
}
