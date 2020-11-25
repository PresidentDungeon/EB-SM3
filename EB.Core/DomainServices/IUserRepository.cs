using EB.Core.Entities.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.DomainServices
{
    public interface IUserRepository
    {
        User AddUser(User user);
        IEnumerable<User> ReadUsers();
        User GetUserByID(int ID);
        User UpdateUser(User user);
        User DeleteUser(int ID);
    }
}
