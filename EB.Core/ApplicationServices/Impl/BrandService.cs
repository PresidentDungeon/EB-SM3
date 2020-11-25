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
    public class BrandService : IBrandService
    {
        private IBrandRepository BrandRepository;
        private IValidator Validator;

        public BrandService(IBrandRepository brandRepository, IValidator validator)
        {
            this.BrandRepository = brandRepository ?? throw new NullReferenceException("Repository can't be null");
            this.Validator = validator ?? throw new NullReferenceException("Validator can't be null");
        }

        public Brand ValidateBrand(Brand brand)
        {
            this.Validator.ValidateBrand(brand);
            return brand;
        }

        public Brand CreateBrand(Brand brand)
        {
            if (brand != null)
            {
                this.Validator.ValidateBrand(brand);
                return BrandRepository.AddBrand(brand);
            }
            return null;
        }

        public List<Brand> GetAllBrands()
        {
            return BrandRepository.ReadBrands().ToList();
        }

        public FilterList<Brand> GetBrandFilterSearch(Filter filter)
        {
            if (filter.CurrentPage < 0 || filter.ItemsPrPage < 0)
            {
                throw new InvalidDataException("Page or items per page must be above zero");
            }

            return BrandRepository.ReadBrandsFilterSearch(filter);
        }

        public Brand GetBrandById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }

            return BrandRepository.ReadBrandById(id);
        }

        public Brand UpdateBrand(Brand brand)
        {
            if (brand == null)
            {
                throw new ArgumentException("Updating brand does not exist");
            }
            Validator.ValidateBrand(brand);
            if (GetBrandById(brand.ID) == null)
            {
                throw new ArgumentException("No brand with such ID found");
            }

            return BrandRepository.UpdateBrandInRepo(brand);
        }

        public Brand DeleteBrand(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }
            if (GetBrandById(id) == null)
            {
                throw new ArgumentException("No brand with such ID found");
            }
            return BrandRepository.DeleteBrandInRepo(id);
        }
    }
}
