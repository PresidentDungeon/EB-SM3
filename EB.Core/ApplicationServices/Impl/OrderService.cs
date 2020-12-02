using EB.Core.DomainServices;
using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB.Core.ApplicationServices.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository OrderRepository;
        private readonly ICustomerRepository CustomerRepository;
        private readonly IValidator Validator;

        public OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepository, IValidator validator)
        {
            this.OrderRepository = orderRepository ?? throw new NullReferenceException("Repository can't be null");
            this.CustomerRepository = customerRepository ?? throw new NullReferenceException("Repository can't be null");
            this.Validator = validator ?? throw new NullReferenceException("Validator can't be null");
        }

        public Order AddOrder(Order order)
        {
            if (order != null)
            {
                if(order.Customer == null || CustomerRepository.ReadCustomerById(order.Customer.ID) == null)
                {
                    throw new ArgumentException("Customer cannot be null");
                }

                this.Validator.ValidateOrder(order);
                return OrderRepository.AddOrder(order);
            }
            return null;
        }

        public List<Order> ReadAllOrders()
        {
            return OrderRepository.ReadAllOrders().ToList();
        }

        public List<Order> ReadAllOrdersByCustomer(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }
            return OrderRepository.ReadAllOrdersByCustomer(id).ToList();
        }

        public Order ReadOrderByID(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }

            return OrderRepository.ReadOrderByID(id);
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
