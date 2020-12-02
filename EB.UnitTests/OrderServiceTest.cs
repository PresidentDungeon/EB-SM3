using EB.Core.ApplicationServices;
using EB.Core.ApplicationServices.Impl;
using EB.Core.DomainServices;
using EB.Core.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xunit;

namespace EB.UnitTests
{
    public class OrderServiceTest
    {
        private readonly SortedDictionary<int, Order> orderDatabase;
        private readonly Mock<IOrderRepository> repoMock;
        private readonly Mock<IValidator> validatorMock;

        public OrderServiceTest()
        {
            orderDatabase = new SortedDictionary<int, Order>();
            repoMock = new Mock<IOrderRepository>();
            validatorMock = new Mock<IValidator>();
            repoMock.Setup(repo => repo.CreateOrder(It.IsAny<Order>())).Callback<Order>(order => orderDatabase.Add(order.ID, order));
            repoMock.Setup(repo => repo.DeleteOrderInRepo(It.IsAny<int>())).Callback<int>(id => orderDatabase.Remove(id));
            repoMock.Setup(repo => repo.ReadAllOrders()).Returns(() => orderDatabase.Values);
            repoMock.Setup(repo => repo.ReadOrderById(It.IsAny<int>())).Returns<int>((id) => orderDatabase.ContainsKey(id) ? orderDatabase[id] : null);
        }

        [Fact]
        public void CreateOrderService_OrderRepositoryAndValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            OrderService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new OrderService(null as IOrderRepository, null as IValidator));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateOrderService_OrderRepositoryIsNull_ExpectNullReferenceException()
        {
            // arrange
            OrderService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new OrderService(null as IOrderRepository, validatorMock.Object));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateOrderService_ValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            OrderService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new OrderService(repoMock.Object, null as IValidator));

            Assert.Equal("Validator can't be null", ex.Message);
        }

        [Fact]
        public void CreateOrderService_ValidOrderRepositoryAndValidator()
        {
            // arrange
            OrderService service = null;

            // act + assert
            service = new OrderService(repoMock.Object, validatorMock.Object);

            Assert.NotNull(service);
        }

        [Fact]
        public void OrderService_ShouldBeOfTypeIOrderService()
        {
            // arrange

            // act + assert
            new OrderService(repoMock.Object, validatorMock.Object).Should().BeAssignableTo<IOrderService>();
        }

        

        [Theory]
        [InlineData(1)]
        public void AddOrder(int id/*, DateTime orderCreated, DateTime orderSent, double accumulatedPrice*/)
        {
            // arrange
            Order order = new Order()
            {
                ID = id,
                OrderCreated = DateTime.Parse("30-03-2012", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat),
                OrderSent = DateTime.Parse("30-03-2020", CultureInfo.GetCultureInfo("da-DK").DateTimeFormat),
                AccumulatedPrice = 256.5
            };

            OrderService service = new OrderService(repoMock.Object, validatorMock.Object);

            // act
            service.CreateOrder(order);

            // assert
            Assert.Contains(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.CreateOrder(It.Is<Order>(o => o == order)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateOrder(It.Is<Order>(o => o == order)), Times.Once);
        }

        [Fact]
        public void GetOrderById_OrderExists()
        {
            // arrange
            Order order = new Order { ID = 1 };
            orderDatabase.Add(order.ID, order);

            OrderService service = new OrderService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetOrderById(order.ID);

            // assert
            Assert.Equal(order, result);
            repoMock.Verify(repo => repo.ReadOrderById(It.Is<int>(id => id == order.ID)), Times.Once);
        }

        [Fact]
        public void GetOrderById_OrderDoesNotExist_ExpectNull()
        {
            // arrange
            var order1 = new Order { ID = 1 };
            var order2 = new Order { ID = 2 };

            orderDatabase.Add(order2.ID, order2);

            OrderService service = new OrderService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetOrderById(order1.ID);

            // assert
            Assert.Null(result);
            repoMock.Verify(repo => repo.ReadOrderById(It.Is<int>(ID => ID == order1.ID)), Times.Once);
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

            OrderService service = new OrderService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetAllOrders();

            // assert
            Assert.Equal(expected, result);
            repoMock.Verify(repo => repo.ReadAllOrders(), Times.Once);
        }

       

       
       
    }
}
