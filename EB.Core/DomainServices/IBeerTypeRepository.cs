using EB.Core.Entities;
using ProductShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.DomainServices
{
    public interface IBeerTypeRepository
    {
        //Create Data
        BeerType AddType(BeerType type);

        //Read Data
        IEnumerable<BeerType> ReadTypes();
        FilterList<BeerType> ReadTypesFilterSearch(Filter filter);
        BeerType ReadTypeById(int id);

        //Update Data
        BeerType UpdateTypeInRepo(BeerType typeUpdate);

        //Delete Data
        BeerType DeleteTypeInRepo(int id);
    }
}
