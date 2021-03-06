﻿using EB.Core.Entities;
using System.Collections.Generic;
using ProductShop.Core.Entities;

namespace EB.Core.DomainServices
{
    public interface IBeerRepository
    {
        //Create Data
        Beer AddBeer(Beer beer);

        //Read Data
        IEnumerable<Beer> ReadBeers();
        FilterList<Beer> ReadBeersFilterSearch(Filter filter);
        Beer ReadBeerById(int id);
        Beer ReadSimpleBeerByID(int id);

        //Update Data
        Beer UpdateBeerInRepo(Beer beerUpdate);
        void UpdateBeerRange(List<Beer> beers);

        //Delete Data
        Beer DeleteBeerInRepo(int id);
    }
}
