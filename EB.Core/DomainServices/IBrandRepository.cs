using EB.Core.Entities;
using ProductShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.DomainServices
{
    public interface IBrandRepository
    {
        //Create Data
        Brand AddBrand(Brand brand);

        //Read Data
        IEnumerable<Brand> ReadBrands();
        FilterList<Brand> ReadBrandsFilterSearch(Filter filter);
        Brand ReadBrandById(int id);

        //Update Data
        Brand UpdateBrandInRepo(Brand brand);

        //Delete Data
        Brand DeleteBrandInRepo(int id);
    }
}
