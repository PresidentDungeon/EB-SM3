using EB.Core.ApplicationServices;
using EB.Core.ApplicationServices.Impl;
using EB.Core.DomainServices;
using EB.Core.Entities;
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
    public class CustomerServiceTest
    {
        private SortedDictionary<int, Customer> customerDatabase;
        private Mock<ICustomerRepository> repoMock;
        private Mock<IValidator> validatorMock;

        public CustomerServiceTest()
        {
            customerDatabase = new SortedDictionary<int, Customer>();
            repoMock = new Mock<ICustomerRepository>();
            validatorMock = new Mock<IValidator>();
            repoMock.Setup(repo => repo.AddCustomer(It.IsAny<Customer>())).Callback<Customer>(customer => customerDatabase.Add(customer.ID, customer));
            repoMock.Setup(repo => repo.UpdateCustomerInRepo(It.IsAny<Customer>())).Callback<Customer>(customer => customerDatabase[customer.ID] = customer);
            repoMock.Setup(repo => repo.ReadCustomerById(It.IsAny<int>())).Returns<int>((id) => customerDatabase.ContainsKey(id) ? customerDatabase[id] : null);
        }

        [Fact]
        public void CreateCustomerService_CustomerRepositoryAndValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            CustomerService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new CustomerService(null as ICustomerRepository, null as IValidator));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateCustomerService_CustomerRepositoryIsNull_ExpectNullReferenceException()
        {
            // arrange
            CustomerService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new CustomerService(null as ICustomerRepository, validatorMock.Object));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateCustomerService_ValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            CustomerService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new CustomerService(repoMock.Object, null as IValidator));

            Assert.Equal("Validator can't be null", ex.Message);
        }

        [Fact]
        public void CreateCustomerService_ValidCustomerRepositoryAndValidator()
        {
            // arrange
            CustomerService service = null;

            // act + assert
            service = new CustomerService(repoMock.Object, validatorMock.Object);

            Assert.NotNull(service);
        }

        [Fact]
        public void CustomerService_ShouldBeOfTypeICustomerService()
        {
            // arrange

            // act + assert
            new CustomerService(repoMock.Object, validatorMock.Object).Should().BeAssignableTo<ICustomerService>();
        }

        [Fact]
        public void CustomerServiceValidate_ShouldValidateCustomerWithCustomerParameter_Once()
        {
            // arrange
            CustomerService service = null;
            Customer customer = new Customer { ID = 1 };

            // act + assert
            service = new CustomerService(repoMock.Object, validatorMock.Object);
            service.ValidateCustomer(customer);
            validatorMock.Verify(validator => validator.ValidateCustomer(It.Is<Customer>(c => c == customer)), Times.Once);
        }

        [Theory]
        [InlineData(1, "Århus")]
        [InlineData(2, "Randers")]
        public void AddCustomer_ValidCustomer(int id, string cityName)
        {
            // arrange
            Customer customer = new Customer()
            {
                ID = id,
                CityName = cityName
            };

            CustomerService service = new CustomerService(repoMock.Object, validatorMock.Object);

            // act
            service.CreateCustomer(customer);

            // assert
            Assert.Contains(customer, customerDatabase.Values);
            repoMock.Verify(repo => repo.AddCustomer(It.Is<Customer>(c => c == customer)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateCustomer(It.Is<Customer>(c => c == customer)), Times.Once);
        }

        [Theory]
        [InlineData(1, null, "City name is null exception")]
        [InlineData(2, "", "City name is null exception")]
        public void AddCustomer_InvalidCustomer(int id, string cityName, string errorExpected)
        {
            // arrange
            Customer customer = new Customer()
            {
                ID = id,
                CityName = cityName
            };

            validatorMock.Setup(mock => mock.ValidateCustomer(It.IsAny<Customer>())).Callback<Customer>(customer => throw new ArgumentException(errorExpected));
            CustomerService service = new CustomerService(repoMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.CreateCustomer(customer));

            // assert
            Assert.Equal(errorExpected, ex.Message);
            Assert.DoesNotContain(customer, customerDatabase.Values);
            repoMock.Verify(repo => repo.AddCustomer(It.Is<Customer>(c => c == customer)), Times.Never);
            validatorMock.Verify(validator => validator.ValidateCustomer(It.Is<Customer>(c => c == customer)), Times.Once);
        }

        [Fact]
        public void GetCustomerById_CustomerExists()
        {
            // arrange

            Customer customer = new Customer { ID = 1 };
            customerDatabase.Add(customer.ID, customer);

            CustomerService service = new CustomerService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetCustomerById(customer.ID);

            // assert
            Assert.Equal(customer, result);
            repoMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(id => id == customer.ID)), Times.Once);
        }

        [Fact]
        public void GetCustomerById_CustomerDoesNotExist_ExpectNull()
        {
            // arrange
            var customer1 = new Customer { ID = 1 };
            var customer2 = new Customer { ID = 2 };

            customerDatabase.Add(customer2.ID, customer2);

            CustomerService service = new CustomerService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetCustomerById(customer1.ID);

            // assert
            Assert.Null(result);
            repoMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(ID => ID == customer1.ID)), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetCustomerById_InvalidId_ExpectArgumentException(int ID)
        {
            // arrange
            CustomerService service = new CustomerService(repoMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.GetCustomerById(ID));

            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(id => id == ID)), Times.Never);
        }

        [Theory]
        [InlineData(1, "København")]
        [InlineData(2, "Odense")]
        public void UpdateCustomer_ValidExistingCustomer(int id, string cityName)
        {
            // arrange
            Customer customer = new Customer()
            {
                ID = id,
                CityName = cityName
            };

            customerDatabase.Add(customer.ID, customer);

            CustomerService service = new CustomerService(repoMock.Object, validatorMock.Object);

            // act
            service.UpdateCustomer(customer);

            // assert
            repoMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(ID => ID == customer.ID)), Times.Once);
            repoMock.Verify(repo => repo.UpdateCustomerInRepo(It.Is<Customer>(c => c == customer)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateCustomer(It.Is<Customer>(c => c == customer)), Times.Once);
            Assert.Equal(repoMock.Object.ReadCustomerById(customer.ID), customer);
        }

        [Theory]
        [InlineData(1, null, "city name is null exception")]
        [InlineData(2, "", "city name is null exception")]
        public void UpdateCustomer_InvalidCustomer_ExpectArgumentException(int id, string cityName, string errorExpected)
        {
            // arrange
            Customer customer = new Customer()
            {
                ID = id,
                CityName = cityName
            };

            validatorMock.Setup(mock => mock.ValidateCustomer(It.IsAny<Customer>())).Callback<Customer>(customer => throw new ArgumentException(errorExpected));
            CustomerService service = new CustomerService(repoMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateCustomer(customer));

            Assert.Equal(errorExpected, ex.Message);
            Assert.Equal(repoMock.Object.ReadCustomerById(customer.ID), null);

            repoMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(ID => ID == customer.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateCustomer(It.Is<Customer>(c => c == customer)), Times.Once);
            repoMock.Verify(repo => repo.UpdateCustomerInRepo(It.Is<Customer>(c => c == customer)), Times.Never);
        }

        [Fact]
        public void UpdateCustomer_CustomerIsNUll_ExpectArgumentException()
        {
            // arrange
            CustomerService service = new CustomerService(repoMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateCustomer(null));

            Assert.Equal("Updating customer does not exist", ex.Message);
            validatorMock.Verify(validator => validator.ValidateCustomer(It.Is<Customer>(c => c == null)), Times.Never);
            repoMock.Verify(repo => repo.UpdateCustomerInRepo(It.Is<Customer>(c => c == null)), Times.Never);
        }

        [Fact]
        public void UpdateCustomer_CustomerDoesNotExist_InvalidOperationException()
        {
            // arrange
            CustomerService service = new CustomerService(repoMock.Object, validatorMock.Object);

            Customer customer = new Customer()
            {
                ID = 1,
                CityName = "Aalborg"
            };

            // act + assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.UpdateCustomer(customer));

            Assert.Equal("No customer with such ID found", ex.Message);
            repoMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(ID => ID == customer.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateCustomer(It.Is<Customer>(c => c == customer)), Times.Once);
            repoMock.Verify(repo => repo.UpdateCustomerInRepo(It.Is<Customer>(c => c == customer)), Times.Never);
        }
    }
}