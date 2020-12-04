using EB.Core.Entities;
using EB.Core.Entities.Security;
using System;
using System.Text.RegularExpressions;

namespace EB.Core.ApplicationServices.Validators
{
    public class BEValidator : IValidator
    {

        Regex emailRegex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");

        public void ValidateBeer(Beer beer)
        {
            if(beer == null)
            {
                throw new ArgumentException("Beer instance can't be null");
            }
            if (beer.ID < 0)
            {
                throw new ArgumentException("Invalid ID");
            }
            if (string.IsNullOrEmpty(beer.Name))
            {
                throw new ArgumentException("Name can not be empty");
            }
            if (string.IsNullOrEmpty(beer.Description))
            {
                throw new ArgumentException("Description can not be empty");
            }
            if (beer.Price <= 0)
            {
                throw new ArgumentException("Price must be higher than zero");
            }
            if (beer.EBC < 0 || beer.EBC > 80)
            {
                throw new ArgumentException("EBC must be betweeen 0-80");
            }
            if (beer.IBU < 0 || beer.IBU > 120)
            {
                throw new ArgumentException("IBU must be betweeen 0-120");
            }

            if (beer.Percentage < 0 || beer.Percentage > 100)
            {
                throw new ArgumentException("Percentage must be between 0-100");
            }
            if (beer.Stock < 0)
            {
                throw new ArgumentException("Stock must be a whole number above or equal to zero");
            }
            if (beer.Brand == null)
            {
                throw new ArgumentException("A brand must be selected");
            }
            if (beer.Type == null)
            {
                throw new ArgumentException("A type must be selected");
            }
        }

        public void ValidateBrand(Brand brand)
        {
            if (brand == null)
            {
                throw new ArgumentException("Brand instance can't be null");
            }
            if (brand.ID < 0)
            {
                throw new ArgumentException("Invalid ID");
            }
            if (string.IsNullOrEmpty(brand.BrandName))
            {
                throw new ArgumentException("Name can not be empty");
            }
        }

        public void ValidateCreateUser(string userName, string password, string userRole)
        {
            if (string.IsNullOrEmpty(userName) || userName.Length < 8 || userName.Length > 24)
            {
                throw new ArgumentException("Username must be be between 8-24 characters");
            }
            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                throw new ArgumentException("Password must be minimum 8 characters");
            }
            if (string.IsNullOrEmpty(userRole))
            {
                throw new ArgumentException("Userrole can't be null or empty");
            }
        }

        public void ValidateUser(User user)
        {
            if (user.ID < 0)
            {
                throw new ArgumentException("Invalid ID");
            }
            if (string.IsNullOrEmpty(user.Username) || user.Username.Length < 8 || user.Username.Length > 24)
            {
                throw new ArgumentException("Username must be be between 8-24 characters");
            }
            if (string.IsNullOrEmpty(user.UserRole))
            {
                throw new ArgumentException("Userrole can't be null or empty");
            }
            if (user.Salt == null || user.Salt.Length <= 0)
            {
                throw new ArgumentException("User salt cannot be null or empty");
            }
            if (user.Password == null || user.Password.Length <= 0)
            {
                throw new ArgumentException("User password cannot be null or empty");
            }
        }


        public void ValidateCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentException("Customer instance can't be null");
            }
            if (customer.ID < 0)
            {
                throw new ArgumentException("Invalid ID");
            }
            if (string.IsNullOrEmpty(customer.FirstName))
            {
                throw new ArgumentException("Firstname can not be empty");
            }
            if (string.IsNullOrEmpty(customer.LastName))
            {
                throw new ArgumentException("Lastname can not be empty");
            }
            if (string.IsNullOrEmpty(customer.Email))
            {
                throw new ArgumentException("Email can not be empty");
            }
            if (!emailRegex.IsMatch(customer.Email))
            {
                throw new ArgumentException("Email must be a valid email");
            }
            if (string.IsNullOrEmpty(customer.StreetName))
            {
                throw new ArgumentException("Streetname can not be empty");
            }
            if (customer.PostalCode < 0 || customer.PostalCode > 9999)
            {
                throw new ArgumentException("Postalcode must be between 0-9999");
            }
            if (string.IsNullOrEmpty(customer.CityName))
            {
                throw new ArgumentException("Cityname can not be empty");
            }
        }

        public void ValidateType(BeerType type)
        {
            if (type == null)
            {
                throw new ArgumentException("Type instance can't be null");
            }
            if (type.ID < 0)
            {
                throw new ArgumentException("Invalid ID");
            }
            if (string.IsNullOrEmpty(type.TypeName))
            {
                throw new ArgumentException("Name can not be empty");
            }
        }

        public void ValidateOrder(Order order)
        {
            if (order == null)
            {
                throw new ArgumentException("Order instance can't be null");
            }
            if (order.ID < 0)
            {
                throw new ArgumentException("Invalid ID");
            }
            if (order.AccumulatedPrice <= 0)
            {
                throw new ArgumentException("Price must be higher than zero");
            }
            if (order.OrderCreated.Date == new DateTime().Date)
            {
                throw new ArgumentException("No order attached");
            }
            if (order.Customer == null)
            {
                throw new ArgumentException("Invalid customer");
            }
            if (order.OrderBeers == null ||order.OrderBeers.Count <= 0)
            {
                throw new ArgumentException("Can not process order with no products");
            }





        }
    }
}
