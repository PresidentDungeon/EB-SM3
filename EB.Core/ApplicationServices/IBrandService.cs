using EB.Core.Entities;
using ProductShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.ApplicationServices
{
    public interface IBrandService
    {
        //Create
        Brand ValidateBrand(Brand brand);
        Brand CreateBrand(Brand brand);

        //Read
        List<Brand> GetAllBrands();
        FilterList<Brand> GetBrandFilterSearch(Filter filter);
        Brand GetBrandById(int id);

        //Update
        Brand UpdateBrand(Brand brand);

        //Delete
        Brand DeleteBrand(int id);
    }
}
