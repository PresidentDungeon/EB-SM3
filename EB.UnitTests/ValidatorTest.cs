using EB.Core.ApplicationServices;
using EB.Core.ApplicationServices.Validators;
using EB.Core.Entities;
using EB.Core.Entities.Security;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using Xunit;

namespace EB.UnitTests
{
    public class ValidatorTest
    {
        [Fact]
        public void BEValidator_IsOfTypeIValidator()
        {
            new BEValidator().Should().BeAssignableTo<IValidator>();
        }

        #region AddBeer
        [Fact]
        public void AddBeer_InvalidBeerNull_ExceptArgumentException()
        {
            // arrange
            Beer beer = null;
            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateBeer(beer));

            Assert.Equal("Beer instance can't be null", ex.Message);
        }

        [Theory]
        [InlineData(-1, "Beer", "Fra 1884!", 65, 50, 100, 14, 500, "image.png", "Invalid ID")]                                                    // invalid id: -1
        [InlineData(int.MinValue, "Beer", "Fra 1884!", 65, 50, 100, 14, 500, "image.png", "Invalid ID")]                                          // invalid id: -2147483648
        [InlineData(1, null, "Fra 1884!", 65, 50, 100, 14, 500, "image.png", "Name can not be empty")]                                            // invalid name: null
        [InlineData(1, "", "Fra 1884!", 65, 50, 100, 14, 500, "image.png", "Name can not be empty")]                                              // invalid name: ""
        [InlineData(1, "Beer", null, 65, 50, 100, 14, 500, "image.png", "Description can not be empty")]                                          // invalid description: null
        [InlineData(1, "Beer", "", 65, 50, 100, 14, 500, "image.png", "Description can not be empty")]                                            // invalid description: ""
        [InlineData(1, "Beer", "Fra 1884!", 0, 50, 100, 14, 500, "image.png", "Price must be higher than zero")]                                  // invalid price: 0
        [InlineData(1, "Beer", "Fra 1884!", -1, 50, 100, 14, 500, "image.png", "Price must be higher than zero")]                                 // invalid price: -1
        [InlineData(1, "Beer", "Fra 1884!", 65, -1, 100, 14, 500, "image.png", "EBC must be betweeen 0-80")]                                      // invalid EBC: -1
        [InlineData(1, "Beer", "Fra 1884!", 65, 81, 100, 14, 500, "image.png", "EBC must be betweeen 0-80")]                                      // invalid EBC: 81
        [InlineData(1, "Beer", "Fra 1884!", 65, 50, -1, 14, 500, "image.png", "IBU must be betweeen 0-120")]                                      // invalid IBU: -1
        [InlineData(1, "Beer", "Fra 1884!", 65, 50, 121, 14, 500, "image.png", "IBU must be betweeen 0-120")]                                     // invalid IBU: 121
        [InlineData(1, "Beer", "Fra 1884!", 65, 50, 100, -1, 500, "image.png", "Percentage must be between 0-100")]                               // invalid percentage: -1
        [InlineData(1, "Beer", "Fra 1884!", 65, 50, 100, 101, 500, "image.png", "Percentage must be between 0-100")]                              // invalid percentage: 101
        [InlineData(1, "Beer", "Fra 1884!", 65, 50, 100, 14, -1, "image.png", "Stock must be a whole number above or equal to zero")]             // invalid stock: -1
        [InlineData(1, "Beer", "Fra 1884!", 65, 50, 100, 14, int.MinValue, "image.png", "Stock must be a whole number above or equal to zero")]   // invalid stock: -2147483648

        public void AddBeer_InvalidBeer_ExceptArgumentException(int id, string name, string description, double price, double EBC, double IBU, double percentage, int stock, string imageURL, string expectedErrorMsg)
        {
            // arrange
            Brand brand = new Brand { ID = 1 };
            BeerType type = new BeerType { ID = 1 };

            Beer beer = new Beer
            {
                ID = id,
                Name = name,
                Description = description,
                Price = price,
                EBC = EBC,
                IBU = IBU,
                Percentage = percentage,
                Stock = stock,
                ImageURL = imageURL,
                Brand = brand,
                Type = type
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateBeer(beer));

            Assert.Equal(expectedErrorMsg, ex.Message);
        }

