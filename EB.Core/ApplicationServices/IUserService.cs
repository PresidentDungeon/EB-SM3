using EB.Core.Entities.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.ApplicationServices
{
    public interface IUserService
    {
        User Login(LoginInputModel inputModel);

        string GenerateJWTToken(User foundUser);

        bool ValidateUser(string userName, string password, string userRole);

        User CreateUser(string userName, string password, string userRole);

        User AddUser(User user);

        List<User> GetAllUsers();

        User GetUserByID(int ID);

        User UpdateUser(User user);

        User UpdatePassword(int id, UpdatePasswordModel updateModel);

        User DeleteUser(int ID);
    }
}
