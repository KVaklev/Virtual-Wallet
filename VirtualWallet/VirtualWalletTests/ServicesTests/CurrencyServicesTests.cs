using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Services.Helpers;
using Business.Services.Models;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Moq;
using static VirtualWalletTests.TestHelpers.TestHelpers;
namespace VirtualWalletTests.ServicesTests
{
    [TestClass]
    public class CurrencyServicesTests
    {
        [TestMethod]
        public async Task CreateAsync_AdminUserAndUniqueCurrency_ReturnsSuccessfulResponseWithData()
        {
            // Arrange
            var currencyDto = GetCurrencyDto();
            var loggedUser = GetTestUserAdmin();
            var currency = GetNewCurrency();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<Currency>(currencyDto)).Returns(currency);

            var service = new CurrencyService(currencyRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await service.CreateAsync(currencyDto, loggedUser);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNotNull(result.Data);
        }

        [TestMethod]
        public async Task CreateAsync_NonAdminUser_ReturnsUnsuccessfulResponseWithErrorMessage()
        {
            // Arrange
            var currencyDto = GetCurrencyDto();
            var loggedUser = GetLoggedUser();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var mapperMock = new Mock<IMapper>();

            var service = new CurrencyService(currencyRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await service.CreateAsync(currencyDto, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(Constants.ModifyUserErrorMessage, result.Message);
        }

        [TestMethod]
        public async Task CreateAsync_DuplicateCurrency_ReturnsUnsuccessfulResponse()
        {
            // Arrange
            var existingCurrencies = GetListCurrency();
            var currencyDto = GetCurrencyDto();
            var loggedUser = GetTestUserAdmin();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            currencyRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingCurrencies.AsQueryable);
            var mapperMock = new Mock<IMapper>();

            var service = new CurrencyService(currencyRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await service.CreateAsync(currencyDto, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(Constants.CurrencyExist, result.Message);
        }

        [TestMethod]
        public async Task DeleteAsync_AdminUserAndCurrencyExists_ReturnsSuccessfulResponseWithData()
        {
            // Arrange
            int currencyId = 1;
            var loggedUser = GetTestUserAdmin();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            currencyRepositoryMock.Setup(repo => repo.GetByIdAsync(currencyId)).ReturnsAsync(new Currency());
            var mapperMock = new Mock<IMapper>();
            var service = new CurrencyService(currencyRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await service.DeleteAsync(currencyId, loggedUser);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNotNull(result.Data);
        }

        [TestMethod]
        public async Task DeleteAsync_NonAdminUser_ReturnsUnsuccessfulResponse()
        {
            // Arrange
            int currencyId = 1;
            var loggedUser = GetTestUser();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var mapperMock = new Mock<IMapper>();
            var service = new CurrencyService(currencyRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await service.DeleteAsync(currencyId, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
        }

        [TestMethod]
        public async Task DeleteAsync_CurrencyNotFound_ReturnsUnsuccessfulResponse()
        {
            // Arrange
            int currencyId = 1; 
            var loggedUser = GetTestUserAdmin();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            currencyRepositoryMock.Setup(repo => repo.GetByIdAsync(currencyId)).ReturnsAsync((Currency)null);
            var mapperMock = new Mock<IMapper>();
            var service = new CurrencyService(currencyRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await service.DeleteAsync(currencyId, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(Constants.NoRecordsFound, result.Message);
        }

        [TestMethod]
        public async Task GetAllAsync_CurrenciesExist_ReturnsSuccessfulResponseWithData()
        {
            // Arrange
            var currencyList = GetListCurrency();
            var loggedUser = GetTestUserAdmin();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            currencyRepositoryMock.Setup(repo => repo.GetAll()).Returns(currencyList.AsQueryable);
            var mapperMock = new Mock<IMapper>();
            var service = new CurrencyService(currencyRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(currencyList.Count, result.Data.Count);
        }

        [TestMethod]
        public async Task GetAllAsync_NoCurrenciesExist_ReturnsUnsuccessfulResponse()
        {
            // Arrange
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            currencyRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Currency>().AsQueryable);
            var mapperMock = new Mock<IMapper>();
            var service = new CurrencyService(currencyRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(Constants.NoRecordsFound, result.Message);
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task GetAllAsync_CurrenciesWithDeletedFlag_ReturnsSuccessfulResponseWithFilteredData()
        {
            // Arrange
            var currencyList = GetListDelete();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            currencyRepositoryMock.Setup(repo => repo.GetAll()).Returns(currencyList.AsQueryable);
            var mapperMock = new Mock<IMapper>();
            var service = new CurrencyService(currencyRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Count);
            Assert.IsFalse(result.Data.Any(c => c.IsDeleted));
        }
    }
}
