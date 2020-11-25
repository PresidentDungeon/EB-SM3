using EB.Core.ApplicationServices;
using EB.Core.ApplicationServices.Impl;
using EB.Core.DomainServices;
using EB.Core.Entities;
using FluentAssertions;
using Moq;
using ProductShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace EB.UnitTests
{
    public class BeerServiceTest
    {
        private SortedDictionary<int, Beer> beerDatabase;
        private SortedDictionary<int, BeerType> typeDatabase;
        private SortedDictionary<int, Brand> brandDatabase;
        private Mock<IBeerRepository> repoMock;
        private Mock<IBeerTypeRepository> repoTypeMock;
        private Mock<IBrandRepository> repoBrandMock;
        private Mock<IValidator> validatorMock;

        public BeerServiceTest()
        {
            beerDatabase = new SortedDictionary<int, Beer>();
            typeDatabase = new SortedDictionary<int, BeerType>();
            brandDatabase = new SortedDictionary<int, Brand>();

            repoMock = new Mock<IBeerRepository>();
            repoTypeMock = new Mock<IBeerTypeRepository>();
            repoBrandMock = new Mock<IBrandRepository>();
            validatorMock = new Mock<IValidator>();

            repoMock.Setup(repo => repo.AddBeer(It.IsAny<Beer>())).Callback<Beer>(beer => beerDatabase.Add(beer.ID, beer));
            repoMock.Setup(repo => repo.UpdateBeerInRepo(It.IsAny<Beer>())).Callback<Beer>(beer => beerDatabase[beer.ID] = beer);
            repoMock.Setup(repo => repo.DeleteBeerInRepo(It.IsAny<int>())).Callback<int>(id => beerDatabase.Remove(id));
            repoMock.Setup(repo => repo.ReadBeers()).Returns(() => beerDatabase.Values);
            repoMock.Setup(repo => repo.ReadBeersFilterSearch(It.IsAny<Filter>())).Returns(() => { List<Beer> beer = beerDatabase.Values.ToList(); return new FilterList<Beer> { totalItems = beer.Count, List = beer }; });
            repoMock.Setup(repo => repo.ReadBeerById(It.IsAny<int>())).Returns<int>((id) => beerDatabase.ContainsKey(id) ? beerDatabase[id] : null);

            repoTypeMock.Setup(repo => repo.AddType(It.IsAny<BeerType>())).Callback<BeerType>(type => typeDatabase.Add(type.ID, type));
            repoTypeMock.Setup(repo => repo.ReadTypeById(It.IsAny<int>())).Returns<int>((id) => typeDatabase.ContainsKey(id) ? typeDatabase[id] : null);

            repoBrandMock.Setup(repo => repo.AddBrand(It.IsAny<Brand>())).Callback<Brand>(brand => brandDatabase.Add(brand.ID, brand));
            repoBrandMock.Setup(repo => repo.ReadBrandById(It.IsAny<int>())).Returns<int>((id) => brandDatabase.ContainsKey(id) ? brandDatabase[id] : null);
        }

        [Fact]
        public void CreateBeerService_RepositoriesAndValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            BeerService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BeerService(null as IBeerRepository, null as IBrandRepository, null as IBeerTypeRepository, null as IValidator));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateBeerService_BeerRepositoryIsNull_ExpectNullReferenceException()
        {
            // arrange
            BeerService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BeerService(null as IBeerRepository, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateBeerService_BrandRepositoryIsNull_ExpectNullReferenceException()
        {
            // arrange
            BeerService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BeerService(repoMock.Object, null as IBrandRepository, repoTypeMock.Object, validatorMock.Object));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateBeerService_BeerTypeRepositoryIsNull_ExpectNullReferenceException()
        {
            // arrange
            BeerService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BeerService(repoMock.Object, repoBrandMock.Object, null as IBeerTypeRepository, validatorMock.Object));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateBeerService_ValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            BeerService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, null as IValidator));

            Assert.Equal("Validator can't be null", ex.Message);
        }

        [Fact]
        public void CreateBeerService_ValidRepositoriesAndValidator()
        {
            // arrange
            BeerService service = null;

            // act + assert
            service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            Assert.NotNull(service);
        }

        [Fact]
        public void BeerService_ShouldBeOfTypeIBeerService()
        {
            // arrange

            // act + assert
            new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object).Should().BeAssignableTo<IBeerService>();
        }

        [Fact]
        public void BeerServiceValidate_ShouldValidateBeerWithBeerParameter_Once()
        {
            // arrange
            BeerService service = null;
            Beer beer = new Beer {ID = 1 };

            // act + assert
            service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);
            service.ValidateBeer(beer);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == beer)), Times.Once);
        }

        [Theory]
        [InlineData(1, "SKIPPER LAS", "Kobberfarvet", 56, 6.6, 2.2, 1.1)]
        [InlineData(2, "FIMUSBRYG", "Ravfarvet", 65, 5.1, 5.3, 2.1)]
        public void AddBeer_ValidBeer(int id, string name, string description, double price, double percentage, double IBU, double EBC)
        {
            // arrange

            BeerType type = new BeerType { ID = 1 };
            Brand brand = new Brand { ID = 2 };

            typeDatabase.Add(type.ID, type);
            brandDatabase.Add(brand.ID, brand);

            Beer beer = new Beer()
            {
                ID = id,
                Name = name,
                Description = description,
                Price = price,
                Percentage = percentage,
                IBU = IBU,
                EBC = EBC,
                Type = type,
                Brand = brand
            };

            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act
            service.CreateBeer(beer);

            // assert
            Assert.Contains(beer, beerDatabase.Values);
            repoMock.Verify(repo => repo.AddBeer(It.Is<Beer>(b => b == beer)), Times.Once);
            repoTypeMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Once);
            repoBrandMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == beer)), Times.Once);
        }

        [Theory]
        [InlineData(1, null, "Kobberfarvet", 56, 6.6, 2.2, 1.1, "Name is null exception")]
        [InlineData(2, "FIMUSBRYG", null, 65, 5.1, 5.3, 2.1, "Description is null exception")]
        public void AddBeer_InvalidBeer_ExceptArgumentException(int id, string name, string description, double price, double percentage, double IBU, double EBC, string errorExpected)
        {
            // arrange
            BeerType type = new BeerType { ID = 1 };
            Brand brand = new Brand { ID = 2 };

            typeDatabase.Add(type.ID, type);
            brandDatabase.Add(brand.ID, brand);

            Beer beer = new Beer()
            {
                ID = id,
                Name = name,
                Description = description,
                Price = price,
                Percentage = percentage,
                IBU = IBU,
                EBC = EBC,
                Type = type,
                Brand = brand
            };

            validatorMock.Setup(mock => mock.ValidateBeer(It.IsAny<Beer>())).Callback<Beer>(beer => throw new ArgumentException(errorExpected));
            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.CreateBeer(beer));

            // assert
            Assert.Equal(errorExpected, ex.Message);
            Assert.DoesNotContain(beer, beerDatabase.Values);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == beer)), Times.Once);
            repoMock.Verify(repo => repo.AddBeer(It.Is<Beer>(b => b == beer)), Times.Never);
            repoTypeMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Never);
            repoBrandMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Never);
        }

        [Fact]
        public void AddBeer_InvalidBeerTypeIsNotInDB_ExceptArgumentException()
        {
            // arrange
            BeerType type = new BeerType { ID = 1 };
            Brand brand = new Brand { ID = 2 };

            brandDatabase.Add(brand.ID, brand);

            Beer beer = new Beer()
            {
                ID = 1,
                Name = "Finus",
                Description = "Brown ale",
                Price = 65,
                Percentage = 4.8,
                IBU = 3,
                EBC = 15,
                Type = type,
                Brand = brand
            };

            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<InvalidOperationException>(() => service.CreateBeer(beer));

            // assert
            Assert.Equal("No type with such ID found", ex.Message);
            Assert.DoesNotContain(beer, beerDatabase.Values);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == beer)), Times.Once);
            repoTypeMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Once);
            repoBrandMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Never);
            repoMock.Verify(repo => repo.AddBeer(It.Is<Beer>(b => b == beer)), Times.Never);
        }

        [Fact]
        public void AddBeer_InvalidBeerBrandIsNotInDB_ExceptArgumentException()
        {
            // arrange
            BeerType type = new BeerType { ID = 1 };
            Brand brand = new Brand { ID = 2 };

            typeDatabase.Add(type.ID, type);

            Beer beer = new Beer()
            {
                ID = 1,
                Name = "Finus",
                Description = "Brown ale",
                Price = 65,
                Percentage = 4.8,
                IBU = 3,
                EBC = 15,
                Type = type,
                Brand = brand
            };

            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<InvalidOperationException>(() => service.CreateBeer(beer));

            // assert
            Assert.Equal("No brand with such ID found", ex.Message);
            Assert.DoesNotContain(beer, beerDatabase.Values);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == beer)), Times.Once);
            repoTypeMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Once);
            repoBrandMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Once);
            repoMock.Verify(repo => repo.AddBeer(It.Is<Beer>(b => b == beer)), Times.Never);
        }


        [Fact]
        public void GetBeerById_BeerExists()
        {
            // arrange

            Beer beer = new Beer{ ID = 1 };
            beerDatabase.Add(beer.ID, beer);

            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act
            var result = service.GetBeerById(beer.ID);

            // assert
            Assert.Equal(beer, result);
            repoMock.Verify(repo => repo.ReadBeerById(It.Is<int>(id => id == beer.ID)), Times.Once);
        }

        [Fact]
        public void GetBeerById_BeerDoesNotExist_ExpectNull()
        {
            // arrange
            var beer1 = new Beer{ ID = 1 };
            var beer2 = new Beer{ ID = 2 };

            beerDatabase.Add(beer2.ID, beer2);

            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act
            var result = service.GetBeerById(beer1.ID);

            // assert
            Assert.Null(result);
            repoMock.Verify(repo => repo.ReadBeerById(It.Is<int>(ID => ID == beer1.ID)), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetBeerById_InvalidId_ExpectArgumentException(int ID)
        {
            // arrange
            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.GetBeerById(ID));

            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repo => repo.ReadBeerById(It.Is<int>(id => id == ID)), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void GetAllBeers(int beerCount)
        {
            // arrange
            Beer beer1 = new Beer{ ID = 1 };
            Beer beer2 = new Beer{ ID = 2 };
            List<Beer> beers = new List<Beer>() { beer1, beer2 };

            var expected = beers.GetRange(0, beerCount);
            foreach (var beer in expected)
            {
                beerDatabase.Add(beer.ID, beer);
            }

            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act
            var result = service.GetAllBeer();

            // assert
            Assert.Equal(expected, result);
            repoMock.Verify(repo => repo.ReadBeers(), Times.Once);
        }

        [Theory]
        [InlineData(-1, 5)]
        [InlineData(2, -1)]
        public void GetBeerFilterSearch_InvalidPaging_ExpectInvalidDataException(int currentPage, int itemsPrPage)
        {
            // arrange
            Filter filter = new Filter {CurrentPage = currentPage, ItemsPrPage = itemsPrPage };
            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<InvalidDataException>(() => service.GetBeerFilterSearch(filter));
            Assert.Equal("Page or items per page must be above zero", ex.Message);

            // assert
            repoMock.Verify(repo => repo.ReadBeersFilterSearch(It.Is<Filter>(f => f == filter)), Times.Never);
        }

        [Fact]
        public void GetBeerFilterSearch_IndexOutOfBounds_ExpectInvalidDataException()
        {
            // arrange
            repoMock.Setup(repo => repo.ReadBeersFilterSearch(It.IsAny<Filter>())).Returns(() => throw new InvalidDataException("Index out of bounds"));

            var beer1 = new Beer { ID = 1 };
            var beer2 = new Beer { ID = 2 };

            beerDatabase.Add(beer1.ID, beer1);
            beerDatabase.Add(beer2.ID, beer2);

            Filter filter = new Filter { CurrentPage = 2, ItemsPrPage = 2 };
            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<InvalidDataException>(() => service.GetBeerFilterSearch(filter));

            // assert
            Assert.Equal("Index out of bounds", ex.Message);
            repoMock.Verify(repo => repo.ReadBeersFilterSearch(It.Is<Filter>(f => f == filter)), Times.Once);
        }

        [Fact]
        public void GetBeerFilterSearch_CorrectPaging()
        {
            // arrange
            var beer1 = new Beer { ID = 1 };
            var beer2 = new Beer { ID = 2 };

            beerDatabase.Add(beer1.ID, beer1);
            beerDatabase.Add(beer2.ID, beer2);

            var expected = new List<Beer>(){ beer1, beer2};
            int expectedSize = expected.Count;

            Filter filter = new Filter { CurrentPage = 1, ItemsPrPage = 2 };
            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act
            var result = service.GetBeerFilterSearch(filter);

            // assert
            Assert.Equal(expected, result.List);
            Assert.Equal(expectedSize, result.totalItems);
            repoMock.Verify(repo => repo.ReadBeersFilterSearch(It.Is<Filter>(f => f == filter)), Times.Once);
        }

        [Theory]
        [InlineData(1, "SKIPPER LAS", "Kobberfarvet", 56, 6.6, 2.2, 1.1)]
        [InlineData(2, "FIMUSBRYG", "Ravfarvet", 65, 5.1, 5.3, 2.1)]
        public void UpdateBeer_ValidExistingBeer(int id, string name, string description, double price, double percentage, double IBU, double EBC)
        {
            // arrange
            BeerType type = new BeerType { ID = 1 };
            Brand brand = new Brand { ID = 2 };

            typeDatabase.Add(type.ID, type);
            brandDatabase.Add(brand.ID, brand);

            Beer beer = new Beer()
            {
                ID = id,
                Name = name,
                Description = description,
                Price = price,
                Percentage = percentage,
                IBU = IBU,
                EBC = EBC,
                Type = type,
                Brand =brand
            };

            beerDatabase.Add(beer.ID, beer);

            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act
            beer.Name = "XXXmas";
            service.UpdateBeer(beer);

            // assert
            repoMock.Verify(repo => repo.ReadBeerById(It.Is<int>(ID => ID == beer.ID)), Times.Once);
            repoTypeMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Once);
            repoBrandMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Once);
            repoMock.Verify(repo => repo.UpdateBeerInRepo(It.Is<Beer>(b => b == beer)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == beer)), Times.Once);
            Assert.Equal(repoMock.Object.ReadBeerById(beer.ID), beer);
            Assert.NotEqual(repoMock.Object.ReadBeerById(beer.ID).Name, name);
        }

        [Theory]
        [InlineData(1, null, "Kobberfarvet", 56, 6.6, 2.2, 1.1, "Name is null exception")]
        [InlineData(2, "FIMUSBRYG", null, 65, 5.1, 5.3, 2.1, "Description is null exception")]
        public void UpdateBeer_InvalidBeer_ExpectArgumentException(int id, string name, string description, double price, double percentage, double IBU, double EBC, string errorExpected)
        {
            // arrange
            BeerType type = new BeerType { ID = 1 };
            Brand brand = new Brand { ID = 2 };

            typeDatabase.Add(type.ID, type);
            brandDatabase.Add(brand.ID, brand);

            Beer beer = new Beer()
            {
                ID = id,
                Name = name,
                Description = description,
                Price = price,
                Percentage = percentage,
                IBU = IBU,
                EBC = EBC,
                Type = type,
                Brand = brand
            };

            validatorMock.Setup(mock => mock.ValidateBeer(It.IsAny<Beer>())).Callback<Beer>(beer => throw new ArgumentException(errorExpected));
            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateBeer(beer));

            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == beer)), Times.Once);
            repoMock.Verify(repo => repo.ReadBeerById(It.Is<int>(ID => ID == beer.ID)), Times.Never);
            repoTypeMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Never);
            repoBrandMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Never);
            repoMock.Verify(repo => repo.UpdateBeerInRepo(It.Is<Beer>(b => b == beer)), Times.Never);

            Assert.Equal(errorExpected, ex.Message);
            Assert.Equal(repoMock.Object.ReadBeerById(beer.ID), null);
        }

        [Fact]
        public void UpdateBeer_BeerIsNUll_ExpectArgumentException()
        {
            // arrange
            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateBeer(null));

            Assert.Equal("Updating beer does not exist", ex.Message);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == null)), Times.Never);
            repoMock.Verify(repo => repo.UpdateBeerInRepo(It.Is<Beer>(b => b == null)), Times.Never);
        }

        [Fact]
        public void UpdateBeer_BeerDoesNotExist_InvalidOperationException()
        {
            // arrange
            BeerType type = new BeerType { ID = 1 };
            Brand brand = new Brand { ID = 2 };

            typeDatabase.Add(type.ID, type);
            brandDatabase.Add(brand.ID, brand);

            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            Beer beer = new Beer()
            {
                ID = 1,
                Name = "FimusBryg",
                Description = "Kobberfarvet",
                Price = 56,
                Percentage = 6.6,
                IBU = 2.2,
                EBC = 1.1
            };

            // act + assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.UpdateBeer(beer));

            Assert.Equal("No beer with such ID found", ex.Message);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == beer)), Times.Once);
            repoMock.Verify(repo => repo.ReadBeerById(It.Is<int>(ID => ID == beer.ID)), Times.Once);
            repoTypeMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Never);
            repoBrandMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Never);
            repoMock.Verify(repo => repo.UpdateBeerInRepo(It.Is<Beer>(b => b == beer)), Times.Never);
        }

        [Fact]
        public void UpdateBeer_BeerTypeDoesNotExist_ExceptArgumentException()
        {
            // arrange
            BeerType type = new BeerType { ID = 1 };
            Brand brand = new Brand { ID = 2 };

            brandDatabase.Add(brand.ID, brand);

            Beer beer = new Beer()
            {
                ID = 1,
                Name = "FimusBryg",
                Description = "Kobberfarvet",
                Price = 56,
                Percentage = 6.6,
                IBU = 2.2,
                EBC = 1.1,
                Type = type,
                Brand = brand
            };

            beerDatabase.Add(beer.ID, beer);

            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.UpdateBeer(beer));

            Assert.Equal("No type with such ID found", ex.Message);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == beer)), Times.Once);
            repoMock.Verify(repo => repo.ReadBeerById(It.Is<int>(ID => ID == beer.ID)), Times.Once);
            repoTypeMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Once);
            repoBrandMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Never);
            repoMock.Verify(repo => repo.UpdateBeerInRepo(It.Is<Beer>(b => b == beer)), Times.Never);
        }

        [Fact]
        public void UpdateBeer_BrandDoesNotExist_ExceptArgumentException()
        {
            // arrange
            BeerType type = new BeerType { ID = 1 };
            Brand brand = new Brand { ID = 2 };

            typeDatabase.Add(type.ID, type);

            Beer beer = new Beer()
            {
                ID = 1,
                Name = "FimusBryg",
                Description = "Kobberfarvet",
                Price = 56,
                Percentage = 6.6,
                IBU = 2.2,
                EBC = 1.1,
                Type = type,
                Brand = brand
            };

            beerDatabase.Add(beer.ID, beer);

            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.UpdateBeer(beer));

            Assert.Equal("No brand with such ID found", ex.Message);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == beer)), Times.Once);
            repoMock.Verify(repo => repo.ReadBeerById(It.Is<int>(ID => ID == beer.ID)), Times.Once);
            repoTypeMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Once);
            repoBrandMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Once);
            repoMock.Verify(repo => repo.UpdateBeerInRepo(It.Is<Beer>(b => b == beer)), Times.Never);
        }

        [Fact]
        public void RemoveBeer_ValidExistingBeer()
        {
            // arrange
            Beer beer = new Beer()
            {
                ID = 1,
                Name = "FimusBryg",
                Description = "Kobberfarvet",
                Price = 56,
                Percentage = 6.6,
                IBU = 2.2,
                EBC = 1.1
            };

            beerDatabase.Add(beer.ID, beer);

            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act
            service.DeleteBeer(beer.ID);

            // assert
            repoMock.Verify(repo => repo.ReadBeerById(It.Is<int>(ID => ID == beer.ID)), Times.Once);
            repoMock.Verify(repo => repo.DeleteBeerInRepo(It.Is<int>(ID => ID == beer.ID)), Times.Once);
            Assert.Null(repoMock.Object.ReadBeerById(beer.ID));
        }

        [Fact]
        public void RemoveBeer_BeerDoesNotExist_ExpectArgumentException()
        {
            // arrange

            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            Beer beer = new Beer()
            {
                ID = 1,
                Name = "FimusBryg",
                Description = "Kobberfarvet",
                Price = 56,
                Percentage = 6.6,
                IBU = 2.2,
                EBC = 1.1
            };

            // act + assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.DeleteBeer(beer.ID));

            // assert
            Assert.Equal("No beer with such ID found", ex.Message);
            repoMock.Verify(repo => repo.ReadBeerById(It.Is<int>(ID => ID == beer.ID)), Times.Once);
            repoMock.Verify(repo => repo.DeleteBeerInRepo(It.Is<int>(ID => ID == beer.ID)), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void RemoveBeer_IncorrectID_ExpectArgumentException(int ID)
        {
            // arrange
            BeerService service = new BeerService(repoMock.Object, repoBrandMock.Object, repoTypeMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.DeleteBeer(ID));

            // assert
            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repo => repo.DeleteBeerInRepo(It.Is<int>(id => id == ID)), Times.Never);
            repoMock.Verify(repo => repo.ReadBeerById(It.Is<int>(id => id == ID)), Times.Never);
        }
    }
}
