using EB.Core.DomainServices;
using EB.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB.Infrastructure.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EBContext ctx;

        public OrderRepository(EBContext ctx)
        {
            this.ctx = ctx;
        }

        public Order CreateOrder(Order order)
        {
            ctx.Attach(order).State = EntityState.Added;
            ctx.SaveChanges();

            return order;
        }

        public Order DeleteOrderInRepo(int id)
        {
            var removedOrder = ctx.Orders.Remove(ReadOrderById(id));
            ctx.SaveChanges();
            return removedOrder.Entity;
        }

        public IEnumerable<Order> ReadAllOrders()
        {
            return ctx.Orders.AsEnumerable();
        }

        public Order ReadOrderById(int id)
        {
            return ctx.Orders.FirstOrDefault(x => x.ID == id);
        }
    }
}
