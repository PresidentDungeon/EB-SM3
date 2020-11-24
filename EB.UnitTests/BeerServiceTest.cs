using EB.Core.ApplicationServices;
using EB.Core.ApplicationServices.Impl;
using EB.Core.DomainServices;
using EB.Core.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace EB.UnitTests
{
    public class BeerServiceTest
    {
        private SortedDictionary<int, Beer> beerDatabase;
        private Mock<IBeerRepository> repoMock;

        public BeerServiceTest() 
        {
            beerDatabase = new SortedDictionary<int, Beer>();
            repoMock = new Mock<IBeerRepository>();
            repoMock.Setup(repo => repo.AddBeer(It.IsAny<Beer>())).Callback<Beer>(beer => beerDatabase.Add(beer.ID, beer));
            repoMock.Setup(repo => repo.UpdateBeerInRepo(It.IsAny<Beer>())).Callback<Beer>(beer => beerDatabase[beer.ID] = beer);
            repoMock.Setup(repo => repo.DeleteBeerInRepo(It.IsAny<int>())).Callback<int>(id => beerDatabase.Remove(id));
            repoMock.Setup(repo => repo.ReadAllBeer(new Filter { })).Returns(() => beerDatabase.Values);
            repoMock.Setup(repo => repo.ReadBeerById(It.IsAny<int>())).Returns<int>((id) => beerDatabase.ContainsKey(id) ? beerDatabase[id] : null);
        }

        [Fact]
        public void CreateBeerService_BeerRepositoryAndValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            BeerService service = null;

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BeerService(null as IBeerRepository, null as IValidator));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateBeerService_BeerRepositoryIsNull_ExpectNullReferenceException()
        {
            // arrange
            BeerService service = null;
            Mock<IValidator> validatorMock = new Mock<IValidator>();

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BeerService(null as IBeerRepository, validatorMock.Object));

            Assert.Equal("Repository can't be null", ex.Message);
        }

        [Fact]
        public void CreateBeerService_ValidatorIsNull_ExpectNullReferenceException()
        {
            // arrange
            BeerService service = null;
            Mock<IBeerRepository> beerRepositoryMock = new Mock<IBeerRepository>();

            // act + assert
            var ex = Assert.Throws<NullReferenceException>(() => service = new BeerService(beerRepositoryMock.Object, null as IValidator));

            Assert.Equal("Validator can't be null", ex.Message);
        }

        [Fact]
        public void CreateBeerService_ValidBeerRepositoryAndValidator()
        {
            // arrange
            BeerService service = null;
            Mock<IBeerRepository> beerRepositoryMock = new Mock<IBeerRepository>();
            Mock<IValidator> validatorMock = new Mock<IValidator>();

            // act + assert
            service = new BeerService(beerRepositoryMock.Object, validatorMock.Object);

            Assert.NotNull(service);
        }

        [Fact]
        public void BeerService_ShouldBeOfTypeIBeerService()
        {
            // arrange
            Mock<IBeerRepository> beerRepositoryMock = new Mock<IBeerRepository>();
            Mock<IValidator> validatorMock = new Mock<IValidator>();

            // act + assert
            new BeerService(beerRepositoryMock.Object, validatorMock.Object).Should().BeAssignableTo<IBeerService>();
        }
 
        [Fact]
        public void BeerServiceValidate_ShouldValidateBeerWithBeerParameter_Once()
        {
            // arrange
            BeerService service = null;
            Mock<IBeerRepository> beerRepositoryMock = new Mock<IBeerRepository>();
            Mock<IValidator> validatorMock = new Mock<IValidator>();
            Beer beer = new Beer { };

            // act + assert
            service = new BeerService(beerRepositoryMock.Object, validatorMock.Object);
            service.ValidateBeer(beer);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer> (b => b == beer)), Times.Once);
        }

        [Theory]
        [InlineData(1, "SKIPPER LAS", "Kobberfarvet", 56, 6.6, 2.2, 1.1)]
        [InlineData(2, "FIMUSBRYG", "Ravfarvet", 65, 5.1, 5.3, 2.1)]
        public void AddBeer_ValidBeer(int id, string name, string description, double price, double percentage, double IBU, double EBC)
        {
            // arrange
            var beer = new Beer()
            {
                ID = id,
                Name = name,
                Description = description,
                Price = price,
                Percentage = percentage,
                IBU = IBU,
                EBC = EBC
            };

            Mock<IValidator> validatorMock = new Mock<IValidator>();
            BeerService service = new BeerService(repoMock.Object, validatorMock.Object);

            // act
            service.CreateBeer(beer);

            // assert
            Assert.Contains(beer, beerDatabase.Values);
            repoMock.Verify(repo => repo.AddBeer(It.Is<Beer>(b => b == beer)), Times.Once);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == beer)), Times.Once);
        }

        [Theory]
        [InlineData(1, null, "Kobberfarvet", 56, 6.6, 2.2, 1.1, "Name is null exception")]
        [InlineData(2, "FIMUSBRYG", null, 65, 5.1, 5.3, 2.1, "Description is null exception")]
        public void AddBeer_InvalidBeer(int id, string name, string description, double price, double percentage, double IBU, double EBC, string errorExpected)
        {
            // arrange
            var beer = new Beer()
            {
                ID = id,
                Name = name,
                Description = description,
                Price = price,
                Percentage = percentage,
                IBU = IBU,
                EBC = EBC
            };

            Mock<IValidator> validatorMock = new Mock<IValidator>();
            validatorMock.Setup(mock => mock.ValidateBeer(It.IsAny<Beer>())).Callback<Beer>(beer => throw new ArgumentException(errorExpected));
            BeerService service = new BeerService(repoMock.Object, validatorMock.Object);

            // act
            var ex = Assert.Throws<ArgumentException>(() => service.CreateBeer(beer));

            // assert
            Assert.Equal(errorExpected, ex.Message);
            Assert.DoesNotContain(beer, beerDatabase.Values);
            repoMock.Verify(repo => repo.AddBeer(It.Is<Beer>(b => b == beer)), Times.Never);
            validatorMock.Verify(validator => validator.ValidateBeer(It.Is<Beer>(b => b == beer)), Times.Once);
        }


    }
}
