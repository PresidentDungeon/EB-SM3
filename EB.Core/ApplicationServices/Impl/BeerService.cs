using EB.Core.DomainServices;
using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.ApplicationServices.Impl
{
    public class BeerService : IBeerService
    {
        private IBeerRepository beerRepository;
        private IValidator validator;

        public BeerService(IBeerRepository beerRepository, IValidator validator)
        {
            this.beerRepository = beerRepository ?? throw new NullReferenceException("Repository can't be null");
            this.validator = validator ?? throw new NullReferenceException("Validator can't be null");
        }


        public Beer CreateBeer(Beer beer)
        {
            throw new NotImplementedException();
        }

        public Beer DeleteBeer(int id)
        {
            throw new NotImplementedException();
        }

        public List<Beer> GetAllBeer()
        {
            throw new NotImplementedException();
        }

        public Beer GetBeerById(int id)
        {
            throw new NotImplementedException();
        }

        public Beer UpdateBeer(Beer beer)
        {
            throw new NotImplementedException();
        }

        public Beer ValidateBeer(Beer beer)
        {
            throw new NotImplementedException();
        }
    }
}
