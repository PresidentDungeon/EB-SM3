using EB.Core.Entities;
using EB.Core.Entities.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.ApplicationServices.Validators
{
    public class BEValidator : IValidator
    {
        public void ValidateBeer(Beer beer)
        {
            throw new NotImplementedException();
        }

        public void ValidateBrand(Brand brand)
        {
            throw new NotImplementedException();
        }

        public void ValidateCreateUser(string userName, string password, string userRole)
        {
            throw new NotImplementedException();
        }

        public void ValidateCustomer(Customer customer)
        {
            throw new NotImplementedException();
        }

        public void ValidateOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public void ValidateType(BeerType type)
        {
            throw new NotImplementedException();
        }

        public void ValidateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
