using EB.Core.Entities;
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
        List<Brand> GetAllBrand();
        Brand GetBrandById(int id);
        //List<Brand> GetFilteredBrands(Filter filter);

      //Update
        Brand UpdateBrand(Brand brand);

      //Delete
        Brand DeleteBrand(int id);
    }
}