        [Fact]
        public void AddBeer_InvalidBeerTypeNull_ExceptArgumentException()
        {
            // arrange
            Brand brand = new Brand { ID = 1 };
            BeerType type = null;

            Beer beer = new Beer
            {
                ID = 1,
                Name = "Beer",
                Description = "From 1884!",
                Price = 65,
                EBC = 60,
                IBU = 40,
                Percentage = 6.5,
                Stock = 49,
                ImageURL = "image.png",
                Brand = brand,
                Type = type
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateBeer(beer));

            Assert.Equal("A type must be selected", ex.Message);
        }

        [Fact]
        public void AddBeer_InvalidBeerBrandNull_ExceptArgumentException()
        {
            // arrange
            Brand brand = null;
            BeerType type = new BeerType { ID = 1 };

            Beer beer = new Beer
            {
                ID = 1,
                Name = "Beer",
                Description = "From 1884!",
                Price = 65,
                EBC = 60,
                IBU = 40,
                Percentage = 6.5,
                Stock = 60,
                ImageURL = "image.png",
                Brand = brand,
                Type = type
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateBeer(beer));

            Assert.Equal("A brand must be selected", ex.Message);
        }

        [Theory]
        [InlineData(1, "Beer", "Fra 1884!", 65, 50, 100, 14, 500, "image.png")]
        [InlineData(2, "Beer2", "Fra 1882!", 200, 20, 35, 4, int.MaxValue, "")]
        [InlineData(2, "Beer2", "Fra 1882!", 200, 20, 35, 4, 0, null)]
        public void AddBeer_ValidBeer(int id, string name, string description, double price, double EBC, double IBU, double percentage, int stock, string ImageURL)
        {
            // arrange
            Brand brand = new Brand { ID = 1 };
            BeerType type = new BeerType { ID = 1 };

            Beer beer = new Beer
            {
                ID = id,
                Name = name,
                Description = description,
                Price = price,
                EBC = EBC,
                IBU = IBU,
                Percentage = percentage,
                ImageURL = ImageURL,
                Brand = brand,
                Type = type
            };

            IValidator validator = new BEValidator();

            // act + assert
            validator.ValidateBeer(beer);
        }
        #endregion

        #region AddType
        [Fact]
        public void AddType_InvalidTypeNull_ExceptArgumentException()
        {
            // arrange
            BeerType type = null;
            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateType(type));

