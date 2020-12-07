using EB.Core.Entities.Security;
using System.Collections.Generic;

namespace EB.Core.ApplicationServices
{
    public interface IUserService
    {
        //Security
        User Login(LoginInputModel inputModel);
        string GenerateJWTToken(User foundUser);
        bool ValidateUser(string userName, string password, string userRole);

        //Create
        User CreateUser(string userName, string password, string userRole);
        User AddUser(User user);

        //Read
        List<User> GetAllUsers();
        User GetUserByID(int ID);

        //Update
        User UpdateUser(User user);
        User UpdatePassword(int id, UpdatePasswordModel updateModel);

        //Delete
        User DeleteUser(int ID);
    }
}
