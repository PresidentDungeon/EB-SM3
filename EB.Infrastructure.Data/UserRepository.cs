using EB.Core.DomainServices;
using EB.Core.Entities;
using EB.Core.Entities.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EB.Infrastructure.Data
{
    public class UserRepository: IUserRepository
    {
        private EBContext ctx;

        public UserRepository(EBContext ctx)
        {
            this.ctx = ctx;
        }

        public User AddUser(User user)
        {
            ctx.Attach(user).State = EntityState.Added;
            ctx.SaveChanges();
            return user;
        }

        public IEnumerable<User> ReadUsers()
        {
            return ctx.Users.Include(u => u.Customer).AsEnumerable();
        }

        public User GetUserByID(int ID)
        {
            return ctx.Users.Include(u => u.Customer).FirstOrDefault(x => x.ID == ID);
        }

        public User UpdateUser(User user)
        {
            ctx.Attach(user).State = EntityState.Modified;
            ctx.Entry(user).Reference(user => user.Customer).IsModified = true;
            ctx.SaveChanges();

            return GetUserByID(user.ID);
        }

        public User DeleteUser(int ID)
        {
            var deletedUser = ctx.Users.Remove(GetUserByID(ID));
            ctx.SaveChanges();
            return deletedUser.Entity;
        }
    }
}
