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
    public class BeerRepository : IBeerRepository
    {
        private EBContext ctx;

        public BeerRepository(EBContext ctx)
        {
            this.ctx = ctx;
        }

        public Beer AddBeer(Beer beer)
        {
            ctx.Attach(beer).State = EntityState.Added;
            ctx.SaveChanges();

            return beer;
        }

        public IEnumerable<Beer> ReadBeers()
        {
            return ctx.Beers.Include(b => b.Type).Include(b => b.Brand).AsEnumerable();
        }

        public FilterList<Beer> ReadBeersFilterSearch(Filter filter)
        {
            IQueryable<Beer> beers = ctx.Beers.Include(b => b.Type).Include(b => b.Brand).AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                beers = from x in beers where x.Name.ToLower().Contains(filter.Name.ToLower()) select x;
            }
            if (filter.BeerTypeID != 0)
            {
                beers = from x in beers where x.Type.ID == filter.BeerTypeID select x;
            }
            if (!string.IsNullOrEmpty(filter.Sorting) && filter.Sorting.ToLower().Equals("asc"))
            {
                if (filter.SortingType.Equals("IBU")) { beers = from x in beers orderby x.IBU select x; }
                if (filter.SortingType.Equals("EBC")) { beers = from x in beers orderby x.EBC select x; }
                if (filter.SortingType.Equals("ALF")) { beers = from x in beers orderby x.Name select x; }
            }
            else if (!string.IsNullOrEmpty(filter.Sorting) && filter.Sorting.ToLower().Equals("desc"))
            {
                if (filter.SortingType.Equals("IBU")) { beers = from x in beers orderby x.IBU descending select x; }
                if (filter.SortingType.Equals("EBC")) { beers = from x in beers orderby x.EBC descending select x; }
                if (filter.SortingType.Equals("ALF")) { beers = from x in beers orderby x.Name descending select x; }
                if (filter.SortingType.Equals("ADDED")) { beers = from x in beers orderby x descending select x; }
            }

            int totalItems = beers.Count();

            if (filter.CurrentPage > 0)
            {
                beers = beers.Skip((filter.CurrentPage - 1) * filter.ItemsPrPage).Take(filter.ItemsPrPage);
                if (beers.Count() == 0 && filter.CurrentPage > 1)
                {
                    throw new InvalidDataException("Index out of bounds");
                }
            }

            FilterList<Beer> filterList = new FilterList<Beer> { totalItems = totalItems, List = beers.ToList() };

            return filterList;
        }

        public Beer ReadBeerById(int id)
        {
            return ctx.Beers.Include(beers => beers.Type).Include(beer => beer.Brand).FirstOrDefault(x => x.ID == id);
        }

        public Beer UpdateBeerInRepo(Beer beerUpdate)
        {
            ctx.Attach(beerUpdate).State = EntityState.Modified;
            ctx.Entry(beerUpdate).Reference(product => product.Type).IsModified = true;
            ctx.Entry(beerUpdate).Reference(product => product.Brand).IsModified = true;
            ctx.SaveChanges();

            return ReadBeerById(beerUpdate.ID);
        }


        public Beer DeleteBeerInRepo(int id)
        {
            var removedBeer = ctx.Beers.Remove(ReadBeerById(id));
            ctx.SaveChanges();
            return removedBeer.Entity;
        }








    }
}
