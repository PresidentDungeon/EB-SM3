using EB.Core.Entities.Security;
using System.Collections.Generic;

namespace EB.Core.DomainServices
{
    public interface IUserRepository
    {
        //Create Data
        User AddUser(User user);

        //Read Data
        IEnumerable<User> ReadUsers();
        User GetUserByID(int ID);

        //Update Data
        User UpdateUser(User user);

        //Delete Data
        User DeleteUser(int ID);
    }
}
