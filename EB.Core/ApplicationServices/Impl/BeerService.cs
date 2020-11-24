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
    public class BeerService : IBeerService
    {
        private IBeerRepository BeerRepository;
        private IValidator Validator;

        public BeerService(IBeerRepository beerRepository, IValidator validator)
        {
            this.BeerRepository = beerRepository ?? throw new NullReferenceException("Repository can't be null");
            this.Validator = validator ?? throw new NullReferenceException("Validator can't be null");
        }

        public Beer ValidateBeer(Beer beer)
        {
            this.Validator.ValidateBeer(beer);
            return beer;
        }

        public Beer CreateBeer(Beer beer)
        {
            if (beer != null)
            {
                Validator.ValidateBeer(beer);
                return BeerRepository.AddBeer(beer);
            }
            return null;
        }

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

        public Beer UpdateBeer(Beer beer)
        {
            if (beer == null)
            {
                throw new ArgumentException("Updating beer does not exist");
            }
            Validator.ValidateBeer(beer);
            if (GetBeerById(beer.ID) == null)
            {
                throw new ArgumentException("No beer with such ID found");
            }

            return BeerRepository.UpdateBeerInRepo(beer);
        }

        public Beer DeleteBeer(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }
            if (GetBeerById(id) == null)
            {
                throw new ArgumentException("No beer with such ID found");
            }
            return BeerRepository.DeleteBeerInRepo(id);
        }
    }
}
