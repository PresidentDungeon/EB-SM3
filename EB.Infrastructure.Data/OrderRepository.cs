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
        private EBContext ctx;

        public OrderRepository(EBContext ctx)
        {
            this.ctx = ctx;
        }

        public Order AddOrder(Order order)
        {
            ctx.Attach(order).State = EntityState.Added;
            ctx.SaveChanges();

            return order;
        }

        public IEnumerable<Order> ReadAllOrders()
        {
            return ctx.Orders.AsEnumerable();
        }

        public IEnumerable<Order> ReadAllOrdersByCustomer(int id)
        {
            return ctx.Orders.Where(o => o.Customer.ID == id).AsEnumerable();
        }

        public Order ReadOrderByID(int id)
        {
            return ctx.Orders.Include(o => o.OrderBeers).ThenInclude(ob => ob.Beer).FirstOrDefault(x => x.ID == id);
        }

        public Order DeleteOrder(int id)
        {
            var deletedOrder = ctx.Orders.Remove(ReadOrderByID(id));
            ctx.SaveChanges();
            return deletedOrder.Entity;
        }

    }
}
