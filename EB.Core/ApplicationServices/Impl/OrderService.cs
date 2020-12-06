using EB.Core.DomainServices;
using EB.Core.Entities;
using ProductShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EB.Core.ApplicationServices.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository OrderRepository;
        private readonly IBeerRepository BeerRepository;
        private readonly ICustomerRepository CustomerRepository;
        private readonly IValidator Validator;
        private readonly IEmailHelper EmailHelper;

        public OrderService(IOrderRepository orderRepository, IBeerRepository beerRepository, ICustomerRepository customerRepository, IValidator validator, IEmailHelper emailHelper)
        {
            this.OrderRepository = orderRepository ?? throw new NullReferenceException("Repository can't be null");
            this.BeerRepository = beerRepository ?? throw new NullReferenceException("Repository can't be null");
            this.CustomerRepository = customerRepository ?? throw new NullReferenceException("Repository can't be null");
            this.Validator = validator ?? throw new NullReferenceException("Validator can't be null");
            this.EmailHelper = emailHelper ?? throw new NullReferenceException("Email helper can't be null");
        }

        public Order AddOrder(Order order)
        {   //Check if order is null or customer exists
            if (order == null) { throw new ArgumentException("Attached order does not exist"); }
            if (order.Customer == null || CustomerRepository.ReadCustomerById(order.Customer.ID) == null) { throw new ArgumentException("Customer cannot be null"); }
            this.Validator.ValidateOrder(order);

            List<Beer> updatedBeers = new List<Beer>();
            double price = 0;

            //Check every beer if stock is available

            foreach (OrderBeer orderBeer in order.OrderBeers)
            {
                Beer savedBeer = BeerRepository.ReadSimpleBeerByID(orderBeer.BeerID);
                if (savedBeer.Stock < orderBeer.Amount) { throw new InvalidOperationException("Order amount higher than inventory stock"); }
                else { savedBeer.Stock -= orderBeer.Amount; price += orderBeer.Amount * savedBeer.Price; updatedBeers.Add(savedBeer); }
            }

            BeerRepository.UpdateBeerRange(updatedBeers);

            order.AccumulatedPrice = Math.Round(price, 2);
            Order addedOrder = OrderRepository.AddOrder(order);
            EmailHelper.SendVerificationEmail(addedOrder);
            return addedOrder;
        }

        public FilterList<Order> ReadAllOrders(Filter filter)
        {
            if (filter.CurrentPage < 0 || filter.ItemsPrPage < 0)
            {
                throw new InvalidDataException("Page or items per page must be above zero");
            }

            return OrderRepository.ReadAllOrders(filter);
        }

        public FilterList<Order> ReadAllOrdersByCustomer(int id, Filter filter)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }
            if (filter.CurrentPage < 0 || filter.ItemsPrPage < 0)
            {
                throw new InvalidDataException("Page or items per page must be above zero");
            }

            return OrderRepository.ReadAllOrdersByCustomer(id, filter);
        }

        public Order ReadOrderByID(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }

            return OrderRepository.ReadOrderByID(id);
        }

        public Order ReadOrderByIDUser(int orderID, int userID)
        {
            if (orderID <= 0 || userID <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }

            return OrderRepository.ReadOrderByIDUser(orderID, userID);
        }

        public Order UpdateOrderStatus(int orderID)
        {
            Order order = ReadOrderByID(orderID);

            if (order == null)
            {
                throw new InvalidOperationException("No order with such ID found");
            }

            order.OrderFinished = true;

            EmailHelper.SendConfirmationEmail(order);
            return OrderRepository.UpdateOrder(order);
        }

        public Order DeleteOrder(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }
            if (ReadOrderByID(id) == null)
            {
                throw new InvalidOperationException("No order with such ID found");
            }
            return OrderRepository.DeleteOrder(id);
        }
    }
}
