using EB.Core.ApplicationServices;
using EB.Core.DomainServices;
using EB.Core.Entities;
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

            BeerTypeService.CreateType(hvedeøl);
            BeerTypeService.CreateType(steam);
            BeerTypeService.CreateType(stout);
            BeerTypeService.CreateType(IPA);
            BeerTypeService.CreateType(pilsner);

            Brand ølværket = new Brand {ID = 1, BrandName = "Ølværket" };
            Brand esbjergBryhus = new Brand {ID = 2, BrandName = "Esbjerg Bryghus" };
            Brand carlsberg = new Brand {ID = 3, BrandName = "Carlsberg" };

            BrandService.CreateBrand(ølværket);
            BrandService.CreateBrand(esbjergBryhus);
            BrandService.CreateBrand(carlsberg);

            Beer famisbryg = new Beer
            {
                Name = "Famisbryg",
                Description = "Ravfarvet hvedeøl på 5.4% med lav bitterhed.Noter af hyldeblomst",
                Price = 65,
                Percentage = 5.4,
                IBU = 20,
                EBC = 8,
                ImageURL = "https://firebasestorage.googleapis.com/v0/b/eb-sdm3.appspot.com/o/NoImage.png?alt=media&token=d522375a-08c4-4ea9-99b0-30f23e3d52ed",
                Type = hvedeøl,
                Brand = ølværket
            };

            Beer cargo = new Beer
            {
                Name = "Cargo",
                Description = "Ravfarvet steam beer på 5.4% med middel bitterhed og noter af citrus",
                Price = 65,
                Percentage = 5.4,
                IBU = 40,
                EBC = 31,
                ImageURL = "https://firebasestorage.googleapis.com/v0/b/eb-sdm3.appspot.com/o/NoImage.png?alt=media&token=d522375a-08c4-4ea9-99b0-30f23e3d52ed",
                Type = steam,
                Brand = ølværket
            };

            Beer carlsbergHumle = new Beer
            {
                Name = "Carlsberg Humle",
                Description = "Carlsberg Humle er en ufiltreret pilsner med en delikat og ganske intens humlearoma fra det tilsatte humleolie, der udvindes af nyhøstet cascade humle. Øllen er gylden, let slørret og med en smuk vedholdende skumkrone.",
                Price = 10,
                Percentage = 4.5,
                IBU = 35,
                EBC = 10,
                ImageURL = "https://firebasestorage.googleapis.com/v0/b/eb-sdm3.appspot.com/o/NoImage.png?alt=media&token=d522375a-08c4-4ea9-99b0-30f23e3d52ed",
                Type = pilsner,
                Brand = carlsberg
            };

            BeerService.CreateBeer(famisbryg);
            BeerService.CreateBeer(cargo);
            BeerService.CreateBeer(carlsbergHumle);

            UserService.AddUser(UserService.CreateUser("SeglerHans", "kodeord", "Admin"));
            UserService.AddUser(UserService.CreateUser("Kutterjørgen", "lasagne28", "User"));
        }
    }
}
