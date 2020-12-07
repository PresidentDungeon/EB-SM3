using EB.Core.DomainServices;
using EB.Core.Entities;
using Microsoft.EntityFrameworkCore;
using ProductShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
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

            return ReadOrderByID(order.ID);
        }

        public FilterList<Order> ReadAllOrders(Filter filter)
        {
            IQueryable<Order> orders = ctx.Orders.Include(order => order.Customer).AsQueryable();

            if (filter.OrderFinished) { orders = from x in orders where x.OrderFinished select x; }
            if (!filter.OrderFinished) { orders = from x in orders where !x.OrderFinished select x; }

            int totalItems = orders.Count();

            if (filter.CurrentPage > 0)
            {
                orders = orders.Skip((filter.CurrentPage - 1) * filter.ItemsPrPage).Take(filter.ItemsPrPage);
                if (orders.Count() == 0 && filter.CurrentPage > 1)
                {
                    throw new InvalidDataException("Index out of bounds");
                }
            }

            FilterList<Order> filterList = new FilterList<Order> { totalItems = totalItems, List = orders.ToList() };

            return filterList;
        }

        public FilterList<Order> ReadAllOrdersByCustomer(int id, Filter filter)
        {

            IQueryable<Order> orders = ctx.Orders.Where(o => o.Customer.ID == id).AsQueryable();

            int totalItems = orders.Count();

            if (filter.CurrentPage > 0)
            {
                orders = orders.Skip((filter.CurrentPage - 1) * filter.ItemsPrPage).Take(filter.ItemsPrPage);
                if (orders.Count() == 0 && filter.CurrentPage > 1)
                {
                    throw new InvalidDataException("Index out of bounds");
                }
            }

            FilterList<Order> filterList = new FilterList<Order> { totalItems = totalItems, List = orders.ToList() };

            return filterList;
        }

        public Order ReadOrderByID(int id)
        {
            return ctx.Orders.Include(o => o.Customer).Include(o => o.OrderBeers).ThenInclude(ob => ob.Beer).FirstOrDefault(x => x.ID == id);
        }

        public Order ReadOrderByIDUser(int orderID, int userID)
        {
            return ctx.Orders.Include(o => o.OrderBeers).ThenInclude(ob => ob.Beer).Where(o => o.Customer.ID == userID).FirstOrDefault(x => x.ID == orderID);
        }

        public Order UpdateOrder(Order order)
        {
            ctx.Attach(order).State = EntityState.Modified;
            ctx.Entry(order).Reference(order => order.Customer).IsModified = false;
            ctx.SaveChanges();

            return ReadOrderByID(order.ID);
        }

        public Order DeleteOrder(int id)
        {
            var deletedOrder = ctx.Orders.Remove(ReadOrderByID(id));
            ctx.SaveChanges();
            return deletedOrder.Entity;
        }

    }
}
