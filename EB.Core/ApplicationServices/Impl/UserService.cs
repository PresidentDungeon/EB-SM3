using EB.Core.DomainServices;
using EB.Core.Entities.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EB.Core.ApplicationServices.Impl
{
    public class UserService: IUserService
    {
        private IUserRepository UserRepository;
        private IAuthenticationHelper AuthenticationHelper;
        private IValidator Validator;

        public UserService(IUserRepository userRepository, IAuthenticationHelper authenticationHelper, IValidator validator)
        {
            this.UserRepository = userRepository ?? throw new NullReferenceException("Repository can't be null");
            this.AuthenticationHelper = authenticationHelper ?? throw new NullReferenceException("Authenticationhelper can't be null");
            this.Validator = validator ?? throw new NullReferenceException("Validator can't be null");
        }


        public User Login(LoginInputModel inputModel)
        {
            if (inputModel == null || inputModel.Username == null || inputModel.Password == null)
            {
                throw new UnauthorizedAccessException("Username or Password is non-existing");
            }

            User foundUser = GetAllUsers().FirstOrDefault(u => u.Username.Equals(inputModel.Username));

            if (foundUser == null)
            {
                throw new UnauthorizedAccessException("No user registered with such a name");
            }

            AuthenticationHelper.ValidateLogin(foundUser, inputModel);

            return foundUser;
        }

        public string GenerateJWTToken(User foundUser)
        {
            if(foundUser == null)
            {
                throw new ArgumentException("User can't be null");
            }
            if(foundUser.Password == null || foundUser.Salt == null)
            {
                throw new ArgumentException("User is missing a password or salt");
            }
            return AuthenticationHelper.GenerateJWTToken(foundUser);
        }

        public bool ValidateUser(string userName, string password, string userRole)
        {
           Validator.ValidateCreateUser(userName, password, userRole);
           return true;
        }

        public User CreateUser(string userName, string password, string userRole)
        {
            ValidateUser(userName, password, userRole);
            byte[] generatedSalt = AuthenticationHelper.GenerateSalt();

            return new User
            {
                Username = userName,
                Salt = generatedSalt,
                Password = AuthenticationHelper.GenerateHash(password, generatedSalt),
                UserRole = userRole
            };
        }

        public User AddUser(User user)
        {
            if (user != null)
            {
                Validator.ValidateUser(user);

                if ((from x in GetAllUsers() where x.Username.ToLower().Equals(user.Username.ToLower()) select x).Count() > 0)
                {
                    throw new ArgumentException("User with same name already exists");
                }
                return UserRepository.AddUser(user);
            }
            throw new ArgumentException("User is invalid");
        }

        public List<User> GetAllUsers()
        {
            return UserRepository.ReadUsers().ToList();
        }

        public User GetUserByID(int ID)
        {
            if (ID <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }

            return UserRepository.GetUserByID(ID);
        }

        public User UpdateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentException("Updating user does not exist");
            }
            Validator.ValidateUser(user);
            if (GetUserByID(user.ID) == null)
            {
                throw new InvalidOperationException("No user with such ID found");
            }

            return UserRepository.UpdateUser(user);
        }

        public User DeleteUser(int ID)
        {
            if (ID <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }
            if (GetUserByID(ID) == null)
            {
                throw new InvalidOperationException("No user with such ID found");
            }
            return UserRepository.DeleteUser(ID);
        }
    }
}
