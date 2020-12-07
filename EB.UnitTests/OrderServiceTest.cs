using EB.Core.ApplicationServices;
using EB.Core.ApplicationServices.Impl;
using EB.Core.DomainServices;
using EB.Core.Entities;
using FluentAssertions;
using Moq;
using ProductShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace EB.UnitTests
{
    public class OrderServiceTest
    {
        #region Mock Setup
        private readonly SortedDictionary<int, Order> orderDatabase;
        private readonly SortedDictionary<int, Customer> customerDatabase;
        private readonly SortedDictionary<int, Beer> beerDatabase;
        private readonly Mock<IOrderRepository> repoMock;
        private readonly Mock<ICustomerRepository> customerMock;
        private readonly Mock<IBeerRepository> beerMock;
        private readonly Mock<IValidator> validatorMock;
        private readonly Mock<IEmailHelper> emailMock;

        public OrderServiceTest()
        {
            orderDatabase = new SortedDictionary<int, Order>();
            customerDatabase = new SortedDictionary<int, Customer>();
            beerDatabase = new SortedDictionary<int, Beer>();

            repoMock = new Mock<IOrderRepository>();
            customerMock = new Mock<ICustomerRepository>();
            beerMock = new Mock<IBeerRepository>();
            validatorMock = new Mock<IValidator>();
            emailMock = new Mock<IEmailHelper>();

            repoMock.Setup(repo => repo.AddOrder(It.IsAny<Order>())).Callback<Order>(order => orderDatabase.Add(order.ID, order));
            repoMock.Setup(repo => repo.DeleteOrder(It.IsAny<int>())).Callback<int>(id => orderDatabase.Remove(id));
            repoMock.Setup(repo => repo.ReadAllOrders(It.IsAny<Filter>())).Returns((Filter filter) => { List<Order> orders = orderDatabase.Values.ToList(); return new FilterList<Order> { totalItems = orders.Count, List = orders }; });
            repoMock.Setup(repo => repo.ReadAllOrdersByCustomer(It.IsAny<int>(), It.IsAny<Filter>())).Returns((int id, Filter filter) => { List<Order> orders = orderDatabase.Values.Where(o => o.Customer.ID == id).ToList(); return new FilterList<Order> { totalItems = orders.Count, List = orders }; });
            repoMock.Setup(repo => repo.ReadOrderByID(It.IsAny<int>())).Returns<int>((id) => orderDatabase.ContainsKey(id) ? orderDatabase[id] : null);
            repoMock.Setup(repo => repo.ReadOrderByIDUser(It.IsAny<int>(), It.IsAny<int>())).Returns((int orderID, int userID) => { return orderDatabase.Values.Where(order => order.Customer.ID == userID).FirstOrDefault(order => order.ID == orderID); });

            customerMock.Setup(repo => repo.ReadCustomerById(It.IsAny<int>())).Returns<int>((id) => customerDatabase.ContainsKey(id) ? customerDatabase[id] : null);
            beerMock.Setup(repo => repo.ReadSimpleBeerByID(It.IsAny<int>())).Returns<int>((id) => beerDatabase.ContainsKey(id) ? beerDatabase[id] : null);
        }
        #endregion

        #region OrderService Tests
        [Fact]
        public void CreateOrderService_RepositoriesAndValidatorsIsNull_ExpectNullReferenceException()
        {
            // arrange
            OrderService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new OrderService(null as IOrderRepository, null as IBeerRepository, null as ICustomerRepository, null as IValidator, null as IEmailHelper));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateOrderService_RepositoriesAndEmailHelperIsNull_ExpectNullReferenceException()
        {
            // arrange
            OrderService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new OrderService(null as IOrderRepository, null as IBeerRepository, null as ICustomerRepository, validatorMock.Object, null as IEmailHelper));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateOrderService_OrderAndBeerRepositoryIsNull_ExpectNullReferenceException()
        {
            // arrange
            OrderService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new OrderService(null as IOrderRepository, null as IBeerRepository, customerMock.Object, validatorMock.Object, emailMock.Object));

            Assert.Equal("Repository can't be null", ex.Message);
        }


        [Fact]
        public void CreateOrderService_ValidatorsIsNull_ExpectNullReferenceException()
        {
            // arrange
            OrderService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, null as IValidator, emailMock.Object));

            Assert.Equal("Validator can't be null", ex.Message);
        }

        [Fact]
        public void CreateOrderService_RepositoriesAndValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            OrderService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, null as IEmailHelper));

            Assert.Equal("Email helper can't be null", ex.Message);
        }

        [Fact]
        public void CreateOrderService_ValidRepositoriesAndValidator()
        {
            // arrange
            OrderService service = null;

            // act + assert
            service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            Assert.NotNull(service);
        }

        [Fact]
        public void OrderService_ShouldBeOfTypeIOrderService()
        {
            // arrange

            // act + assert
            new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object).Should().BeAssignableTo<IOrderService>();
        }
        #endregion

        #region Create Order Tests
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

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            service.AddOrder(order);

            // assert
            Assert.Contains(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Once);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.Is<int>(id => id == beer.ID)), Times.Once);
            customerMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(id => id == customer.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateOrder(It.Is<Order>(o => o == order)), Times.Once);
            emailMock.Verify(emailMock => emailMock.SendVerificationEmail(It.IsAny<Order>()), Times.Once);
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

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var ex = Assert.Throws<InvalidOperationException>(() => service.AddOrder(order));

            // assert
            Assert.Equal("Order amount higher than inventory stock", ex.Message);
            Assert.DoesNotContain(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Never);
            emailMock.Verify(emailMock => emailMock.SendVerificationEmail(It.Is<Order>(o => o == order)), Times.Never);
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

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var ex = Assert.Throws<InvalidOperationException>(() => service.AddOrder(order));

            // assert
            Assert.Equal("Order amount higher than inventory stock", ex.Message);
            Assert.DoesNotContain(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Never);
            emailMock.Verify(emailMock => emailMock.SendVerificationEmail(It.Is<Order>(o => o == order)), Times.Never);
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

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.AddOrder(order));

            // assert
            Assert.Equal("Customer cannot be null", ex.Message);
            Assert.DoesNotContain(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Never);
            emailMock.Verify(emailMock => emailMock.SendVerificationEmail(It.Is<Order>(o => o == order)), Times.Never);
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

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.AddOrder(order));

            // assert
            Assert.Equal("Customer cannot be null", ex.Message);
            Assert.DoesNotContain(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Never);
            emailMock.Verify(emailMock => emailMock.SendVerificationEmail(It.Is<Order>(o => o == order)), Times.Never);
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

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.AddOrder(order));

            // assert
            Assert.Equal(errorExpected, ex.Message);
            Assert.DoesNotContain(order, orderDatabase.Values);
            validatorMock.Verify(validator => validator.ValidateOrder(It.Is<Order>(o => o == order)), Times.Once);
            customerMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(ID => ID == customer.ID)), Times.Once);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Never);
            emailMock.Verify(emailMock => emailMock.SendVerificationEmail(It.Is<Order>(o => o == order)), Times.Never);
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

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            service.AddOrder(order);

            // assert
            Assert.Contains(order, orderDatabase.Values);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Once);
            emailMock.Verify(emailMock => emailMock.SendVerificationEmail(It.IsAny<Order>()), Times.Once);
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

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            service.AddOrder(order);

            // assert
            Assert.Contains(order, orderDatabase.Values);
            Assert.Equal(Math.Round(price*amount,2), orderDatabase[1].AccumulatedPrice);
            repoMock.Verify(repo => repo.AddOrder(It.Is<Order>(o => o == order)), Times.Once);
            emailMock.Verify(emailMock => emailMock.SendVerificationEmail(It.IsAny<Order>()), Times.Once);
            beerMock.Verify(repo => repo.ReadSimpleBeerByID(It.Is<int>(id => id == beer.ID)), Times.Once);
            customerMock.Verify(repo => repo.ReadCustomerById(It.Is<int>(id => id == customer.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateOrder(It.Is<Order>(o => o == order)), Times.Once);
        }
        #endregion

        #region Read Order Tests
        [Fact]
        public void GetOrderById_OrderExists()
        {
            // arrange
            Order order = new Order { ID = 1 };
            orderDatabase.Add(order.ID, order);

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

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

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var result = service.ReadOrderByID(order1.ID);

            // assert
            Assert.Null(result);
            repoMock.Verify(repo => repo.ReadOrderByID(It.Is<int>(ID => ID == order1.ID)), Times.Once);
        }

        [Fact]
        public void GetOrderByUserId_ValidateUser_OrderExists()
        {
            // arrange
            Customer customer = new Customer { ID = 1 };

            Order order = new Order { ID = 1, Customer = customer};
            orderDatabase.Add(order.ID, order);

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var result = service.ReadOrderByIDUser(order.ID, customer.ID);

            // assert
            Assert.Equal(order, result);
            repoMock.Verify(repo => repo.ReadOrderByIDUser(It.Is<int>(ID => ID == order.ID), It.Is<int>(userID => userID == customer.ID)), Times.Once);
        }

        [Fact]
        public void GetOrderByUserId_OrderExistUserNotValid_ExpectNull()
        {
            // arrange
            Customer customer1 = new Customer { ID = 1 };
            Customer customer2 = new Customer { ID = 2 };

            var order = new Order { ID = 1 , Customer = customer2};

            orderDatabase.Add(order.ID, order);

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var result = service.ReadOrderByIDUser(order.ID, customer1.ID);

            // assert
            Assert.Null(result);
            repoMock.Verify(repo => repo.ReadOrderByIDUser(It.Is<int>(ID => ID == order.ID), It.Is<int>(userID => userID == customer1.ID)), Times.Once);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 2)]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        public void GetOrderByUserId_OrderIDIsInvalid_ExpectArgumentException(int orderID, int userID)
        {
            // arrange
            Customer customer1 = new Customer { ID = 1 };
            Customer customer2 = new Customer { ID = 2 };

            var order = new Order { ID = 1, Customer = customer2 };

            orderDatabase.Add(order.ID, order);

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.ReadOrderByIDUser(orderID, userID));

            // assert
            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repo => repo.ReadOrderByIDUser(It.Is<int>(ID => ID == order.ID), It.Is<int>(userID => userID == customer1.ID)), Times.Never);
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

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var result = service.ReadAllOrders(new Filter { });

            // assert
            Assert.Equal(expected, result.List);
            repoMock.Verify(repo => repo.ReadAllOrders(It.IsAny<Filter>()), Times.Once);
        }

        [Theory]
        [InlineData(-1, 5)]
        [InlineData(2, -50)]
        public void GetAllOrders_InvalidPaging_ExpectInvalidDataException(int currentPage, int itemsPrPage)
        {
            // arange
            Filter filter = new Filter { CurrentPage = currentPage, ItemsPrPage = itemsPrPage };

            Order order1 = new Order { ID = 1};
            Order order2 = new Order { ID = 2};

            orderDatabase.Add(order1.ID, order1);
            orderDatabase.Add(order2.ID, order2);

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var ex = Assert.Throws<InvalidDataException>(() => service.ReadAllOrders(filter));

            // assert
            Assert.Equal("Page or items per page must be above zero", ex.Message);
            repoMock.Verify(repo => repo.ReadAllOrders(It.Is<Filter>(f => f == filter)), Times.Never);
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
            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var result = service.ReadAllOrdersByCustomer(customer2.ID, new Filter { });

            // assert
            Assert.Equal(expected, result.List);
            repoMock.Verify(repo => repo.ReadAllOrdersByCustomer(It.Is<int>(ID => ID == customer2.ID), It.IsAny<Filter>()), Times.Once);
        }

        [Theory]
        [InlineData(6, -2)]
        [InlineData(-1, 3)]
        public void GetAllOrdersByCustomerID_InvalidPaginmg_ExpectInvalidDataException(int currentPage, int itemsPrPage)
        {
            // arrange
            Filter filter = new Filter { CurrentPage = currentPage, ItemsPrPage = itemsPrPage };

            Customer customer1 = new Customer { ID = 1 };
            Customer customer2 = new Customer { ID = 2 };
            customerDatabase.Add(customer1.ID, customer1);
            customerDatabase.Add(customer2.ID, customer2);

            Order order1 = new Order { ID = 1, Customer = customer1 };
            Order order2 = new Order { ID = 2, Customer = customer1 };
            Order order3 = new Order { ID = 3, Customer = customer2 };

            orderDatabase.Add(order1.ID, order1);
            orderDatabase.Add(order2.ID, order2);
            orderDatabase.Add(order3.ID, order3);

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var ex = Assert.Throws<InvalidDataException>(() => service.ReadAllOrdersByCustomer(customer2.ID, filter));

            // assert
            Assert.Equal("Page or items per page must be above zero", ex.Message);
            repoMock.Verify(repo => repo.ReadAllOrdersByCustomer(It.Is<int>(ID => ID == customer2.ID), It.Is<Filter>(f => f == filter)), Times.Never);
        }
        #endregion

        #region Update Order Tests
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void UpdateOrderStatus_ValidExistingOrder(int orderID)
        {
            // arrange
            Customer customer = new Customer { ID = 1, FirstName = "Hans", LastName = "Christiansen"};
            customerDatabase.Add(customer.ID, customer);

            Order order = new Order()
            {
                ID = orderID,
                Customer = customer,
                OrderBeers = new List<OrderBeer> { },
                OrderFinished = false
            };
            orderDatabase.Add(order.ID, order);


            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            service.UpdateOrderStatus(orderID);

            // assert
            repoMock.Verify(repoMock => repoMock.ReadOrderByID(It.Is<int>(ID => ID == order.ID)), Times.Once);
            emailMock.Verify(emailMock => emailMock.SendConfirmationEmail(It.Is<Order>(or => or == order)), Times.Once);
            repoMock.Verify(repo => repo.UpdateOrder(It.Is<Order>(o => o == order)), Times.Once);
            Assert.True(orderDatabase[orderID].OrderFinished);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void UpdateOrderStatus_OrderIsInvalid_ExceptArgumentException(int orderID)
        {
            // arrange
            Customer customer = new Customer { ID = 1, FirstName = "Hans", LastName = "Christiansen" };
            customerDatabase.Add(customer.ID, customer);

            Order order = new Order()
            {
                ID = orderID,
                Customer = customer,
                OrderBeers = new List<OrderBeer> { },
                OrderFinished = false
            };

            orderDatabase.Add(order.ID, order);

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateOrderStatus(orderID));

            // assert
            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repoMock => repoMock.ReadOrderByID(It.Is<int>(ID => ID == order.ID)), Times.Never);
            emailMock.Verify(emailMock => emailMock.SendConfirmationEmail(It.Is<Order>(or => or == order)), Times.Never);
            repoMock.Verify(repo => repo.UpdateOrder(It.Is<Order>(o => o == order)), Times.Never);
            Assert.False(orderDatabase[orderID].OrderFinished);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void UpdateOrderStatus_OrderDoesNotExist_ExceptInvalidOperationException(int orderID)
        {
            // arrange
            Customer customer = new Customer { ID = 1, FirstName = "Hans", LastName = "Christiansen" };
            customerDatabase.Add(customer.ID, customer);

            Order order = new Order()
            {
                ID = orderID,
                Customer = customer,
                OrderBeers = new List<OrderBeer> { },
                OrderFinished = false
            };

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act
            var ex = Assert.Throws<InvalidOperationException>(() => service.UpdateOrderStatus(orderID));

            // assert
            Assert.Equal("No order with such ID found", ex.Message);
            repoMock.Verify(repoMock => repoMock.ReadOrderByID(It.Is<int>(ID => ID == order.ID)), Times.Once);
            emailMock.Verify(emailMock => emailMock.SendConfirmationEmail(It.Is<Order>(or => or == order)), Times.Never);
            repoMock.Verify(repo => repo.UpdateOrder(It.Is<Order>(o => o == order)), Times.Never);
        }
        #endregion

        #region Delete Order Tests
        [Fact]
        public void RemoveOrder_ValidExistingOrder()
        {
            // arrange
            Order order = new Order()
            {
                ID = 1
            };

            orderDatabase.Add(order.ID, order);

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

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

            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

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
            OrderService service = new OrderService(repoMock.Object, beerMock.Object, customerMock.Object, validatorMock.Object, emailMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.DeleteOrder(ID));

            // assert
            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repo => repo.ReadOrderByID(It.Is<int>(id => id == ID)), Times.Never);
            repoMock.Verify(repo => repo.DeleteOrder(It.Is<int>(id => id == ID)), Times.Never);
        }
        #endregion
    }
}
