using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.ApplicationServices
{
    public interface IBeerTypeService
    {
      //Create
        BeerType ValidateBeerType(BeerType type);
        BeerType CreateBeerType(BeerType type);

      //Read
        List<BeerType> GetAllBeerType();
        BeerType GetBeerTypeById(int id);
        //List<BeerType> GetFilteredBeerTypes(Filter filter);

      //Update
        BeerType UpdateBeerType(BeerType type);

      //Delete
        BeerType DeleteBeerType(int id);
    }
}
