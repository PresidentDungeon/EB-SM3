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
        private readonly IValidator Validator;

        public OrderService(IOrderRepository orderRepository, IValidator validator)
        {
            this.OrderRepository = orderRepository ?? throw new NullReferenceException("Repository can't be null");
            this.Validator = validator ?? throw new NullReferenceException("Validator can't be null");
        }

        public Order CreateOrder(Order order)
        {
            Validator.ValidateOrder(order);
            return OrderRepository.CreateOrder(order);
        }

        public Order DeleteOrder(int id)
        {
            return OrderRepository.DeleteOrderInRepo(id);
        }

        public List<Order> GetAllOrders() //Later to be changed to specific Customer
        {
            
            return OrderRepository.ReadAllOrders().ToList();
        }

        public Order GetOrderById(int id)
        {
            
            return OrderRepository.ReadOrderById(id);
        }
    }
}
