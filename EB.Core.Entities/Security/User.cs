using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.Entities.Security
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public string UserRole { get; set; }
        public Customer Customer { get; set; }
    }
}
