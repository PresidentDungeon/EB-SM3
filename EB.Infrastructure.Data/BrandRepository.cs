using EB.Core.DomainServices;
using EB.Core.Entities;
using Microsoft.EntityFrameworkCore;
using ProductShop.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EB.Infrastructure.Data
{
    public class BrandRepository : IBrandRepository
    {
        private EBContext ctx;

        public BrandRepository(EBContext ctx)
        {
            this.ctx = ctx;
        }

        public Brand AddBrand(Brand brand)
        {
            ctx.Attach(brand).State = EntityState.Added;
            ctx.SaveChanges();

            return brand;
        }

        public IEnumerable<Brand> ReadBrands()
        {
            return ctx.Brands.AsEnumerable();
        }

        public FilterList<Brand> ReadBrandsFilterSearch(Filter filter)
        {
            IQueryable<Brand> brands = ctx.Brands.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                brands = from x in brands where x.BrandName.Contains(filter.Name) select x;
            }
            if (!string.IsNullOrEmpty(filter.Sorting) && filter.Sorting.ToLower().Equals("asc"))
            {
                if (filter.SortingType.Equals("ALF")) { brands = from x in brands orderby x.BrandName select x; }
            }
            else if (!string.IsNullOrEmpty(filter.Sorting) && filter.Sorting.ToLower().Equals("desc"))
            {
                if (filter.SortingType.Equals("ALF")) { brands = from x in brands orderby x.BrandName descending select x; }
                if (filter.SortingType.Equals("ADDED")) { brands = from x in brands orderby x descending select x; }
            }

            int totalItems = brands.Count();

            if (filter.CurrentPage > 0)
            {
                brands = brands.Skip((filter.CurrentPage - 1) * filter.ItemsPrPage).Take(filter.ItemsPrPage);
                if (brands.Count() == 0 && filter.CurrentPage > 1)
                {
                    throw new InvalidDataException("Index out of bounds");
                }
            }

            FilterList<Brand> filterList = new FilterList<Brand> { totalItems = totalItems, List = brands.ToList() };
            return filterList;
        }

        public Brand ReadBrandById(int id)
        {
            return ctx.Brands.FirstOrDefault(x => x.ID == id);
        }

        public Brand UpdateBrandInRepo(Brand brand)
        {
            ctx.Attach(brand).State = EntityState.Modified;
            ctx.SaveChanges();
            return brand;
        }

        public Brand DeleteBrandInRepo(int id)
        {
            var removedBrand = ctx.Brands.Remove(ReadBrandById(id));
            ctx.SaveChanges();
            return removedBrand.Entity;
        }
    }
}
