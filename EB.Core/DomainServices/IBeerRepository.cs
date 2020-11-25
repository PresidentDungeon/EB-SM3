using System;
using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
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

        //Update Data
        Beer UpdateBeerInRepo(Beer beerUpdate);

        //Delete Data
        Beer DeleteBeerInRepo(int id);
    }
}
