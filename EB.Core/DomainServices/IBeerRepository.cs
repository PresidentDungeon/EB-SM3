using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.DomainServices
{
    public interface IBeerRepository
    {
        //Create Data
        Beer AddBeer(Beer beer);

        //Read Data
        IEnumerable<Beer> ReadAllBeer(Filter filter);
        Beer ReadBeerById(int id);

        //Update Data
        Beer UpdateBeerInRepo(Beer beerUpdate);

        //Delete Data
        Beer DeleteBeerInRepo(int id);
    }
}
