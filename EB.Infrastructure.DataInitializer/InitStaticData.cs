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
                ImageURL = "https://firebasestorage.googleapis.com/v0/b/eb-sdm3.appspot.com/o/225023694.png?alt=media&token=e54b5623-a7dc-4a6b-b911-0ed503428f9c",
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
                ImageURL = "https://firebasestorage.googleapis.com/v0/b/eb-sdm3.appspot.com/o/548622110.png?alt=media&token=eb67524a-38c1-4fc7-a21b-3de34d85591d",
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
                ImageURL = "https://firebasestorage.googleapis.com/v0/b/eb-sdm3.appspot.com/o/566781339.png?alt=media&token=cf4fc0bc-5917-463d-8e34-e06ff5659fca",
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
                ImageURL = "https://firebasestorage.googleapis.com/v0/b/eb-sdm3.appspot.com/o/566781339.png?alt=media&token=cf4fc0bc-5917-463d-8e34-e06ff5659fca",
                Type = saison,
                Brand = ølværket
            };

            BeerService.CreateBeer(borebisse);
            BeerService.CreateBeer(cargo);
            BeerService.CreateBeer(witch);
            BeerService.CreateBeer(witchGinger);

            UserService.AddUser(UserService.CreateUser("SeglerHans", "kodeord", "Admin"));
            UserService.AddUser(UserService.CreateUser("Kutterjørgen", "lasagne28", "User"));

        }
    }
}
