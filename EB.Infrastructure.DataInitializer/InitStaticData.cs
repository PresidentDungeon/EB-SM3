using EB.Core.ApplicationServices;
using EB.Core.DomainServices;
using EB.Core.Entities;
using EB.Core.Entities.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Infrastructure.DataInitializer
{
    public class InitStaticData: IInitStaticData
    {
        private IBeerService BeerService;
        private IBeerTypeService BeerTypeService;
        private IBrandService BrandService;
        private IUserService UserService;

        public InitStaticData(IBeerService beerService, IBeerTypeService beerTypeService, IBrandService brandService,IUserService UserService)
        {
            this.BeerService = beerService;
            this.BeerTypeService = beerTypeService;
            this.BrandService = brandService;
            this.UserService = UserService;
        }

        public void InitData()
        {
            BeerType hvedeøl = new BeerType {ID = 1, TypeName = "Hvedeøl" };
            BeerType steam = new BeerType {ID = 2, TypeName = "Steam" };
            BeerType stout = new BeerType {ID = 3, TypeName = "Stout" };
            BeerType IPA = new BeerType {ID = 4, TypeName = "IPA" };
            BeerType pilsner = new BeerType {ID = 5, TypeName = "Pilsner" };
            BeerType saison = new BeerType {ID = 6, TypeName = "Saison" };

            BeerTypeService.CreateType(hvedeøl);
            BeerTypeService.CreateType(steam);
            BeerTypeService.CreateType(stout);
            BeerTypeService.CreateType(IPA);
            BeerTypeService.CreateType(pilsner);
            BeerTypeService.CreateType(saison);

            Brand ølværket = new Brand {ID = 1, BrandName = "Ølværket" };
            Brand esbjergBryhus = new Brand {ID = 2, BrandName = "Esbjerg Bryghus" };
            Brand carlsberg = new Brand {ID = 3, BrandName = "Carlsberg" };

            BrandService.CreateBrand(ølværket);
            BrandService.CreateBrand(esbjergBryhus);
            BrandService.CreateBrand(carlsberg);

            Beer borebisse = new Beer
            {
                Name = "Borebisse",
                Description = "Noter af lakrids, kaffe og karamel.",
                Price = 49.95,
                Percentage = 8.5,
                IBU = 60,
                EBC = 70,
                Stock = 5,
                ImageURL = "https://firebasestorage.googleapis.com/v0/b/eb-sdm3.appspot.com/o/XL%20BOREBISSE.png?alt=media&token=e810dddf-e099-4ea5-ab4d-af5067eba37b",
                Type = stout,
                Brand = ølværket
            };

            Beer cargo = new Beer
            {
                Name = "Cargo",
                Description = "Noter af Citrus",
                Price = 49.95,
                Percentage = 5.4,
                IBU = 60,
                EBC = 26,
                Stock = 3,
                ImageURL = "https://firebasestorage.googleapis.com/v0/b/eb-sdm3.appspot.com/o/XL%20CARGO.png?alt=media&token=8b5a4933-7331-401f-b906-0590e45ae20c",
                Type = steam,
                Brand = ølværket
            };

            Beer witch = new Beer
            {
                Name = "Saison of the Witch",
                Description = "Noter af Appelsin og Grape",
                Price = 49.95,
                Percentage = 7.5,
                IBU = 60,
                EBC = 26,
                Stock = 99,
                ImageURL = "https://firebasestorage.googleapis.com/v0/b/eb-sdm3.appspot.com/o/258701189.png?alt=media&token=988b3f15-f5db-424f-bc3e-1ca6a497acf6",
                Type = saison,
                Brand = ølværket
            };

            Beer witchGinger = new Beer
            {
                Name = "Saison of the Witch Ginger",
                Description = "Noter af Appelsin, Grape og Ingefær",
                Price = 49.95,
                Percentage = 7.5,
                IBU = 60,
                EBC = 26,
                Stock = 10,
                ImageURL = "https://firebasestorage.googleapis.com/v0/b/eb-sdm3.appspot.com/o/149572519.png?alt=media&token=d28f0523-22c5-4ade-9412-d296ce964986",
                Type = saison,
                Brand = ølværket
            };

            BeerService.CreateBeer(borebisse);
            BeerService.CreateBeer(cargo);
            BeerService.CreateBeer(witch);
            BeerService.CreateBeer(witchGinger);

            UserService.AddUser(UserService.CreateUser("SeglerHans", "password", "Admin"));
            UserService.AddUser(UserService.CreateUser("Kutterjørgen", "lasagne28", "User"));

        }
    }
}
