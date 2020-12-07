using EB.Core.Entities.Security;
using System.Collections.Generic;

namespace EB.Core.Entities
{
    public class Customer
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetName { get; set; }
        public int PostalCode { get; set; }
        public string CityName { get; set; }
        public User User { get; set; }
        public List<Order> Orders { get; set; }
    }
}
