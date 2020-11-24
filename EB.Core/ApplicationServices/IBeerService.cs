using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.ApplicationServices
{
    public interface IBeerService
    {
      //Create
        Beer ValidateBeer(Beer beer);
        Beer CreateBeer(Beer beer);

      //Read
        List<Beer> GetAllBeer();
        Beer GetBeerById(int id);
        //List<Beer> GetFilteredBeers(Filter filter);

      //Update
        Beer UpdateBeer(Beer beer);

      //Delete
        Beer DeleteBeer(int id);
    }
}
