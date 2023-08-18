using AutoMapper;
using Business.DTOs.Responses;
using Business.Mappers;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using Business.Services.Models;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using Microsoft.AspNetCore.Hosting;
using Moq;
using static VirtualWalletTests.TestHelpers.TestHelpers;


namespace VirtualWalletTests.ServicesTests
{
    [TestClass]
    public class TransferServicesTests
    {
        [TestMethod]
        public async Task FilterByAsync_NoRecordsFound_ReturnsErrorResponse()
        {
            // Arrange
            var filterParameters = new TransferQueryParameters();
            var loggedUser = new User { };

            var transferService = CreateTransferServiceWithMocks();

            // Act
            var result = await transferService.FilterByAsync(filterParameters, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(Constants.NoRecordsFound, result.Message);
            Assert.IsNull(result.Data);
        }

        public TransferService CreateTransferServiceWithMocks()
        {
            var transferRepositoryMock = new Mock<ITransferRepository>();
            var cardRepositoryMock = new Mock<ICardRepository>();
            var historyRepositoryMock = new Mock<IHistoryRepository>();
            var mapperMock = new Mock<IMapper>();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityServiceMock = new Mock<ISecurityService>();

            var transferService = new TransferService(
                transferRepositoryMock.Object,
                cardRepositoryMock.Object,
                new ApplicationContext(),
                historyRepositoryMock.Object,
                mapperMock.Object,
                currencyRepositoryMock.Object,
                accountServiceMock.Object,
                cardServiceMock.Object,
                exchangeRateServiceMock.Object,
                securityServiceMock.Object);

            return transferService;
        }


        [TestMethod]
        public async Task GetByIdAsync_ValidIdUser_ReturnsValidResponseWithData()
        {
            // Arrange
            int id = 1;
            User user = new User { IsAdmin = false };
            var transfer = GetTransferTest();
            var getTransferDto = GetTransferDtoTest();

            var transferRepositoryMock = new Mock<ITransferRepository>();
            var cardRepositoryMock = new Mock<ICardRepository>();
            var historyRepositoryMock = new Mock<IHistoryRepository>();
            var mapperMock = new Mock<IMapper>();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityServiceMock = new Mock<ISecurityService>();

            transferRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                                 .ReturnsAsync(transfer);

            var securityMock = new Mock<ISecurityService>();
            securityMock.Setup(security => security.IsUserAuthorizedAsync(It.IsAny<Transfer>(), user))
                        .ReturnsAsync(true);

            mapperMock.Setup(mapper => mapper.Map<GetTransferDto>(It.IsAny<Transfer>()))
                      .Returns(getTransferDto);

            var transferService = new TransferService(
                transferRepositoryMock.Object,
                cardRepositoryMock.Object,
                new ApplicationContext(),
                historyRepositoryMock.Object,
                mapperMock.Object,
                currencyRepositoryMock.Object,
                accountServiceMock.Object,
                cardServiceMock.Object,
                exchangeRateServiceMock.Object,
                securityServiceMock.Object);

            // Act
            var result = await transferService.GetByIdAsync(id, user);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNotNull(result.Data);

        }

        [TestMethod]
        public async Task CreateAsync_UserIsBlocked_ReturnsErrorResponse()
        {
            // Arrange
            var createTransferDto = CreateTransferDtoTest();

            var transferDto = createTransferDto;
            var user = new User { IsBlocked = true };

            var transferRepositoryMock = new Mock<ITransferRepository>();
            var cardRepositoryMock = new Mock<ICardRepository>();
            var historyRepositoryMock = new Mock<IHistoryRepository>();
            var mapperMock = new Mock<IMapper>();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityServiceMock = new Mock<ISecurityService>();

            var transferService = new TransferService(
                 transferRepositoryMock.Object,
                 cardRepositoryMock.Object,
                 new ApplicationContext(),
                 historyRepositoryMock.Object,
                 mapperMock.Object,
                 currencyRepositoryMock.Object,
                 accountServiceMock.Object,
                 cardServiceMock.Object,
                 exchangeRateServiceMock.Object,
                 securityServiceMock.Object);

            // Act
            var result = await transferService.CreateAsync(transferDto, user);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(Constants.ModifyTransferErrorMessage, result.Message);
            Assert.IsNull(result.Data);

        }

        [TestMethod]
        public async Task GetByIdAsync_UserNotAuthorizedAndNotAdmin_ReturnsErrorResponse()
        {
            // Arrange
            int id = 1;
            User user = new User { IsAdmin = false };

            var transfer = GetTransferTest();

            var transferToGet = transfer;

            var cardRepositoryMock = new Mock<ICardRepository>();
            var historyRepositoryMock = new Mock<IHistoryRepository>();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityServiceMock = new Mock<ISecurityService>();
            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                                .ReturnsAsync(transferToGet);
            var securityMock = new Mock<ISecurityService>();
            securityMock.Setup(security => security.IsUserAuthorizedAsync(transferToGet, user))
                        .ReturnsAsync(false);

            var mapperMock = new Mock<IMapper>();

            var transferService = new TransferService(
                 transferRepositoryMock.Object,
                 cardRepositoryMock.Object,
                 new ApplicationContext(),
                 historyRepositoryMock.Object,
                 mapperMock.Object,
                 currencyRepositoryMock.Object,
                 accountServiceMock.Object,
                 cardServiceMock.Object,
                 exchangeRateServiceMock.Object,
                 securityServiceMock.Object);

            // Act
            var result = await transferService.GetByIdAsync(id, user);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(Constants.ModifyTransferGetByIdErrorMessage, result.Message);
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task GetByIdAsync_TransferNotFound_ReturnsErrorResponse()
        {
            // Arrange
            int id = 1;
            User user = new User { IsAdmin = false };

            var cardRepositoryMock = new Mock<ICardRepository>();
            var historyRepositoryMock = new Mock<IHistoryRepository>();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityServiceMock = new Mock<ISecurityService>();
            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                                 .ReturnsAsync((Transfer)null);

            var securityMock = new Mock<ISecurityService>();
            var mapperMock = new Mock<IMapper>();

            var transferService = new TransferService(
                 transferRepositoryMock.Object,
                 cardRepositoryMock.Object,
                 new ApplicationContext(),
                 historyRepositoryMock.Object,
                 mapperMock.Object,
                 currencyRepositoryMock.Object,
                 accountServiceMock.Object,
                 cardServiceMock.Object,
                 exchangeRateServiceMock.Object,
                 securityServiceMock.Object);

            // Act
            var result = await transferService.GetByIdAsync(id, user);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(Constants.NoRecordsFound, result.Message);
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task GetByIdAsync_UserNotAuthorized_ReturnsErrorResponse()
        {
            // Arrange
            int id = 1;

            User user = new User { IsAdmin = false };

            var transfer = GetTransferTest();

            var transferToGet = transfer;

            var cardRepositoryMock = new Mock<ICardRepository>();
            var historyRepositoryMock = new Mock<IHistoryRepository>();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityServiceMock = new Mock<ISecurityService>();
            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                                 .ReturnsAsync(transferToGet);

            var securityMock = new Mock<ISecurityService>();
            securityMock.Setup(security => security.IsUserAuthorizedAsync(transferToGet, user))
                        .ReturnsAsync(false);

            var mapperMock = new Mock<IMapper>();

            var transferService = new TransferService(
                 transferRepositoryMock.Object,
                 cardRepositoryMock.Object,
                 new ApplicationContext(),
                 historyRepositoryMock.Object,
                 mapperMock.Object,
                 currencyRepositoryMock.Object,
                 accountServiceMock.Object,
                 cardServiceMock.Object,
                 exchangeRateServiceMock.Object,
                 securityServiceMock.Object);

            // Act
            var result = await transferService.GetByIdAsync(id, user);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(Constants.ModifyTransferGetByIdErrorMessage, result.Message);
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task CreateAsync_CurrencyNotFound_ReturnsErrorResponse()
        {
            // Arrange
            var createTransferDto = CreateTransferDtoTest();

            var transferDto = createTransferDto;

            var user = new User { IsBlocked = false, AccountId = 1 };

            var cardRepositoryMock = new Mock<ICardRepository>();
            var historyRepositoryMock = new Mock<IHistoryRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityServiceMock = new Mock<ISecurityService>();

            cardRepositoryMock.Setup(repo => repo.GetByAccountId(It.IsAny<int>()))
                             .Returns(Enumerable.Empty<Card>().AsQueryable());

            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            currencyRepositoryMock.Setup(repo => repo.GetByCurrencyCodeAsync(transferDto.CurrencyCode))
                                  .ReturnsAsync((Currency)null);

            var transferRepositoryMock = new Mock<ITransferRepository>();
            var mapperMock = new Mock<IMapper>();
            var securityMock = new Mock<ISecurityService>();

            var transferService = new TransferService(
                 transferRepositoryMock.Object,
                 cardRepositoryMock.Object,
                 new ApplicationContext(),
                 historyRepositoryMock.Object,
                 mapperMock.Object,
                 currencyRepositoryMock.Object,
                 accountServiceMock.Object,
                 cardServiceMock.Object,
                 exchangeRateServiceMock.Object,
                 securityServiceMock.Object);


            // Act
            var result = await transferService.CreateAsync(transferDto, user);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(Constants.NoRecordsFound, result.Message);
            Assert.IsNull(result.Data);

        }

        [TestMethod]
        public async Task CreateAsync_InsufficientBalance_ReturnsErrorResponse()
        {
            // Arrange
            var createTransferDto = CreateTransferDtoTest();

            var transferDto = createTransferDto;

            var cardToGet = GetCardTest();

            var user = new User { IsBlocked = false, AccountId = 1 };

            var card = cardToGet;

            var accountToGet = GetAccountRecipient();

            var account = accountToGet;

            var transferToGet = GetTransferTest();

            var transfer = transferToGet;

            var historyRepositoryMock = new Mock<IHistoryRepository>();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityServiceMock = new Mock<ISecurityService>();

            var cardRepositoryMock = new Mock<ICardRepository>();
            cardRepositoryMock.Setup(repo => repo.GetByAccountId(It.IsAny<int>()))
                             .Returns(new List<Card> { card }.AsQueryable());

            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Transfer>()))
                                 .ReturnsAsync(transfer);
            var mapperMock = new Mock<IMapper>();

            var securityMock = new Mock<ISecurityService>();

            var transferService = new TransferService(
            transferRepositoryMock.Object,
            cardRepositoryMock.Object,
            new ApplicationContext(),
            historyRepositoryMock.Object,
            mapperMock.Object,
            currencyRepositoryMock.Object,
            accountServiceMock.Object,
            cardServiceMock.Object,
            exchangeRateServiceMock.Object,
            securityServiceMock.Object);

            // Act
            var result = await transferService.CreateAsync(transferDto, user);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(Constants.ModifyAccountBalancetErrorMessage, result.Message);
            Assert.IsNull(result.Data);

        }

        [TestMethod]
        public async Task CreateAsync_TransferCreation_ReturnsSuccessResponse()
        {
            // Arrange
            var transferDtoToGet = CreateTransferDtoTest();

            var transferDto = transferDtoToGet;

            var user = new User { IsBlocked = false, AccountId = 1 };

            var cardToGet = GetCardTest();

            var card = cardToGet;

            var currencyToGet = GetCurrency();

            var currency = currencyToGet;

            var transferToGet = GetTransferTest();

            var transfer = transferToGet;

            var historyRepositoryMock = new Mock<IHistoryRepository>();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityServiceMock = new Mock<ISecurityService>();

            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Transfer>()))
                                 .ReturnsAsync(transfer);

            var cardRepositoryMock = new Mock<ICardRepository>();
            cardRepositoryMock.Setup(repo => repo.GetByAccountId(It.IsAny<int>()))
                             .Returns(new List<Card> { card }.AsQueryable());


            var mapperMock = new Mock<IMapper>();

            var securityMock = new Mock<ISecurityService>();

            var transferService = new TransferService(
            transferRepositoryMock.Object,
            cardRepositoryMock.Object,
            new ApplicationContext(),
            historyRepositoryMock.Object,
            mapperMock.Object,
            currencyRepositoryMock.Object,
            accountServiceMock.Object,
            cardServiceMock.Object,
            exchangeRateServiceMock.Object,
            securityServiceMock.Object);

            // Act
            var result = await transferService.CreateAsync(transferDto, user);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNull(result.Message);
            Assert.IsNotNull(result.Data);
        }
    }
}

