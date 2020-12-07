using EB.Core.DomainServices;
using EB.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EB.Infrastructure.Data
{
    public class CustomerRepository: ICustomerRepository
    {
        #region Dependency Injection
        private EBContext ctx;

        public CustomerRepository(EBContext ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region Create Data
        public Customer AddCustomer(Customer customer)
        {
            ctx.Attach(customer).State = EntityState.Added;
            ctx.SaveChanges();
            return customer;
        }
        #endregion

        #region Read Data
        public Customer ReadCustomerById(int id)
        {
            return ctx.Costumers.FirstOrDefault(x => x.ID == id);
        }
        #endregion

        #region Update Data
        public Customer UpdateCustomerInRepo(Customer customer)
        {
            ctx.Attach(customer).State = EntityState.Modified;
            ctx.SaveChanges();

            return ReadCustomerById(customer.ID);
        }
        #endregion
    }
}
