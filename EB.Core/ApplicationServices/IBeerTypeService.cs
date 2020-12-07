using EB.Core.Entities;
using ProductShop.Core.Entities;
using System.Collections.Generic;

namespace EB.Core.ApplicationServices
{
    public interface IBeerTypeService
    {
        //Create
        BeerType ValidateType(BeerType type);
        BeerType CreateType(BeerType type);

        //Read
        List<BeerType> GetAllTypes();
        FilterList<BeerType> GetTypesFilterSearch(Filter filter);
        BeerType GetTypeById(int id);

        //Update
        BeerType UpdateType(BeerType type);

        //Delete
        BeerType DeleteType(int id);
    }
}
