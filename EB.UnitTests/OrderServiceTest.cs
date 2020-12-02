﻿using EB.Core.ApplicationServices;
using EB.Core.ApplicationServices.Impl;
using EB.Core.DomainServices;
using EB.Core.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace EB.UnitTests
{
    public class OrderServiceTest
    {
        private readonly SortedDictionary<int, Order> orderDatabase;
        private readonly SortedDictionary<int, Customer> customerDatabase;
        private readonly SortedDictionary<int, Beer> beerDatabase;
        private readonly Mock<IOrderRepository> repoMock;
        private readonly Mock<ICustomerRepository> customerMock;
        private readonly Mock<IBeerRepository> beerMock;
        private readonly Mock<IValidator> validatorMock;

        public OrderServiceTest()
        {
            orderDatabase = new SortedDictionary<int, Order>();
            customerDatabase = new SortedDictionary<int, Customer>();
            beerDatabase = new SortedDictionary<int, Beer>();

            repoMock = new Mock<IOrderRepository>();
            customerMock = new Mock<ICustomerRepository>();
            beerMock = new Mock<IBeerRepository>();
            validatorMock = new Mock<IValidator>();

            repoMock.Setup(repo => repo.AddOrder(It.IsAny<Order>())).Callback<Order>(order => orderDatabase.Add(order.ID, order));
            repoMock.Setup(repo => repo.DeleteOrder(It.IsAny<int>())).Callback<int>(id => orderDatabase.Remove(id));
            repoMock.Setup(repo => repo.ReadAllOrders()).Returns(() => orderDatabase.Values);
            repoMock.Setup(repo => repo.ReadAllOrdersByCustomer(It.IsAny<int>())).Returns<int>((id) => orderDatabase.Values.Where(o => o.Customer.ID == id));
            repoMock.Setup(repo => repo.ReadOrderByID(It.IsAny<int>())).Returns<int>((id) => orderDatabase.ContainsKey(id) ? orderDatabase[id] : null);

            customerMock.Setup(repo => repo.ReadCustomerById(It.IsAny<int>())).Returns<int>((id) => customerDatabase.ContainsKey(id) ? customerDatabase[id] : null);
            beerMock.Setup(repo => repo.ReadSimpleBeerByID(It.IsAny<int>())).Returns<int>((id) => beerDatabase.ContainsKey(id) ? beerDatabase[id] : null);
        }

        [Fact]
        public void CreateOrderService_RepositoriesAndValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            OrderService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new OrderService(null as IOrderRepository, null as IBeerRepository, null as ICustomerRepository, null as IValidator));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateOrderService_RepositoriesIsNull_ExpectNullReferenceException()
        {
            // arrange
            OrderService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new OrderService(null as IOrderRepository, null as IBeerRepository, null as ICustomerRepository, validatorMock.Object));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateOrderService_OrderAndBeerRepositoryIsNull_ExpectNullReferenceException()
        {
            // arrange
            OrderService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new OrderService(null as IOrderRepository, null as IBeerRepository, customerMock.Object, validatorMock.Object));

            Assert.Equal("Repository can't be null", ex.Message);
        }


        [Fact]
        public void CreateOrderService_ValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            OrderService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, null as IValidator));

            Assert.Equal("Validator can't be null", ex.Message);
        }

        [Fact]
        public void CreateOrderService_ValidRepositoriesAndValidator()
        {
            // arrange
            OrderService service = null;

            // act + assert
            service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            Assert.NotNull(service);
        }

        [Fact]
        public void OrderService_ShouldBeOfTypeIOrderService()
        {
            // arrange

            // act + assert
            new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object).Should().BeAssignableTo<IOrderService>();
        }


        [Fact]
        public void AddOrder()
        {
            // arrange
            Customer customer = new Customer {ID = 1 };
            customerDatabase.Add(customer.ID, customer);

            Beer beer = new Beer { ID = 1, Price = 65, Stock = 3};
            beerDatabase.Add(beer.ID, beer);

            List<OrderBeer> orderedBeers = new List<OrderBeer> { new OrderBeer { BeerID = beer.ID, Amount = 2 } };

            Order order = new Order()
            {
                ID = 1,
                OrderCreated = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat),
                AccumulatedPrice = 130,
                Customer = customer,
                OrderBeers = orderedBeers
            };

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act
            service.AddOrder(order);

            // assert
            Assert.Contains(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Once);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.Is<int>(id => id == beer.ID)), Times.Once);
            customerMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(id => id == customer.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateOrder(It.Is<Order>(o => o == order)), Times.Once);
        }

        [Fact]
        public void AddOrder_OrderAmountHigherThanStock_ExpectInvalidOperationException()
        {
            // arrange
            Customer customer = new Customer { ID = 1 };
            customerDatabase.Add(customer.ID, customer);

            Beer beer = new Beer { ID = 1, Price = 65, Stock = 5 };
            beerDatabase.Add(beer.ID, beer);

            List<OrderBeer> orderedBeers = new List<OrderBeer> { new OrderBeer { BeerID = beer.ID, Amount = 7 } };

            Order order = new Order()
            {
                ID = 1,
                OrderCreated = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat),
                AccumulatedPrice = 256.5,
                Customer = customer,
                OrderBeers = orderedBeers
            };

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<InvalidOperationException>(() => service.AddOrder(order));

            // assert
            Assert.Equal("Order amount higher than inventory stock", ex.Message);
            Assert.DoesNotContain(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Never);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.Is<int>(id => id == beer.ID)), Times.Once);
            customerMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(id => id == customer.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateOrder(It.Is<Order>(o => o == order)), Times.Once);
        }

        [Fact]
        public void AddOrder_OrderAmountHigherThanStockMoreWares_ExpectInvalidOperationException()
        {
            // arrange
            Customer customer = new Customer { ID = 1 };
            customerDatabase.Add(customer.ID, customer);

            Beer beer1 = new Beer { ID = 1, Price = 65, Stock = 5};
            Beer beer2 = new Beer { ID = 2, Price = 65, Stock = 2};

            beerDatabase.Add(beer1.ID, beer1);
            beerDatabase.Add(beer2.ID, beer2);

            List<OrderBeer> orderedBeers = new List<OrderBeer> { new OrderBeer { BeerID = beer1.ID, Amount = 5 }, new OrderBeer { BeerID = beer2.ID, Amount = 3 } };

            Order order = new Order()
            {
                ID = 1,
                OrderCreated = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat),
                AccumulatedPrice = 256.5,
                Customer = customer,
                OrderBeers = orderedBeers
            };

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<InvalidOperationException>(() => service.AddOrder(order));

            // assert
            Assert.Equal("Order amount higher than inventory stock", ex.Message);
            Assert.DoesNotContain(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Never);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.Is<int>(id => id == beer1.ID)), Times.Once);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.Is<int>(id => id == beer2.ID)), Times.Once);
            customerMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(id => id == customer.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateOrder(It.Is<Order>(o => o == order)), Times.Once);
        }

        [Fact]
        public void AddOrder_CustomerIsNull_ExpectArgumentException()
        {
            // arrange
            Customer customer = null;

            Order order = new Order()
            {
                ID = 1,
                OrderCreated = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat),
                AccumulatedPrice = 256.5,
                Customer = customer
            };

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.AddOrder(order));

            // assert
            Assert.Equal("Customer cannot be null", ex.Message);
            Assert.DoesNotContain(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Never);
            customerMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(id => id == customer.ID)), Times.Never);
            validatorMock.Verify(validator => validator.ValidateOrder(It.Is<Order>(o => o == order)), Times.Never);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void AddOrder_CustomerIsNotInDB_ExpectArgumentException()
        {
            // arrange
            Customer customer = new Customer { ID = 1 };

            Order order = new Order()
            {
                ID = 1,
                OrderCreated = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat),
                AccumulatedPrice = 256.5,
                Customer = customer
            };

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.AddOrder(order));

            // assert
            Assert.Equal("Customer cannot be null", ex.Message);
            Assert.DoesNotContain(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Never);
            customerMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(id => id == customer.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateOrder(It.Is<Order>(o => o == order)), Times.Never);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [InlineData(-1, 249.95, "ID is invalid")]
        [InlineData(2, -22, "Price cannot be negative")]
        public void AddOrder_InvalidOrder_ExceptArgumentException(int id, double price, string errorExpected)
        {
            // arrange
            Customer customer = new Customer { ID = 1 };
            customerDatabase.Add(customer.ID, customer);

            Order order = new Order()
            {
                ID = id,
                OrderCreated = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat),
                AccumulatedPrice = price,
                Customer = customer
            };

            validatorMock.Setup(mock => mock.ValidateOrder(It.IsAny<Order>())).Callback<Order>(order => throw new ArgumentException(errorExpected));

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.AddOrder(order));

            // assert
            Assert.Equal(errorExpected, ex.Message);
            Assert.DoesNotContain(order, orderDatabase.Values);
            validatorMock.Verify(validator => validator.ValidateOrder(It.Is<Order>(o => o == order)), Times.Once);
            customerMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(ID => ID == customer.ID)), Times.Once);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Never);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [InlineData(1, 249.95)]
        [InlineData(2, 22)]
        public void AddOrder_ValidOrder(int id, double price)
        {
            // arrange
            Customer customer = new Customer { ID = 1 };
            customerDatabase.Add(customer.ID, customer);

            Beer beer1 = new Beer { ID = 1, Price = 65, Stock = 5 };
            Beer beer2 = new Beer { ID = 2, Price = 65, Stock = 2 };

            beerDatabase.Add(beer1.ID, beer1);
            beerDatabase.Add(beer2.ID, beer2);

            List<OrderBeer> orderedBeers = new List<OrderBeer> { new OrderBeer { BeerID = beer1.ID, Amount = 5 }, new OrderBeer { BeerID = beer2.ID, Amount = 1 } };

            Order order = new Order()
            {
                ID = id,
                OrderCreated = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat),
                AccumulatedPrice = price,
                Customer = customer,
                OrderBeers = orderedBeers
            };

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act
            service.AddOrder(order);

            // assert
            Assert.Contains(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Once);
            customerMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(ID => ID == customer.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateOrder(It.Is<Order>(o => o == order)), Times.Once);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.Is<int>(id => id == beer1.ID)), Times.Once);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.Is<int>(id => id == beer2.ID)), Times.Once);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.IsAny<int>()), Times.Exactly(2));
        }

        [Theory]
        [InlineData(22, 2, 3)]
        [InlineData(249.95, 3, 10)]
        public void AddOrder_ValidOrderCorrectAmount(double price, int amount, int stock)
        {
            // arrange
            Customer customer = new Customer { ID = 1 };
            customerDatabase.Add(customer.ID, customer);

            Beer beer = new Beer { ID = 1, Price = price, Stock = stock };
            beerDatabase.Add(beer.ID, beer);

            List<OrderBeer> orderedBeers = new List<OrderBeer> { new OrderBeer { BeerID = beer.ID, Amount = amount } };

            Order order = new Order()
            {
                ID = 1,
                OrderCreated = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat),
                AccumulatedPrice = 130,
                Customer = customer,
                OrderBeers = orderedBeers
            };

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act
            service.AddOrder(order);

            // assert
            Assert.Contains(order, orderDatabase.Values);
            Assert.Equal(Math.Round(price*amount,2), orderDatabase[1].AccumulatedPrice);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Once);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.Is<int>(id => id == beer.ID)), Times.Once);
            customerMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(id => id == customer.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateOrder(It.Is<Order>(o => o == order)), Times.Once);
        }

        [Fact]
        public void GetOrderById_OrderExists()
        {
            // arrange
            Order order = new Order { ID = 1 };
            orderDatabase.Add(order.ID, order);

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act
            var result = service.ReadOrderByID(order.ID);

            // assert
            Assert.Equal(order, result);
            repoMock.Verify(repo => repo.ReadOrderByID(It.Is<int>(id => id == order.ID)), Times.Once);
        }

        [Fact]
        public void GetOrderById_OrderDoesNotExist_ExpectNull()
        {
            // arrange
            var order1 = new Order { ID = 1 };
            var order2 = new Order { ID = 2 };

            orderDatabase.Add(order2.ID, order2);

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object); ;

            // act
            var result = service.ReadOrderByID(order1.ID);

            // assert
            Assert.Null(result);
            repoMock.Verify(repo => repo.ReadOrderByID(It.Is<int>(ID => ID == order1.ID)), Times.Once);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void GetAllOrder(int orderCount)
        {
            // arrange
            Order order1 = new Order { ID = 1 };
            Order order2 = new Order { ID = 2 };
            List<Order> orders = new List<Order>() { order1, order2 };

            var expected = orders.GetRange(0, orderCount);
            foreach (var order in expected)
            {
                orderDatabase.Add(order.ID, order);
            }

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act
            var result = service.ReadAllOrders();

            // assert
            Assert.Equal(expected, result);
            repoMock.Verify(repo => repo.ReadAllOrders(), Times.Once);
        }

        [Fact]
        public void GetAllOrders_ByCustomerID()
        {
            // arrange
            Customer customer1 = new Customer { ID = 1 };
            Customer customer2 = new Customer { ID = 2 };
            customerDatabase.Add(customer1.ID, customer1);
            customerDatabase.Add(customer2.ID, customer2);

            Order order1 = new Order { ID = 1, Customer = customer1};
            Order order2 = new Order { ID = 2, Customer = customer1};
            Order order3 = new Order { ID = 3, Customer = customer2};

            orderDatabase.Add(order1.ID, order1);
            orderDatabase.Add(order2.ID, order2);
            orderDatabase.Add(order3.ID, order3);

            var expected = new List<Order> {order3};
            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act
            var result = service.ReadAllOrdersByCustomer(customer2.ID);

            // assert
            Assert.Equal(expected, result);
            repoMock.Verify(repo => repo.ReadAllOrdersByCustomer(It.Is<int>(ID => ID == customer2.ID)), Times.Once);
        }

        [Fact]
        public void RemoveOrder_ValidExistingOrder()
        {
            // arrange
            Order order = new Order()
            {
                ID = 1
            };

            orderDatabase.Add(order.ID, order);

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act
            service.DeleteOrder(order.ID);

            // assert
            repoMock.Verify(repo => repo.ReadOrderByID(It.Is<int>(ID => ID == order.ID)), Times.Once);
            repoMock.Verify(repo => repo.DeleteOrder(It.Is<int>(ID => ID == order.ID)), Times.Once);
            Assert.Null(repoMock.Object.ReadOrderByID(order.ID));
        }

        [Fact]
        public void RemoveOrder_OrderDoesNotExist_ExpectArgumentException()
        {
            // arrange

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            Order order = new Order()
            {
                ID = 1
            };

            // act + assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.DeleteOrder(order.ID));

            // assert
            Assert.Equal("No order with such ID found", ex.Message);
            repoMock.Verify(repo => repo.ReadOrderByID(It.Is<int>(ID => ID == order.ID)), Times.Once);
            repoMock.Verify(repo => repo.DeleteOrder(It.Is<int>(ID => ID == order.ID)), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void RemoveOrder_IncorrectID_ExpectArgumentException(int ID)
        {
            // arrange
            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.DeleteOrder(ID));

            // assert
            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repo => repo.ReadOrderByID(It.Is<int>(id => id == ID)), Times.Never);
            repoMock.Verify(repo => repo.DeleteOrder(It.Is<int>(id => id == ID)), Times.Never);
        }
    }
}
