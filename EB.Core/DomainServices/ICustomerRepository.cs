using EB.Core.Entities;

namespace EB.Core.DomainServices
{
    public interface ICustomerRepository
    {
        //Create Data
        Customer AddCustomer(Customer customer);

        //Read Data
        Customer ReadCustomerById(int id);

        //Update Data
        Customer UpdateCustomerInRepo(Customer customer);
    }
}
