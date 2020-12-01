using EB.Core.DomainServices;
using EB.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB.Infrastructure.Data
{
    public class CustomerRepository: ICustomerRepository
    {
        private EBContext ctx;

        public CustomerRepository(EBContext ctx)
        {
            this.ctx = ctx;
        }

        public Customer AddCustomer(Customer customer)
        {
            ctx.Attach(customer).State = EntityState.Added;
            ctx.SaveChanges();
            return customer;
        }

        public Customer ReadCustomerById(int id)
        {
            return ctx.Costumers.FirstOrDefault(x => x.ID == id);
        }

        public Customer UpdateCustomerInRepo(Customer customer)
        {
            ctx.Attach(customer).State = EntityState.Modified;
            ctx.SaveChanges();

            return ReadCustomerById(customer.ID);
        }
    }
}
