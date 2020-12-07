using EB.Core.DomainServices;
using EB.Core.Entities;
using Microsoft.EntityFrameworkCore;
using ProductShop.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EB.Infrastructure.Data
{
    public class BeerTypeRepository : IBeerTypeRepository
    {
        #region Dependency Injection
        private EBContext ctx;
        public BeerTypeRepository(EBContext ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region Create Data
        public BeerType AddType(BeerType type)
        {
            ctx.Attach(type).State = EntityState.Added;
            ctx.SaveChanges();

            return type;
        }
        #endregion

        #region Read Data
        public IEnumerable<BeerType> ReadTypes()
        {
            return ctx.Types.AsEnumerable();
        }

        public FilterList<BeerType> ReadTypesFilterSearch(Filter filter)
        {
            IQueryable<BeerType> types = ctx.Types.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                types = from x in types where x.TypeName.Contains(filter.Name) select x;
            }
            if (!string.IsNullOrEmpty(filter.Sorting) && filter.Sorting.ToLower().Equals("asc"))
            {
                if (filter.SortingType.Equals("ALF")) { types = from x in types orderby x.TypeName select x; }
            }
            else if (!string.IsNullOrEmpty(filter.Sorting) && filter.Sorting.ToLower().Equals("desc"))
            {
                if (filter.SortingType.Equals("ALF")) { types = from x in types orderby x.TypeName descending select x; }
                if (filter.SortingType.Equals("ADDED")) { types = from x in types orderby x descending select x; }
            }

            int totalItems = types.Count();

            if (filter.CurrentPage > 0)
            {
                types = types.Skip((filter.CurrentPage - 1) * filter.ItemsPrPage).Take(filter.ItemsPrPage);
                if (types.Count() == 0 && filter.CurrentPage > 1)
                {
                    throw new InvalidDataException("Index out of bounds");
                }
            }

            FilterList<BeerType> filterList = new FilterList<BeerType> { totalItems = totalItems, List = types.ToList()};

            return filterList;
        }

        public BeerType ReadTypeById(int id)
        {
            return ctx.Types.FirstOrDefault(x => x.ID == id);
        }
        #endregion

        #region Update Data
        public BeerType UpdateTypeInRepo(BeerType typeUpdate)
        {
            ctx.Attach(typeUpdate).State = EntityState.Modified;
            ctx.SaveChanges();
            return typeUpdate;
        }
        #endregion

        #region Delete Data
        public BeerType DeleteTypeInRepo(int id)
        {
            var removedType = ctx.Types.Remove(ReadTypeById(id));
            ctx.SaveChanges();
            return removedType.Entity;
        }
        #endregion
    }
}