            Assert.Equal("Type instance can't be null", ex.Message);
        }

        [Theory]
        [InlineData(-1, "IPA", "Invalid ID")]               // invalid id: 0
        [InlineData(int.MinValue, "IPA", "Invalid ID")]     // invalid id: -2147483648             
        [InlineData(1, null, "Name can not be empty")]      // invalid name: null              
        [InlineData(1, "", "Name can not be empty")]        // invalid name: empty         

        public void AddType_InvalidType_ExceptArgumentException(int id, string name, string expectedErrorMsg)
        {
            // arrange
            BeerType type = new BeerType { ID = id, TypeName = name };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateType(type));

            Assert.Equal(expectedErrorMsg, ex.Message);
        }

        [Theory]
        [InlineData(1, "IPA")]
        [InlineData(2, "Stout")]
        public void AddType_ValidType(int id, string name)
        {
            // arrange
            BeerType type = new BeerType { ID = id, TypeName = name };

            IValidator validator = new BEValidator();

            // act + assert
            validator.ValidateType(type);
        }
        #endregion

        #region AddBrand
        [Fact]
        public void AddBrand_InvalidBrandNull_ExceptArgumentException()
        {
            // arrange
            Brand brand = null;
            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateBrand(brand));

            Assert.Equal("Brand instance can't be null", ex.Message);
        }

        [Theory]
        [InlineData(-1, "Ølværket", "Invalid ID")]              // invalid id: -1
        [InlineData(int.MinValue, "Ølværket", "Invalid ID")]    // invalid id: -2147483648             
        [InlineData(1, null, "Name can not be empty")]          // invalid name: null              
        [InlineData(1, "", "Name can not be empty")]            // invalid name: empty         

        public void AddBrand_InvalidBrand_ExceptArgumentException(int id, string name, string expectedErrorMsg)
        {
            // arrange
            Brand brand = new Brand { ID = id, BrandName = name };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateBrand(brand));

            Assert.Equal(expectedErrorMsg, ex.Message);
        }

        [Theory]
        [InlineData(1, "Ølværket")]
        [InlineData(2, "Esbjerg Bryghus")]
        public void AddBrand_ValidBrand(int id, string name)
        {
            // arrange
            Brand brand = new Brand { ID = id, BrandName = name };

            IValidator validator = new BEValidator();

            // act + assert
            validator.ValidateBrand(brand);
        }
        #endregion

        #region AddCustomer
        [Fact]
        public void AddCustomer_InvalidCustomerNull_ExceptArgumentException()
        {
            // arrange
            Customer customer = null;
            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateCustomer(customer));

            Assert.Equal("Customer instance can't be null", ex.Message);
        }

        [Theory]
        [InlineData(-1, "Karsten", "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Invalid ID")]                           // invalid id: -1
        [InlineData(int.MinValue, "Karsten", "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Invalid ID")]                 // invalid id: -2147483648
        [InlineData(1, null, "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Firstname can not be empty")]                 // invalid firstname: null
        [InlineData(1, "", "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Firstname can not be empty")]                   // invalid firstname: ""
        [InlineData(1, "Karsten", null, "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Lastname can not be empty")]                  // invalid lastname: null
        [InlineData(1, "Karsten", "", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Lastname can not be empty")]                    // invalid lastname: ""
        [InlineData(1, "Karsten", "Clausen", null, "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Email can not be empty")]                          // invalid email: null
        [InlineData(1, "Karsten", "Clausen", "", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Email can not be empty")]                            // invalid: ""
        [InlineData(1, "Karsten", "Clausen", "Carsten", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Email must be a valid email")]                 // invalid: "Carsten"
        [InlineData(1, "Karsten", "Clausen", "KC@gmail.com", "+45 20 20 20 90", null, 6771, "Esbjerg", "Streetname can not be empty")]                    // invalid streetname: null
        [InlineData(1, "Karsten", "Clausen", "KC@gmail.com", "+45 20 20 20 90", "", 6771, "Esbjerg", "Streetname can not be empty")]                      // invalid streetname: ""
        [InlineData(1, "Karsten", "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", -1, "Esbjerg", "Postalcode must be between 0-9999")]       // invalid postalcode: -1
        [InlineData(1, "Karsten", "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 10000, "Esbjerg", "Postalcode must be between 0-9999")]    // invalid postalcode: 10000
        [InlineData(1, "Karsten", "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, null, "Cityname can not be empty")]                  // invalid cityname: null
        [InlineData(1, "Karsten", "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "", "Cityname can not be empty")]                    // invalid cityname: ""

        public void AddCustomer_InvalidCustomer_ExceptArgumentException(int id, string firstName, string lastName, string email, string phoneNumber, string streetName, int postalCode, string cityName, string expectedErrorMsg)
        {
            // arrange
            Customer customer = new Customer
            {
                ID = id,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                StreetName = streetName,
                PostalCode = postalCode,
                CityName = cityName
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateCustomer(customer));

            Assert.Equal(expectedErrorMsg, ex.Message);
        }

        [Theory]
        [InlineData("Carsten")]
        [InlineData("Carsten@")]
        [InlineData("Carsten@gmail")]
        [InlineData("Carsten@hotmail")]
        [InlineData("Carsten@@gmail.com")]
        [InlineData("@Carsten@gmail.com")]
        [InlineData("Carsten@gmail!.com")]
        [InlineData("Carsten.com")]
        public void AddCustomer_InvalidEmail_ExceptArgumentException(string email)
        {
            // arrange
            Customer customer = new Customer
            {
                ID = 1,
                FirstName = "Carsten",
                LastName = "Hansen",
                Email = email,
                PhoneNumber = "76 117 117",
                StreetName = "Rolf Krakesvej 10",
                PostalCode = 3600,
                CityName = "Frederikssund"
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateCustomer(customer));

            Assert.Equal("Email must be a valid email", ex.Message);
        }

        [Theory]
        [InlineData("Carsten@gmail.com")]
        [InlineData("Kristian@hotmail.se")]
        [InlineData("Emil.Larsen@gmail.dk")]
        public void AddCustomer_ValidEmail(string email)
        {
            // arrange
            Customer customer = new Customer
            {
                ID = 1,
                FirstName = "Carsten",
                LastName = "Hansen",
                Email = email,
                PhoneNumber = "76 117 117",
                StreetName = "Rolf Krakesvej 10",
                PostalCode = 3600,
                CityName = "Frederikssund"
            };

            IValidator validator = new BEValidator();

            // act + assert
            validator.ValidateCustomer(customer);
        }

        [Theory]
        [InlineData(1, "Karsten", "Clausen", "KC@gmail.com", "", "Skolegade 2", 6771, "Esbjerg")]
        [InlineData(2, "Kirsten", "Clausen", "Karsten.Hansen@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg")]
        public void AddCustomer_ValidCustomer(int id, string firstName, string lastName, string email, string phoneNumber, string streetName, int postalCode, string cityName)
        {
            // arrange
            Customer customer = new Customer
            {
                ID = id,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                StreetName = streetName,
                PostalCode = postalCode,
                CityName = cityName
            };

            IValidator validator = new BEValidator();

            // act + assert
            validator.ValidateCustomer(customer);
        }
        #endregion

        #region AddUser
        [Theory]
        [InlineData(null, "password", "user", "Username must be be between 8-24 characters")]                        // invalid username: null
        [InlineData("", "password", "user", "Username must be be between 8-24 characters")]                          // invalid username: ""
        [InlineData("Jespern", "password", "user", "Username must be be between 8-24 characters")]                   // invalid username: under 8 characters
        [InlineData("JespernJespernJespernJesp", "password", "user", "Username must be be between 8-24 characters")] // invalid username: over 24 characters
        [InlineData("Andreasen", null, "user", "Password must be minimum 8 characters")]                            // invalid password: null
        [InlineData("Andreasen", "", "user", "Password must be minimum 8 characters")]                              // invalid password: ""
        [InlineData("Andreasen", "kode", "user", "Password must be minimum 8 characters")]                          // invalid password: under 8 characters
        [InlineData("Andreasen", "password", null, "Userrole can't be null or empty")]                               // invalid userrole: null
        [InlineData("Andreasen", "password", "", "Userrole can't be null or empty")]                                 // invalid userrole: ""
        public void CreateUser_InvalidUser_ExceptArgumentException(string username, string password, string userrole, string expectedErrorMsg)
        {
            // arrange
            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateCreateUser(username, password, userrole));

            Assert.Equal(expectedErrorMsg, ex.Message);
        }

        [Theory]
        [InlineData("Andreasen", "lasagne28", "User")]
        [InlineData("HurtigeJan", "2sa55y4me@$FA", "Admin")]
        public void CreateUser_ValidUser(string username, string password, string userrole)
        {
            // arrange
            IValidator validator = new BEValidator();

            // act + assert
            validator.ValidateCreateUser(username, password, userrole);
        }


        [Theory]
        [InlineData(-1, "Andreasen", "user", "Invalid ID")]                                                 // invalid id: -1
        [InlineData(int.MinValue, "Andreasen", "user", "Invalid ID")]                                       // invalid id: -2147483648
        [InlineData(1, null, "user", "Username must be be between 8-24 characters")]                        // invalid username: null
        [InlineData(1, "", "user", "Username must be be between 8-24 characters")]                          // invalid username: ""
        [InlineData(1, "Jespern", "user", "Username must be be between 8-24 characters")]                   // invalid username: under 8 characters
        [InlineData(1, "JespernJespernJespernJesp", "user", "Username must be be between 8-24 characters")] // invalid username: over 24 characters
        [InlineData(1, "Andreasen", null, "Userrole can't be null or empty")]                               // invalid userrole: null
        [InlineData(1, "Andreasen", "", "Userrole can't be null or empty")]                                 // invalid userrole: ""
        public void AddUser_InvalidUser_ExceptArgumentException(int id, string username, string userrole, string expectedErrorMsg)
        {
            // arrange
            byte[] password = new byte[] { 1 };
            byte[] salt = new byte[] { 1 };

            User user = new User
            {
                ID = id,
                Username = username,
                UserRole = userrole,
                Password = password,
                Salt = salt
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateUser(user));

            Assert.Equal(expectedErrorMsg, ex.Message);
        }

        [Fact]
        public void AddUser_InvalidPasswordNull_ExceptArgumentException()
        {
            // arrange
            byte[] password = null;
            byte[] salt = new byte[] { 1 };

            User user = new User
            {
                ID = 1,
                Username = "Andreasen",
                UserRole = "User",
                Password = password,
                Salt = salt
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateUser(user));

            Assert.Equal("User password cannot be null or empty", ex.Message);
        }

        [Fact]
        public void AddUser_InvalidPasswordEmpty_ExceptArgumentException()
        {
            // arrange
            byte[] password = new byte[] { };
            byte[] salt = new byte[] { 1 };

            User user = new User
            {
                ID = 1,
                Username = "Andreasen",
                UserRole = "User",
                Password = password,
                Salt = salt
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateUser(user));

            Assert.Equal("User password cannot be null or empty", ex.Message);
        }

        [Fact]
        public void AddUser_InvalidSaltNull_ExceptArgumentException()
        {
            // arrange
            byte[] password = new byte[] { 1 };
            byte[] salt = null;

            User user = new User
            {
                ID = 1,
                Username = "Andreasen",
                UserRole = "User",
                Password = password,
                Salt = salt
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateUser(user));

            Assert.Equal("User salt cannot be null or empty", ex.Message);
        }

        [Fact]
        public void AddUser_InvalidSaltEmpty_ExceptArgumentException()
        {
            // arrange
            byte[] password = new byte[] { 1 };
            byte[] salt = new byte[] { };

            User user = new User
            {
                ID = 1,
                Username = "Andreasen",
                UserRole = "User",
                Password = password,
                Salt = salt
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateUser(user));

            Assert.Equal("User salt cannot be null or empty", ex.Message);
        }

        [Theory]
        [InlineData(1, "Andreasen", "User")]
        [InlineData(2, "HurtigeJan", "Admin")]
        public void AddUser_ValidUser(int id, string username, string userrole)
        {
            // arrange
            byte[] password = new byte[] { 1 };
            byte[] salt = new byte[] { 1 };

            User user = new User
            {
                ID = id,
                Username = username,
                UserRole = userrole,
                Password = password,
                Salt = salt
            };

            IValidator validator = new BEValidator();

            // act + assert
            validator.ValidateUser(user);
        }
        #endregion

        #region AddOrder
        [Theory]
        [InlineData(1, 100)]
        [InlineData(2, 35)]
        public void AddOrder_ValidOrder(int id, double price)
        {
            // arrange
            Customer customer = new Customer { ID = 1 };
            DateTime orderDate = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat);
            List<OrderBeer> orderBeers = new List<OrderBeer> { new OrderBeer { BeerID = 1 } };

            Order order = new Order
            {
                ID = id,
                AccumulatedPrice = price,
                Customer = customer,
                OrderCreated = orderDate,
                OrderBeers = orderBeers
            };

            IValidator validator = new BEValidator();

            // act + assert
            validator.ValidateOrder(order);
        }

        [Fact]
        public void AddOrder_InvalidOrderNull_ExceptArgumentException()
        {
            // arrange
            Order order = null;
            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateOrder(order));

            Assert.Equal("Order instance can't be null", ex.Message);
        }

        [Theory]
        [InlineData(-1, 100, "Invalid ID")]                                   // invalid id: -1
        [InlineData(int.MinValue, 35, "Invalid ID")]                          // invalid id: -2147483648
        [InlineData(1, -1, "Price must be higher than zero")]                 // invalid AccumulatedPrice: -1
        [InlineData(2, int.MinValue, "Price must be higher than zero")]       // invalid AccumulatedPrice: -2147483648
        public void AddOrder_InvalidOrder_ExceptArgumentException(int id, double price, string expectedErrorMsg)
        {
            // arrange
            Customer customer = new Customer { ID = 1 };
            DateTime orderDate = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat);
            List<OrderBeer> orderBeers = new List<OrderBeer> { new OrderBeer { BeerID = 1 } };

            Order order = new Order
            {
                ID = id,
                AccumulatedPrice = price,
                Customer = customer,
                OrderCreated = orderDate,
                OrderBeers = orderBeers
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateOrder(order));

            Assert.Equal(expectedErrorMsg, ex.Message);
        }

        [Fact]
        public void AddOrder_InvalidOrderCustomerNull_ExceptArgumentException()
        {
            // arrange
            Customer customer = null;
            DateTime orderDate = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat);
            List<OrderBeer> orderBeers = new List<OrderBeer> { new OrderBeer { BeerID = 1 } };

            Order order = new Order
            {
                ID = 1,
                AccumulatedPrice = 49.95,
                Customer = customer,
                OrderCreated = orderDate,
                OrderBeers = orderBeers
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateOrder(order));

            Assert.Equal("Invalid customer", ex.Message);
        }

        [Fact]
        public void AddOrder_InvalidOrderNewDate_ExceptArgumentException()
        {
            // arrange
            Customer customer = new Customer { ID = 1 };
            DateTime orderDate = new DateTime();
            List<OrderBeer> orderBeers = new List<OrderBeer> { new OrderBeer { BeerID = 1 } };

            Order order = new Order
            {
                ID = 1,
                AccumulatedPrice = 49.95,
                Customer = customer,
                OrderCreated = orderDate,
                OrderBeers = orderBeers
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateOrder(order));

            Assert.Equal("No order attached", ex.Message);
        }

        [Fact]
        public void AddOrder_InvalidOrderListEmplty_ExceptArgumentException()
        {
            // arrange
            Customer customer = new Customer { ID = 1 };
            DateTime orderDate = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat);
            List<OrderBeer> orderBeers = new List<OrderBeer> {};

            Order order = new Order
            {
                ID = 1,
                AccumulatedPrice = 49.95,
                Customer = customer,
                OrderCreated = orderDate,
                OrderBeers = orderBeers
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateOrder(order));

            Assert.Equal("Can not process order with no products", ex.Message);
        }

        [Fact]
        public void AddOrder_InvalidOrderListNull_ExceptArgumentException()
        {
            // arrange
            Customer customer = new Customer { ID = 1 };
            DateTime orderDate = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat);
            List<OrderBeer> orderBeers = null;

            Order order = new Order
            {
                ID = 1,
                AccumulatedPrice = 49.95,
                Customer = customer,
                OrderCreated = orderDate,
                OrderBeers = orderBeers
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateOrder(order));

            Assert.Equal("Can not process order with no products", ex.Message);
        }
        #endregion
    }
}
