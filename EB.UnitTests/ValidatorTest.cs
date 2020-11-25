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


        //[Theory]
        //[InlineData(0, "Company", "Address", 1234, "District", "uri")]      // invalid id: 0
        //[InlineData(-1, "Company", "Address", 1234, "District", "uri")]     // invalid id: -1
        //[InlineData(1, null, "Address", 1234, "District", "uri")]           // invalid name: null
        //[InlineData(1, "", "Address", 1234, "District", "uri")]             // invalid name: ""
        //[InlineData(1, "Company", null, 1234, "District", "uri")]           // invalid address: null
        //[InlineData(1, "Company", "", 1234, "District", "uri")]             // invalid address: ""
        //[InlineData(1, "Company", "Address", 0, "District", "uri")]         // invalid zipcode: 0
        //[InlineData(1, "Company", "Address", -1, "District", "uri")]        // invalid zipcode: -1
        //[InlineData(1, "Company", "Address", 1234, null, "uri")]            // invalid PostalDistrict: null
        //[InlineData(1, "Company", "Address", 1234, "", "uri")]              // invalid PostalDistrict: ""
        //[InlineData(1, "Company", "Address", 1234, "District", "")]         // invaild CompanyUri: ""
        public void AddBeer_InvalidBeer_ExceptArgumentException(int id, string name, string description, double price, double EBC, double IBU, double percentage, Brand brand, BeerType type, string expectedErrorMsg)
        {
            // arrange
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

            Assert.Equal("Invalid Company property", ex.Message);
        }
    }
}
