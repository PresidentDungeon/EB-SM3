using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
