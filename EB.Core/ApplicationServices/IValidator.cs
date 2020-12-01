using EB.Core.Entities;
using EB.Core.Entities.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.ApplicationServices
{
    public interface IValidator
    {
        public void ValidateBeer(Beer beer);
        public void ValidateType(BeerType type);
        public void ValidateBrand(Brand brand);
        public void ValidateCustomer(Customer customer);
        public void ValidateCreateUser(string userName, string password, string userRole);
        public void ValidateUser(User user);
        public void ValidateOrder(Order order);
    }
}
