using EB.Core.Entities;
using ProductShop.Core.Entities;
using System.Collections.Generic;

namespace EB.Core.ApplicationServices
{
    public interface IBeerService
    {
      //Create
        Beer ValidateBeer(Beer beer);
        Beer CreateBeer(Beer beer);

        //Read
        List<Beer> GetAllBeer();
        FilterList<Beer> GetBeerFilterSearch(Filter filter);
        Beer GetBeerById(int id);

      //Update
        Beer UpdateBeer(Beer beer);

      //Delete
        Beer DeleteBeer(int id);
    }
}
