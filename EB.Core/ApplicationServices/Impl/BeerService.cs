using EB.Core.DomainServices;
using EB.Core.Entities;
using ProductShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EB.Core.ApplicationServices.Impl
{
    public class BeerService : IBeerService
    {
        #region Dependency Injection
        private IBeerRepository BeerRepository;
        private IBrandRepository BrandRepository;
        private IBeerTypeRepository TypeRepository;
        private IValidator Validator;

        public BeerService(IBeerRepository beerRepository, IBrandRepository brandRepository, IBeerTypeRepository typeRepository, IValidator validator)
        {
            this.BeerRepository = beerRepository ?? throw new NullReferenceException("Repository can't be null");
            this.BrandRepository = brandRepository ?? throw new NullReferenceException("Repository can't be null");
            this.TypeRepository = typeRepository ?? throw new NullReferenceException("Repository can't be null");
            this.Validator = validator ?? throw new NullReferenceException("Validator can't be null");
        }
        #endregion

        #region Validate
        public Beer ValidateBeer(Beer beer)
        {
            this.Validator.ValidateBeer(beer);
            return beer;
        }
        #endregion

        #region Create
        public Beer CreateBeer(Beer beer)
        {
            if (beer != null)
            {
                Validator.ValidateBeer(beer);

                if (TypeRepository.ReadTypeById(beer.Type.ID) == null)
                {
                    throw new InvalidOperationException("No type with such ID found");
                }
                if (BrandRepository.ReadBrandById(beer.Brand.ID) == null)
                {
                    throw new InvalidOperationException("No brand with such ID found");
                }
                return BeerRepository.AddBeer(beer);
            }
            return null;
        }
        #endregion

        #region Read
        public List<Beer> GetAllBeer()
        {
            return BeerRepository.ReadBeers().ToList();
        }

        public FilterList<Beer> GetBeerFilterSearch(Filter filter)
        {
            if (filter.CurrentPage < 0 || filter.ItemsPrPage < 0)
            {
                throw new InvalidDataException("Page or items per page must be above zero");
            }

            return BeerRepository.ReadBeersFilterSearch(filter);
        }

        public Beer GetBeerById(int id)
        {
            if(id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }

            return BeerRepository.ReadBeerById(id);
        }
        #endregion

        #region Update
        public Beer UpdateBeer(Beer beer)
        {
            if (beer == null)
            {
                throw new ArgumentException("Updating beer does not exist");
            }
            Validator.ValidateBeer(beer);
            if (GetBeerById(beer.ID) == null)
            {
                throw new InvalidOperationException("No beer with such ID found");
            }
            if (TypeRepository.ReadTypeById(beer.Type.ID) == null)
            {
                throw new InvalidOperationException("No type with such ID found");
            }
            if (BrandRepository.ReadBrandById(beer.Brand.ID) == null)
            {
                throw new InvalidOperationException("No brand with such ID found");
            }

            return BeerRepository.UpdateBeerInRepo(beer);
        }
        #endregion

        #region Delete
        public Beer DeleteBeer(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }
            if (GetBeerById(id) == null)
            {
                throw new InvalidOperationException("No beer with such ID found");
            }
            return BeerRepository.DeleteBeerInRepo(id);
        }
        #endregion
    }
}
