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
            if (order == null) {
                throw new ArgumentException("Order can not be null");
            }
            if (order.ID < 0)
            {
                throw new ArgumentException("Invalid ID");
            }
            if (order.AccumulatedPrice < 0)
            {
                throw new ArgumentException("Invalid Price");
            }
            if (order.Beers.Length < 0)
            {
                throw new ArgumentException("Order has to have Items to buy");
            }
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
