using EB.Core.ApplicationServices;
using EB.Core.ApplicationServices.Impl;
using EB.Core.DomainServices;
using EB.Core.Entities;
using EB.Core.Entities.Security;
using FluentAssertions;
using Moq;
using ProductShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace EB.UnitTests
{
    public class UserServiceTest
    {
        private SortedDictionary<int, User> userDatabase;
        private Mock<IUserRepository> repoMock;
        private Mock<IAuthenticationHelper> authMock;
        private Mock<IValidator> validatorMock;

        public UserServiceTest()
        {
            userDatabase = new SortedDictionary<int, User>();
            repoMock = new Mock<IUserRepository>();
            authMock = new Mock<IAuthenticationHelper>();
            validatorMock = new Mock<IValidator>();

            repoMock.Setup(repo => repo.AddUser(It.IsAny<User>())).Callback<User>(user => userDatabase.Add(user.ID, user));
            repoMock.Setup(repo => repo.UpdateUser(It.IsAny<User>())).Callback<User>(user => userDatabase[user.ID] = user);
            repoMock.Setup(repo => repo.DeleteUser(It.IsAny<int>())).Callback<int>(id => userDatabase.Remove(id));
            repoMock.Setup(repo => repo.ReadUsers()).Returns(() => userDatabase.Values);
            repoMock.Setup(repo => repo.GetUserByID(It.IsAny<int>())).Returns<int>((id) => userDatabase.ContainsKey(id) ? userDatabase[id] : null);
        }

        [Fact]
        public void CreateUserService_UserRepositoryValidatorAndAuthenticationHelperIsNull_ExpectNullReferenceException()
        {
            // arrange
            UserService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new UserService(null as IUserRepository, null as IAuthenticationHelper, null as IValidator));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateUserService_UserRepositoryIsNull_ExpectNullReferenceException()
        {
            // arrange
            UserService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new UserService(null as IUserRepository, authMock.Object, validatorMock.Object));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateUserService_AuthHelperIsNull_ExpectNullReferenceException()
        {
            // arrange
            UserService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new UserService(repoMock.Object, null, validatorMock.Object));

            Assert.Equal("Authenticationhelper can't be null", ex.Message);
        }

        [Fact]
        public void CreateUserService_ValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            UserService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new UserService(repoMock.Object, authMock.Object, null));

            Assert.Equal("Validator can't be null", ex.Message);
        }

        [Fact]
        public void CreateUserService_ValidUserRepositoryAuthHelperAndValidator()
        {
            // arrange
            UserService service = null;

            // act + assert
            service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            Assert.NotNull(service);
        }

        [Fact]
        public void UserService_ShouldBeOfTypeIUserService()
        {
            // arrange

            // act + assert
            new UserService(repoMock.Object, authMock.Object, validatorMock.Object).Should().BeAssignableTo<IUserService>();
        }

        [Fact]
        public void UserServiceLogin_LoginWithNullImportModel_ExceptUnauthorizedAccessException()
        {
            // arrange
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);
            LoginInputModel model = null;

            // act + assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => service.Login(model));
            Assert.Equal("Username or Password is non-existing", ex.Message);

            authMock.Verify(auth => auth.ValidateLogin(It.IsAny<User>(), It.Is<LoginInputModel>(lm => lm == model)), Times.Never);
            repoMock.Verify(repo => repo.ReadUsers(), Times.Never);
        }

        [Fact]
        public void UserServiceLogin_LoginWithInvalidImportModel_ExceptUnauthorizedAccessException()
        {
            // arrange
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);
            LoginInputModel model = new LoginInputModel {Username = "", Password = null };

            // act + assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => service.Login(model));
            Assert.Equal("Username or Password is non-existing", ex.Message);

            authMock.Verify(auth => auth.ValidateLogin(It.IsAny<User>(), It.Is<LoginInputModel>(lm => lm == model)), Times.Never);
            repoMock.Verify(repo => repo.ReadUsers(), Times.Never);
        }

        [Fact]
        public void UserServiceLogin_LoginWithValidImportModelNoResults_ExceptUnauthorizedAccessException()
        {
            // arrange
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);
            LoginInputModel model = new LoginInputModel { Username = "Christian", Password = "1234" };


            // act + assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => service.Login(model));
            Assert.Equal("No user registered with such a name", ex.Message);

            authMock.Verify(auth => auth.ValidateLogin(It.IsAny<User>(), It.Is<LoginInputModel>(lm => lm == model)), Times.Never);
            repoMock.Verify(repo => repo.ReadUsers(), Times.Once);
        }

        [Fact]
        public void UserServiceLogin_LoginWithValidImportModelWithResults()
        {
            // arrange
            LoginInputModel model = new LoginInputModel { Username = "Christian", Password = "1234" };
            User user1 = new User { ID = 1, Username = "Christian" };
            User user2 = new User { ID = 2, Username = "Hans" };

            userDatabase.Add(user1.ID, user1);
            userDatabase.Add(user2.ID, user2);

            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);


            // act + assert
            User foundUser = service.Login(model);


            authMock.Verify(auth => auth.ValidateLogin(It.Is<User>(user => user == foundUser), It.Is<LoginInputModel>(lm => lm == model)), Times.Once);
            repoMock.Verify(repo => repo.ReadUsers(), Times.Once);
        }


        [Fact]
        public void UserServiceGenerateJWTToken_WithNullUser_ExceptArgumentException()
        {
            // arrange
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);
            User foundUser = null;

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.GenerateJWTToken(foundUser));
            Assert.Equal("User can't be null", ex.Message);

            authMock.Verify(auth => auth.GenerateJWTToken(It.Is<User>(user => user == foundUser)), Times.Never);
        }

        [Theory]
        [InlineData(1, new byte[] { }, null)]
        [InlineData(2, null, new byte[] { })]
        public void UserServiceGenerateJWTToken_WithInvalidUser_ExceptArgumentException(int id, byte[] salt, byte[] password)
        {
            // arrange
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);
            User foundUser = new User {ID = 1, Salt = salt, Password = password };

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.GenerateJWTToken(foundUser));
            Assert.Equal("User is missing a password or salt", ex.Message);

            authMock.Verify(auth => auth.GenerateJWTToken(It.Is<User>(user => user == foundUser)), Times.Never);
        }

        [Fact]
        public void UserServiceGenerateJWTToken_WithValidUser()
        {
            // arrange
            authMock.Setup(repo => repo.GenerateJWTToken(It.IsAny<User>())).Returns(() => "correct login");
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);
            User foundUser = new User { ID = 1, Salt = new byte[]{ }, Password = new byte[]{ } };

            // act + assert
            string token = service.GenerateJWTToken(foundUser);

            Assert.Equal("correct login", token);
            authMock.Verify(auth => auth.GenerateJWTToken(It.Is<User>(user => user == foundUser)), Times.Once);
        }

        [Fact]
        public void UserServiceValidate_ShouldValidateUserWithParameter_Once()
        {
            // arrange
            UserService service = null;
            string userName = "Hans";
            string password = "1234";
            string userRole = "Admin";
            

            // act + assert
            service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);
            service.ValidateUser(userName, password, userRole);
            validatorMock.Verify(validator => validator.ValidateCreateUser(It.Is<string>(un => un == userName), It.Is<string>(pw => pw == password), It.Is<string>(ur => ur == userRole)), Times.Once);
        }

        [Theory]
        [InlineData("Hans", null, null, "password can't be null or empty")]
        [InlineData(null, "", "user", "username can't be null or empty")]
        public void UserServiceCreateUser_WithInvalidData_ExceptInvalidArgumentException(string username, string password, string role, string errorMessage)
        {
            // arrange
            validatorMock.Setup(repo => repo.ValidateCreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback(() => throw new ArgumentException(errorMessage));
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.CreateUser(username, password, role));
            Assert.Equal(errorMessage, ex.Message);

            validatorMock.Verify(validator => validator.ValidateCreateUser(It.Is<string>(un => un == username), It.Is<string>(pw => pw == password), It.Is<string>(ur => ur == role)), Times.Once);
        }

        [Fact]
        public void UserServiceCreateUser_WithValidData()
        {
            // arrange
            authMock.Setup(repo => repo.GenerateSalt()).Returns(() => new byte[] { });
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);
            string username = "Hans";
            string password = "4321";
            string role = "Admin";

            // act + assert
            User user = service.CreateUser(username, password, role);

            Assert.NotNull(user);
            validatorMock.Verify(validator => validator.ValidateCreateUser(It.Is<string>(un => un == username), It.Is<string>(pw => pw == password), It.Is<string>(ur => ur == role)), Times.Once);
            authMock.Verify(auth => auth.GenerateSalt(), Times.Once);
        }

        [Theory]
        [InlineData(1, "Kim")]
        [InlineData(2, "Ole")]
        public void AddUser_ValidUserType(int id, string username)
        {
            // arrange
            User user = new User()
            {
                ID = id,
                Username = username
            };

            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act
            service.AddUser(user);

            // assert
            Assert.Contains(user, userDatabase.Values);
            repoMock.Verify(repo => repo.AddUser(It.Is<User>(u => u == user)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateUser(It.Is<User>(u => u == user)), Times.Once);
        }

        [Theory]
        [InlineData(1, null, "Name is null exception")]
        [InlineData(2, "", "Name is null exception")]
        public void AddUser_InvalidUser_ExceptArgumentException(int id, string username, string errorExpected)
        {
            // arrange
            User user = new User()
            {
                ID = id,
                Username = username
            };

            validatorMock.Setup(mock => mock.ValidateUser(It.IsAny<User>())).Callback<User>(user => throw new ArgumentException(errorExpected));
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.AddUser(user));

            // assert
            Assert.Equal(errorExpected, ex.Message);
            Assert.DoesNotContain(user, userDatabase.Values);
            repoMock.Verify(repo => repo.AddUser(It.Is<User>(u => u == user)), Times.Never);
            validatorMock.Verify(validator => validator.ValidateUser(It.Is<User>(u => u == user)), Times.Once);
        }

        [Fact]
        public void AddUser_NullUser_ExceptArgumentException()
        {
            // arrange
            User user = null;

            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.AddUser(user));

            // assert
            Assert.Equal("User is invalid", ex.Message);
            Assert.DoesNotContain(user, userDatabase.Values);
            repoMock.Verify(repo => repo.AddUser(It.Is<User>(u => u == user)), Times.Never);
            validatorMock.Verify(validator => validator.ValidateUser(It.Is<User>(u => u == user)), Times.Never);
        }

        [Fact]
        public void AddUser_ValidDuplicatedUser_ExceptArgumentException()
        {
            // arrange
            User user = new User { ID = 1, Username = "Hans"};
            userDatabase.Add(user.ID, user);
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.AddUser(user));

            // assert
            Assert.Equal("User with same name already exists", ex.Message);
            repoMock.Verify(repo => repo.AddUser(It.Is<User>(u => u == user)), Times.Never);
            validatorMock.Verify(validator => validator.ValidateUser(It.Is<User>(u => u == user)), Times.Once);
        }

        [Fact]
        public void GetUserById_UserExists()
        {
            // arrange

            User user = new User { ID = 1 };
            userDatabase.Add(user.ID, user);

            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act
            var result = service.GetUserByID(user.ID);

            // assert
            Assert.Equal(user, result);
            repoMock.Verify(repo => repo.GetUserByID(It.Is<int>(id => id == user.ID)), Times.Once);
        }

        [Fact]
        public void GetUserById_UserDoesNotExist_ExpectNull()
        {
            // arrange
            User user1 = new User { ID = 1 };
            User user2 = new User { ID = 2 };

            userDatabase.Add(user2.ID, user2);

            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act
            var result = service.GetUserByID(user1.ID);

            // assert
            Assert.Null(result);
            repoMock.Verify(repo => repo.GetUserByID(It.Is<int>(ID => ID == user1.ID)), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetUserById_InvalidId_ExpectArgumentException(int ID)
        {
            // arrange
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.GetUserByID(ID));

            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repo => repo.GetUserByID(It.Is<int>(id => id == ID)), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void GetAllUsers(int userCount)
        {
            // arrange
            User user1 = new User { ID = 1 };
            User user2 = new User { ID = 2 };
            List<User> users = new List<User>() { user1, user2 };

            var expected = users.GetRange(0, userCount);
            foreach (var user in expected)
            {
                userDatabase.Add(user.ID, user);
            }

            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act
            var result = service.GetAllUsers();

            // assert
            Assert.Equal(expected, result);
            repoMock.Verify(repo => repo.ReadUsers(), Times.Once);
        }

        [Theory]
        [InlineData(1, "Kurt")]
        [InlineData(2, "Hans")]
        public void UpdateUser_ValidExistingUser(int id, string name)
        {
            // arrange
            User user = new User()
            {
                ID = id,
                Username = name,
            };

            userDatabase.Add(user.ID, user);

            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act
            service.UpdateUser(user);

            // assert
            repoMock.Verify(repo => repo.GetUserByID(It.Is<int>(ID => ID == user.ID)), Times.Once);
            repoMock.Verify(repo => repo.UpdateUser(It.Is<User>(u => u == user)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateUser(It.Is<User>(u => u == user)), Times.Once);
            Assert.Equal(repoMock.Object.GetUserByID(user.ID), user);
        }

        [Theory]
        [InlineData(1, null, "Name is null exception")]
        [InlineData(2, "", "Description is null exception")]
        public void UpdateUser_InvalidUser_ExpectArgumentException(int id, string name, string errorExpected)
        {
            // arrange
            User user = new User()
            {
                ID = id,
                Username = name
            };

            validatorMock.Setup(mock => mock.ValidateUser(It.IsAny<User>())).Callback<User>(user => throw new ArgumentException(errorExpected));
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateUser(user));

            Assert.Equal(errorExpected, ex.Message);
            Assert.Equal(repoMock.Object.GetUserByID(user.ID), null);

            repoMock.Verify(repo => repo.GetUserByID(It.Is<int>(ID => ID == user.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateUser(It.Is<User>(u => u == user)), Times.Once);
            repoMock.Verify(repo => repo.UpdateUser(It.Is<User>(u => u == user)), Times.Never);
        }

        [Fact]
        public void UpdateUser_UserIsNUll_ExpectArgumentException()
        {
            // arrange
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateUser(null));

            Assert.Equal("Updating user does not exist", ex.Message);
            validatorMock.Verify(validator => validator.ValidateUser(It.Is<User>(u => u == null)), Times.Never);
            repoMock.Verify(repo => repo.UpdateUser(It.Is<User>(u => u == null)), Times.Never);
        }

        [Fact]
        public void UpdateUser_UserDoesNotExist_InvalidOperationException()
        {
            // arrange
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            User user = new User()
            {
                ID = 1,
                Username = "Hans"
            };

            // act + assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.UpdateUser(user));

            Assert.Equal("No user with such ID found", ex.Message);
            repoMock.Verify(repo => repo.GetUserByID(It.Is<int>(ID => ID == user.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateUser(It.Is<User>(u => u == user)), Times.Once);
            repoMock.Verify(repo => repo.UpdateUser(It.Is<User>(u => u == user)), Times.Never);
        }

        [Fact]
        public void RemoveUser_ValidExistingUser()
        {
            // arrange
            User user = new User()
            {
                ID = 1,
                Username = "Hans"
            };

            userDatabase.Add(user.ID, user);

            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act
            service.DeleteUser(user.ID);

            // assert
            repoMock.Verify(repo => repo.GetUserByID(It.Is<int>(ID => ID == user.ID)), Times.Once);
            repoMock.Verify(repo => repo.DeleteUser(It.Is<int>(ID => ID == user.ID)), Times.Once);
            Assert.Null(repoMock.Object.GetUserByID(user.ID));
        }

        [Fact]
        public void RemoveUser_UserDoesNotExist_ExpectArgumentException()
        {
            // arrange

            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            User user = new User()
            {
                ID = 1,
                Username = "Lone"
            };

            // act + assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.DeleteUser(user.ID));

            // assert
            Assert.Equal("No user with such ID found", ex.Message);
            repoMock.Verify(repo => repo.GetUserByID(It.Is<int>(ID => ID == user.ID)), Times.Once);
            repoMock.Verify(repo => repo.DeleteUser(It.Is<int>(ID => ID == user.ID)), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void RemoveUser_IncorrectID_ExpectArgumentException(int ID)
        {
            // arrange
            UserService service = new UserService(repoMock.Object, authMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.DeleteUser(ID));

            // assert
            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repo => repo.DeleteUser(It.Is<int>(id => id == ID)), Times.Never);
            repoMock.Verify(repo => repo.GetUserByID(It.Is<int>(id => id == ID)), Times.Never);
        }
    }
}
