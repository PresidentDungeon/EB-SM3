using EB.Core.ApplicationServices;
using EB.Core.ApplicationServices.Validators;
using EB.Core.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [InlineData(0, "Beer", "Fra 1884!", 65, 50, 100, 14, "Invalid ID")]                         // invalid id: 0
        [InlineData(-1, "Beer", "Fra 1884!", 65, 50, 100, 14, "Invalid ID")]                        // invalid id: -1
        [InlineData(1, null, "Fra 1884!", 65, 50, 100, 14, "Name can not be empty")]                // invalid name: null
        [InlineData(1, "", "Fra 1884!", 65, 50, 100, 14, "Name can not be empty")]                  // invalid name: ""
        [InlineData(1, "Beer", null, 65, 50, 100, 14, "Description can not be empty")]              // invalid description: null
        [InlineData(1, "Beer", "", 65, 50, 100, 14, "Description can not be empty")]                // invalid description: ""
        [InlineData(1, "Beer", "Fra 1884!", 0, 50, 100, 14, "Price must be higher than zero")]      // invalid price: 0
        [InlineData(1, "Beer", "Fra 1884!", -1, 50, 100, 14, "Price must be higher than zero")]     // invalid price: -1
        [InlineData(1, "Beer", "Fra 1884!", 65, -1, 100, 14, "EBC must be betweeen 0-80")]          // invalid EBC: -1
        [InlineData(1, "Beer", "Fra 1884!", 65, 81, 100, 14, "EBC must be betweeen 0-80")]          // invalid EBC: 81
        [InlineData(1, "Beer", "Fra 1884!", 65, 50, -1, 14, "IBU must be betweeen 0-120")]          // invalid IBU: -1
        [InlineData(1, "Beer", "Fra 1884!", 65, 50, 121, 14, "IBU must be betweeen 0-120")]         // invalid IBU: 121
        [InlineData(1, "Beer", "Fra 1884!", 65, 50, 100, -1, "Percentage must be between 0-100")]   // invalid percentage: -1
        [InlineData(1, "Beer", "Fra 1884!", 65, 50, 100, 101, "Percentage must be between 0-100")]  // invalid percentage: 101

        public void AddBeer_InvalidBeer_ExceptArgumentException(int id, string name, string description, double price, double EBC, double IBU, double percentage, string expectedErrorMsg)
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
                Brand = brand,
                Type = type
            };

            IValidator validator = new BEValidator();

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => validator.ValidateBeer(beer));

            Assert.Equal("A brand must be selected", ex.Message);
        }

        [Theory]
        [InlineData(1, "Beer", "Fra 1884!", 65, 50, 100, 14)]
        [InlineData(2, "Beer2", "Fra 1882!", 200, 20, 35, 4)]
        public void AddBeer_ValidBeer(int id, string name, string description, double price, double EBC, double IBU, double percentage)
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
                Brand = brand,
                Type = type
            };

            IValidator validator = new BEValidator();

            // act + assert
            validator.ValidateBeer(beer);
        }

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
        [InlineData(0, "IPA", "Invalid ID")]                // invalid id: 0
        [InlineData(-1, "IPA", "Invalid ID")]               // invalid id: 0              
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
        [InlineData(0, "Ølværket", "Invalid ID")]                // invalid id: 0
        [InlineData(-1, "Ølværket", "Invalid ID")]               // invalid id: 0              
        [InlineData(1, null, "Name can not be empty")]      // invalid name: null              
        [InlineData(1, "", "Name can not be empty")]        // invalid name: empty         

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
        [InlineData(0, "Karsten", "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Invalid ID")]                            // invalid id: 0
        [InlineData(-1, "Karsten", "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Invalid ID")]                           // invalid id: -1
        [InlineData(1, null, "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Firstname can not be empty")]                 // invalid firstname: null
        [InlineData(1, "", "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Firstname can not be empty")]                   // invalid firstname: ""
        [InlineData(1, "Karsten", null, "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Lastname can not be empty")]                  // invalid lastname: null
        [InlineData(1, "Karsten", "", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg", "Lastname can not be empty")]                    // invalid lastname: ""
        [InlineData(1, "Karsten", "Clausen", "KC@gmail.com", null, "Skolegade 2", 6771, "Esbjerg", "Phonenumber can not be empty")]                       // invalid phonenumber: null
        [InlineData(1, "Karsten", "Clausen", "KC@gmail.com", "", "Skolegade 2", 6771, "Esbjerg", "Phonenumber can not be empty")]                         // invalid phonenumber: ""
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
        [InlineData(1, "Karsten", "Clausen", "KC@gmail.com", "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg")]
        [InlineData(2, "Kirsten", "Clausen", null, "+45 20 20 20 90", "Skolegade 2", 6771, "Esbjerg")]
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

    }
}
