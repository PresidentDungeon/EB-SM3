using EB.Core.DomainServices;
using EB.Core.Entities;
using System;

namespace EB.Core.ApplicationServices.Impl
{
    public class CustomerService: ICustomerService
    {
        #region Dependency Injection
        private ICustomerRepository CustomerRepository;
        private IValidator Validator;

        public CustomerService(ICustomerRepository customerRepository, IValidator validator)
        {
            this.CustomerRepository = customerRepository ?? throw new NullReferenceException("Repository can't be null");
            this.Validator = validator ?? throw new NullReferenceException("Validator can't be null");
        }
        #endregion

        #region Validate
        public Customer ValidateCustomer(Customer customer)
        {
            this.Validator.ValidateCustomer(customer);
            return customer;
        }
        #endregion

        #region Create
        public Customer CreateCustomer(Customer customer)
        {
            if (customer != null)
            {
                Validator.ValidateCustomer(customer);
                return CustomerRepository.AddCustomer(customer);
            }
            return null;
        }
        #endregion

        #region Read
        public Customer GetCustomerById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }

            return CustomerRepository.ReadCustomerById(id);
        }
        #endregion

        #region Update
        public Customer UpdateCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentException("Updating customer does not exist");
            }
            Validator.ValidateCustomer(customer);
            if (GetCustomerById(customer.ID) == null)
            {
                throw new InvalidOperationException("No customer with such ID found");
            }

            return CustomerRepository.UpdateCustomerInRepo(customer);
        }
        #endregion
    }
}
