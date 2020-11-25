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
    public class BrandServiceTest
    {
        private SortedDictionary<int, Brand> brandDatabase;
        private Mock<IBrandRepository> repoMock;
        private Mock<IValidator> validatorMock;

        public BrandServiceTest()
        {
            brandDatabase = new SortedDictionary<int, Brand>();
            repoMock = new Mock<IBrandRepository>();
            validatorMock = new Mock<IValidator>();
            repoMock.Setup(repo => repo.AddBrand(It.IsAny<Brand>())).Callback<Brand>(brand => brandDatabase.Add(brand.ID, brand));
            repoMock.Setup(repo => repo.UpdateBrandInRepo(It.IsAny<Brand>())).Callback<Brand>(brand => brandDatabase[brand.ID] = brand);
            repoMock.Setup(repo => repo.DeleteBrandInRepo(It.IsAny<int>())).Callback<int>(id => brandDatabase.Remove(id));
            repoMock.Setup(repo => repo.ReadBrands()).Returns(() => brandDatabase.Values);
            repoMock.Setup(repo => repo.ReadBrandsFilterSearch(It.IsAny<Filter>())).Returns(() => { List<Brand> brand = brandDatabase.Values.ToList(); return new FilterList<Brand> { totalItems = brand.Count, List = brand }; });
            repoMock.Setup(repo => repo.ReadBrandById(It.IsAny<int>())).Returns<int>((id) => brandDatabase.ContainsKey(id) ? brandDatabase[id] : null);
        }

        [Fact]
        public void CreateBrandService_BeerRepositoryAndValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            BrandService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BrandService(null as IBrandRepository, null as IValidator));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateBrandService_BrandRepositoryIsNull_ExpectNullReferenceException()
        {
            // arrange
            BrandService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BrandService(null as IBrandRepository, validatorMock.Object));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateBrandService_ValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            BrandService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BrandService(repoMock.Object, null as IValidator));

            Assert.Equal("Validator can't be null", ex.Message);
        }

        [Fact]
        public void CreateBrandService_ValidBrandRepositoryAndValidator()
        {
            // arrange
            BrandService service = null;

            // act + assert
            service = new BrandService(repoMock.Object, validatorMock.Object);

            Assert.NotNull(service);
        }

        [Fact]
        public void BrandService_ShouldBeOfTypeIBrandService()
        {
            // arrange

            // act + assert
            new BrandService(repoMock.Object, validatorMock.Object).Should().BeAssignableTo<IBrandService>();
        }

        [Fact]
        public void BrandServiceValidate_ShouldValidateBrandWithBrandParameter_Once()
        {
            // arrange
            BrandService service = null;
            Brand brand = new Brand { ID = 1 };

            // act + assert
            service = new BrandService(repoMock.Object, validatorMock.Object);
            service.ValidateBrand(brand);
            validatorMock.Verify(validator => validator.ValidateBrand(It.Is<Brand>(b => b == brand)), Times.Once);
        }

        [Theory]
        [InlineData(1, "Ølværket")]
        [InlineData(2, "Esbjerg Bryghus")]
        public void AddBrand_ValidBrand(int id, string brandName)
        {
            // arrange
            Brand brand = new Brand()
            {
                ID = id,
                BrandName = brandName
            };

            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act
            service.CreateBrand(brand);

            // assert
            Assert.Contains(brand, brandDatabase.Values);
            repoMock.Verify(repo => repo.AddBrand(It.Is<Brand>(b => b == brand)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateBrand(It.Is<Brand>(b => b == brand)), Times.Once);
        }

        [Theory]
        [InlineData(1, null, "Name is null exception")]
        [InlineData(2, "", "Description is null exception")]
        public void AddBrand_InvalidBrand(int id, string brandName, string errorExpected)
        {
            // arrange
            Brand brand = new Brand()
            {
                ID = id,
                BrandName = brandName
            };

            validatorMock.Setup(mock => mock.ValidateBrand(It.IsAny<Brand>())).Callback<Brand>(brand => throw new ArgumentException(errorExpected));
            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.CreateBrand(brand));

            // assert
            Assert.Equal(errorExpected, ex.Message);
            Assert.DoesNotContain(brand, brandDatabase.Values);
            repoMock.Verify(repo => repo.AddBrand(It.Is<Brand>(b => b == brand)), Times.Never);
            validatorMock.Verify(validator => validator.ValidateBrand(It.Is<Brand>(b => b == brand)), Times.Once);
        }

        [Fact]
        public void GetBrandById_BrandExists()
        {
            // arrange

            Brand brand = new Brand { ID = 1 };
            brandDatabase.Add(brand.ID, brand);

            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetBrandById(brand.ID);

            // assert
            Assert.Equal(brand, result);
            repoMock.Verify(repo => repo.ReadBrandById(It.Is<int>(id => id == brand.ID)), Times.Once);
        }

        [Fact]
        public void GetBrandById_BrandDoesNotExist_ExpectNull()
        {
            // arrange
            var brand1 = new Brand { ID = 1 };
            var brand2 = new Brand { ID = 2 };

            brandDatabase.Add(brand2.ID, brand2);

            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetBrandById(brand1.ID);

            // assert
            Assert.Null(result);
            repoMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand1.ID)), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetBrandById_InvalidId_ExpectArgumentException(int ID)
        {
            // arrange
            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.GetBrandById(ID));

            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repo => repo.ReadBrandById(It.Is<int>(id => id == ID)), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void GetAllBrands(int brandCount)
        {
            // arrange
            Brand brand1 = new Brand { ID = 1 };
            Brand brand2 = new Brand { ID = 2 };
            List<Brand> brands = new List<Brand>() { brand1, brand2 };

            var expected = brands.GetRange(0, brandCount);
            foreach (var brand in expected)
            {
                brandDatabase.Add(brand.ID, brand);
            }

            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetAllBrands();

            // assert
            Assert.Equal(expected, result);
            repoMock.Verify(repo => repo.ReadBrands(), Times.Once);
        }

        [Theory]
        [InlineData(-1, 5)]
        [InlineData(2, -1)]
        public void GetBrandFilterSearch_InvalidPaging_ExpectInvalidDataException(int currentPage, int itemsPrPage)
        {
            // arrange
            Filter filter = new Filter { CurrentPage = currentPage, ItemsPrPage = itemsPrPage };
            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<InvalidDataException>(() => service.GetBrandFilterSearch(filter));
            Assert.Equal("Page or items per page must be above zero", ex.Message);

            // assert
            repoMock.Verify(repo => repo.ReadBrandsFilterSearch(It.Is<Filter>(f => f == filter)), Times.Never);
        }

        [Fact]
        public void GetBrandFilterSearch_IndexOutOfBounds_ExpectInvalidDataException()
        {
            // arrange
            repoMock.Setup(repo => repo.ReadBrandsFilterSearch(It.IsAny<Filter>())).Returns(() => throw new InvalidDataException("Index out of bounds"));

            var brand1 = new Brand { ID = 1 };
            var brand2 = new Brand { ID = 2 };

            brandDatabase.Add(brand1.ID, brand1);
            brandDatabase.Add(brand2.ID, brand2);

            Filter filter = new Filter { CurrentPage = 2, ItemsPrPage = 2 };
            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<InvalidDataException>(() => service.GetBrandFilterSearch(filter));

            // assert
            Assert.Equal("Index out of bounds", ex.Message);
            repoMock.Verify(repo => repo.ReadBrandsFilterSearch(It.Is<Filter>(f => f == filter)), Times.Once);
        }

        [Fact]
        public void GetBrandFilterSearch_CorrectPaging()
        {
            // arrange
            var brand1 = new Brand { ID = 1 };
            var brand2 = new Brand { ID = 2 };

            brandDatabase.Add(brand1.ID, brand1);
            brandDatabase.Add(brand2.ID, brand2);

            var expected = new List<Brand>() { brand1, brand2 };
            int expectedSize = expected.Count;

            Filter filter = new Filter { CurrentPage = 1, ItemsPrPage = 2 };
            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetBrandFilterSearch(filter);

            // assert
            Assert.Equal(expected, result.List);
            Assert.Equal(expectedSize, result.totalItems);
            repoMock.Verify(repo => repo.ReadBrandsFilterSearch(It.Is<Filter>(f => f == filter)), Times.Once);
        }

        [Theory]
        [InlineData(1, "Into Waves")]
        [InlineData(2, "Esbjerg Bryghus")]
        public void UpdateBrand_ValidExistingBrand(int id, string brandName)
        {
            // arrange
            Brand brand = new Brand()
            {
                ID = id,
                BrandName = brandName,
            };

            brandDatabase.Add(brand.ID, brand);

            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act
            service.UpdateBrand(brand);

            // assert
            repoMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Once);
            repoMock.Verify(repo => repo.UpdateBrandInRepo(It.Is<Brand>(b => b == brand)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateBrand(It.Is<Brand>(b => b == brand)), Times.Once);
            Assert.Equal(repoMock.Object.ReadBrandById(brand.ID), brand);
        }

        [Theory]
        [InlineData(1, null, "Name is null exception")]
        [InlineData(2, "", "Description is null exception")]
        public void UpdateBrand_InvalidBrand_ExpectArgumentException(int id, string brandName, string errorExpected)
        {
            // arrange
            Brand brand = new Brand()
            {
                ID = id,
                BrandName = brandName
            };

            validatorMock.Setup(mock => mock.ValidateBrand(It.IsAny<Brand>())).Callback<Brand>(brand => throw new ArgumentException(errorExpected));
            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateBrand(brand));

            Assert.Equal(errorExpected, ex.Message);
            Assert.Equal(repoMock.Object.ReadBrandById(brand.ID), null);

            repoMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateBrand(It.Is<Brand>(b => b == brand)), Times.Once);
            repoMock.Verify(repo => repo.UpdateBrandInRepo(It.Is<Brand>(b => b == brand)), Times.Never);
        }

        [Fact]
        public void UpdateBrand_BrandIsNUll_ExpectArgumentException()
        {
            // arrange
            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateBrand(null));

            Assert.Equal("Updating brand does not exist", ex.Message);
            validatorMock.Verify(validator => validator.ValidateBrand(It.Is<Brand>(b => b == null)), Times.Never);
            repoMock.Verify(repo => repo.UpdateBrandInRepo(It.Is<Brand>(b => b == null)), Times.Never);
        }

        [Fact]
        public void UpdateBrand_BrandDoesNotExist_InvalidOperationException()
        {
            // arrange
            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            Brand brand = new Brand()
            {
                ID = 1,
                BrandName = "Ølværket",
            };

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateBrand(brand));

            Assert.Equal("No brand with such ID found", ex.Message);
            repoMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateBrand(It.Is<Brand>(b => b == brand)), Times.Once);
            repoMock.Verify(repo => repo.UpdateBrandInRepo(It.Is<Brand>(b => b == brand)), Times.Never);
        }

        [Fact]
        public void RemoveBrand_ValidExistingBrand()
        {
            // arrange
            Brand brand = new Brand()
            {
                ID = 1,
                BrandName = "Ølværket"
            };

            brandDatabase.Add(brand.ID, brand);

            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act
            service.DeleteBrand(brand.ID);

            // assert
            repoMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Once);
            repoMock.Verify(repo => repo.DeleteBrandInRepo(It.Is<int>(ID => ID == brand.ID)), Times.Once);
            Assert.Null(repoMock.Object.ReadBrandById(brand.ID));
        }

        [Fact]
        public void RemoveBrand_BrandDoesNotExist_ExpectArgumentException()
        {
            // arrange

            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            Brand brand = new Brand()
            {
                ID = 1,
                BrandName = "Ølværket"
            };

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.DeleteBrand(brand.ID));

            // assert
            Assert.Equal("No brand with such ID found", ex.Message);
            repoMock.Verify(repo => repo.ReadBrandById(It.Is<int>(ID => ID == brand.ID)), Times.Once);
            repoMock.Verify(repo => repo.DeleteBrandInRepo(It.Is<int>(ID => ID == brand.ID)), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void RemoveBrand_IncorrectID_ExpectArgumentException(int ID)
        {
            // arrange
            BrandService service = new BrandService(repoMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.DeleteBrand(ID));

            // assert
            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repo => repo.DeleteBrandInRepo(It.Is<int>(id => id == ID)), Times.Never);
            repoMock.Verify(repo => repo.ReadBrandById(It.Is<int>(id => id == ID)), Times.Never);
        }
    }
}
