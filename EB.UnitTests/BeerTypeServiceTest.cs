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
    public class BeerTypeServiceTest
    {
        private SortedDictionary<int, BeerType> typeDatabase;
        private Mock<IBeerTypeRepository> repoMock;
        private Mock<IValidator> validatorMock;

        public BeerTypeServiceTest()
        {
            typeDatabase = new SortedDictionary<int, BeerType>();
            repoMock = new Mock<IBeerTypeRepository>();
            validatorMock = new Mock<IValidator>();
            repoMock.Setup(repo => repo.AddType(It.IsAny<BeerType>())).Callback<BeerType>(type => typeDatabase.Add(type.ID, type));
            repoMock.Setup(repo => repo.UpdateTypeInRepo(It.IsAny<BeerType>())).Callback<BeerType>(type => typeDatabase[type.ID] = type);
            repoMock.Setup(repo => repo.DeleteTypeInRepo(It.IsAny<int>())).Callback<int>(id => typeDatabase.Remove(id));
            repoMock.Setup(repo => repo.ReadTypes()).Returns(() => typeDatabase.Values);
            repoMock.Setup(repo => repo.ReadTypesFilterSearch(It.IsAny<Filter>())).Returns(() => { List<BeerType> type = typeDatabase.Values.ToList(); return new FilterList<BeerType> { totalItems = type.Count, List = type }; });
            repoMock.Setup(repo => repo.ReadTypeById(It.IsAny<int>())).Returns<int>((id) => typeDatabase.ContainsKey(id) ? typeDatabase[id] : null);
        }

        [Fact]
        public void CreateBeerTypeService_BeerTypeRepositoryAndValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            BeerTypeService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BeerTypeService(null as IBeerTypeRepository, null as IValidator));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateBeerTypeService_BeerTypeRepositoryIsNull_ExpectNullReferenceException()
        {
            // arrange
            BeerTypeService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BeerTypeService(null as IBeerTypeRepository, validatorMock.Object));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateBeerTypeService_ValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            BeerTypeService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BeerTypeService(repoMock.Object, null as IValidator));

            Assert.Equal("Validator can't be null", ex.Message);
        }

        [Fact]
        public void CreateBeerTypeService_ValidBeerTypeRepositoryAndValidator()
        {
            // arrange
            BeerTypeService service = null;

            // act + assert
            service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            Assert.NotNull(service);
        }

        [Fact]
        public void BeerTypeService_ShouldBeOfTypeIBeerTypeService()
        {
            // arrange

            // act + assert
            new BeerTypeService(repoMock.Object, validatorMock.Object).Should().BeAssignableTo<IBeerTypeService>();
        }

        [Fact]
        public void BeerTypeServiceValidate_ShouldValidateBeerTypeWithBeerTypeParameter_Once()
        {
            // arrange
            BeerTypeService service = null;
            BeerType type = new BeerType { ID = 1 };

            // act + assert
            service = new BeerTypeService(repoMock.Object, validatorMock.Object);
            service.ValidateType(type);
            validatorMock.Verify(validator => validator.ValidateType(It.Is<BeerType>(t => t == type)), Times.Once);
        }

        [Theory]
        [InlineData(1, "IPA")]
        [InlineData(2, "Stout")]
        public void AddBeerType_ValidBeerType(int id, string typeName)
        {
            // arrange
            BeerType type = new BeerType()
            {
                ID = id,
                TypeName = typeName
            };

            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act
            service.CreateType(type);

            // assert
            Assert.Contains(type, typeDatabase.Values);
            repoMock.Verify(repo => repo.AddType(It.Is<BeerType>(t => t == type)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateType(It.Is<BeerType>(t => t == type)), Times.Once);
        }

        [Theory]
        [InlineData(1, null, "Name is null exception")]
        [InlineData(2, "", "Name is null exception")]
        public void AddBeerType_InvalidBeerType(int id, string typeName, string errorExpected)
        {
            // arrange
            BeerType type = new BeerType()
            {
                ID = id,
                TypeName = typeName
            };

            validatorMock.Setup(mock => mock.ValidateType(It.IsAny<BeerType>())).Callback<BeerType>(type => throw new ArgumentException(errorExpected));
            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.CreateType(type));

            // assert
            Assert.Equal(errorExpected, ex.Message);
            Assert.DoesNotContain(type, typeDatabase.Values);
            repoMock.Verify(repo => repo.AddType(It.Is<BeerType>(t => t == type)), Times.Never);
            validatorMock.Verify(validator => validator.ValidateType(It.Is<BeerType>(t => t == type)), Times.Once);
        }

        [Fact]
        public void GetBeerTypeById_BeerTypeExists()
        {
            // arrange
            BeerType type = new BeerType { ID = 1 };
            typeDatabase.Add(type.ID, type);

            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetTypeById(type.ID);

            // assert
            Assert.Equal(type, result);
            repoMock.Verify(repo => repo.ReadTypeById(It.Is<int>(id => id == type.ID)), Times.Once);
        }

        [Fact]
        public void GetBeerTypeById_BeerTypeDoesNotExist_ExpectNull()
        {
            // arrange
            var beerType1 = new BeerType { ID = 1 };
            var beerType2 = new BeerType { ID = 2 };

            typeDatabase.Add(beerType2.ID, beerType2);

            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetTypeById(beerType1.ID);

            // assert
            Assert.Null(result);
            repoMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == beerType1.ID)), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetBeerTypeById_InvalidId_ExpectArgumentException(int ID)
        {
            // arrange
            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.GetTypeById(ID));

            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repo => repo.ReadTypeById(It.Is<int>(id => id == ID)), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void GetAllTypes(int beerTypeCount)
        {
            // arrange
            BeerType beerType1 = new BeerType { ID = 1 };
            BeerType beerType2 = new BeerType { ID = 2 };
            List<BeerType> types = new List<BeerType>() { beerType1, beerType2 };

            var expected = types.GetRange(0, beerTypeCount);
            foreach (var type in expected)
            {
                typeDatabase.Add(type.ID, type);
            }

            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetAllTypes();

            // assert
            Assert.Equal(expected, result);
            repoMock.Verify(repo => repo.ReadTypes(), Times.Once);
        }

        [Theory]
        [InlineData(-1, 5)]
        [InlineData(2, -1)]
        public void GetBeerTypesFilterSearch_InvalidPaging_ExpectInvalidDataException(int currentPage, int itemsPrPage)
        {
            // arrange
            Filter filter = new Filter { CurrentPage = currentPage, ItemsPrPage = itemsPrPage };
            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<InvalidDataException>(() => service.GetTypesFilterSearch(filter));
            Assert.Equal("Page or items per page must be above zero", ex.Message);

            // assert
            repoMock.Verify(repo => repo.ReadTypesFilterSearch(It.Is<Filter>(f => f == filter)), Times.Never);
        }

        [Fact]
        public void GetBeerTypesFilterSearch_IndexOutOfBounds_ExpectInvalidDataException()
        {
            // arrange
            repoMock.Setup(repo => repo.ReadTypesFilterSearch(It.IsAny<Filter>())).Returns(() => throw new InvalidDataException("Index out of bounds"));

            BeerType beerType1 = new BeerType { ID = 1 };
            BeerType beerType2 = new BeerType { ID = 2 };

            typeDatabase.Add(beerType1.ID, beerType1);
            typeDatabase.Add(beerType2.ID, beerType2);

            Filter filter = new Filter { CurrentPage = 2, ItemsPrPage = 2 };
            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<InvalidDataException>(() => service.GetTypesFilterSearch(filter));

            // assert
            Assert.Equal("Index out of bounds", ex.Message);
            repoMock.Verify(repo => repo.ReadTypesFilterSearch(It.Is<Filter>(f => f == filter)), Times.Once);
        }

        [Fact]
        public void GetBeerTypesFilterSearch_CorrectPaging()
        {
            // arrange
            BeerType beerType1 = new BeerType { ID = 1 };
            BeerType beerType2 = new BeerType { ID = 2 };

            typeDatabase.Add(beerType1.ID, beerType1);
            typeDatabase.Add(beerType2.ID, beerType2);

            var expected = new List<BeerType>() { beerType1, beerType2 };
            int expectedSize = expected.Count;

            Filter filter = new Filter { CurrentPage = 1, ItemsPrPage = 2 };
            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act
            var result = service.GetTypesFilterSearch(filter);

            // assert
            Assert.Equal(expected, result.List);
            Assert.Equal(expectedSize, result.totalItems);
            repoMock.Verify(repo => repo.ReadTypesFilterSearch(It.Is<Filter>(f => f == filter)), Times.Once);
        }

        [Theory]
        [InlineData(1, "IPA")]
        [InlineData(2, "Stout")]
        public void UpdateBeerType_ValidExistingBeerType(int id, string typeName)
        {
            // arrange
            BeerType type = new BeerType()
            {
                ID = id,
                TypeName = typeName
            };

            typeDatabase.Add(type.ID, type);

            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act
            service.UpdateType(type);

            // assert
            repoMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Once);
            repoMock.Verify(repo => repo.UpdateTypeInRepo(It.Is<BeerType>(t => t == type)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateType(It.Is<BeerType>(t => t == type)), Times.Once);
            Assert.Equal(repoMock.Object.ReadTypeById(type.ID), type);
        }

        [Theory]
        [InlineData(1, null, "Name is null exception")]
        [InlineData(2, "", "Name is null exception")]
        public void UpdateBeerType_InvalidBeerType_ExpectArgumentException(int id, string typeName, string errorExpected)
        {
            // arrange
            BeerType type = new BeerType()
            {
                ID = id,
                TypeName = typeName
            };

            validatorMock.Setup(mock => mock.ValidateType(It.IsAny<BeerType>())).Callback<BeerType>(type => throw new ArgumentException(errorExpected));
            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateType(type));

            Assert.Equal(errorExpected, ex.Message);
            Assert.Equal(repoMock.Object.ReadTypeById(type.ID), null);

            repoMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateType(It.Is<BeerType>(t => t == type)), Times.Once);
            repoMock.Verify(repo => repo.UpdateTypeInRepo(It.Is<BeerType>(t => t == type)), Times.Never);
        }

        [Fact]
        public void UpdateBeerType_BeerTypeIsNUll_ExpectArgumentException()
        {
            // arrange
            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateType(null));

            Assert.Equal("Updating type does not exist", ex.Message);
            validatorMock.Verify(validator => validator.ValidateType(It.Is<BeerType>(t => t == null)), Times.Never);
            repoMock.Verify(repo => repo.UpdateTypeInRepo(It.Is<BeerType>(t => t == null)), Times.Never);
        }

        [Fact]
        public void UpdateBeerType_BeerTypeDoesNotExist_InvalidOperationException()
        {
            // arrange
            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            BeerType type = new BeerType()
            {
                ID = 1,
                TypeName = "IPA"
            };

            // act + assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.UpdateType(type));

            Assert.Equal("No type with such ID found", ex.Message);
            repoMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateType(It.Is<BeerType>(t => t == type)), Times.Once);
            repoMock.Verify(repo => repo.UpdateTypeInRepo(It.Is<BeerType>(t => t == type)), Times.Never);
        }

        [Fact]
        public void RemoveBeerType_ValidExistingBeerType()
        {
            // arrange
            BeerType type = new BeerType()
            {
                ID = 1,
                TypeName = "Stout"
            };

            typeDatabase.Add(type.ID, type);

            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act
            service.DeleteType(type.ID);

            // assert
            repoMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Once);
            repoMock.Verify(repo => repo.DeleteTypeInRepo(It.Is<int>(ID => ID == type.ID)), Times.Once);
            Assert.Null(repoMock.Object.ReadTypeById(type.ID));
        }

        [Fact]
        public void RemoveBeerType_BeerTypeDoesNotExist_ExpectArgumentException()
        {
            // arrange

            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            BeerType type = new BeerType()
            {
                ID = 1,
                TypeName = "IPA"
            };

            // act + assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.DeleteType(type.ID));

            // assert
            Assert.Equal("No type with such ID found", ex.Message);
            repoMock.Verify(repo => repo.ReadTypeById(It.Is<int>(ID => ID == type.ID)), Times.Once);
            repoMock.Verify(repo => repo.DeleteTypeInRepo(It.Is<int>(ID => ID == type.ID)), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void RemoveBeerType_IncorrectID_ExpectArgumentException(int ID)
        {
            // arrange
            BeerTypeService service = new BeerTypeService(repoMock.Object, validatorMock.Object);

            // act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.DeleteType(ID));

            // assert
            Assert.Equal("Incorrect ID entered", ex.Message);
            repoMock.Verify(repo => repo.DeleteTypeInRepo(It.Is<int>(id => id == ID)), Times.Never);
            repoMock.Verify(repo => repo.ReadTypeById(It.Is<int>(id => id == ID)), Times.Never);
        }
    }
}
