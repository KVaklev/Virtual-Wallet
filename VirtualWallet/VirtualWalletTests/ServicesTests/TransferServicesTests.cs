using AutoMapper;
using Business.DTOs.Responses;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using Business.Services.Models;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using static VirtualWalletTests.TestHelpers.TestHelpers;
using Moq;

namespace VirtualWalletTests.ServicesTests
{
    [TestClass]
    public class TransferServicesTests
    {
        //[TestMethod]
        //public async Task FilterByAsync_NoRecordsFound_ReturnsErrorResponse()
        //{
        //    // Arrange
        //    var filterParameters = new TransferQueryParameters();
        //    var loggedUser = new User { };

        //    var transferRepositoryMock = new Mock<ITransferRepository>();
        //    var cardRepositoryMock = new Mock<ICardRepository>();
        //    var currencyRepositoryMock = new Mock<ICurrencyRepository>();
        //    var mapperMock = new Mock<IMapper>();
        //    var accountServiceMock = new Mock<IAccountService>();
        //    var cardServiceMock = new Mock<ICardService>();
        //    var exchangeRateServiceMock = new Mock<IExchangeRateService>();
        //    var securityMock = new Mock<ISecurityService>();

        //    var transferService = new TransferService(
        //      transferRepositoryMock.Object,
        //      new Mock<ICardRepository>().Object,
        //      new Mock<IHistoryRepository>().Object,
        //      mapperMock.Object,
        //      currencyRepositoryMock.Object,
        //      new Mock<IAccountService>().Object,
        //      new Mock<ICardService>().Object,
        //      new Mock<IExchangeRateService>().Object,
        //      securityMock.Object);

        //    // Act
        //    var result = await transferService.FilterByAsync(filterParameters, loggedUser);

        //    // Assert
        //    Assert.IsFalse(result.IsSuccessful);
        //    Assert.AreEqual(Constants.NoRecordsFound, result.Message);
        //    Assert.IsNull(result.Data);
        //}


        //[TestMethod]
        //public async Task GetByIdAsync_ValidIdUser_ReturnsValidResponseWithData()
        //{
        //    // Arrange
        //    int id = 1;
        //    User user = new User { IsAdmin = false };
        //    var transfer = GetTransferTest();
        //    var getTransferDto = GetTransferDtoTest();

        //    var transferRepositoryMock = new Mock<ITransferRepository>();
        //    var securityMock = new Mock<ISecurityService>();
        //    var mapperMock = new Mock<IMapper>();
        //    var cardRepositoryMock = new Mock<ICardRepository>();
        //    var historyRepositoryMock = new Mock<IHistoryRepository>();
        //    var currencyRepositoryMock = new Mock<ICurrencyRepository>();
        //    var accountServiceMock = new Mock<IAccountService>();
        //    var cardServiceMock = new Mock<ICardService>();
        //    var exchangeRateServiceMock = new Mock<IExchangeRateService>();
   
        //    transferRepositoryMock
        //        .Setup(repo => repo
        //        .GetByIdAsync(id))
        //        .ReturnsAsync(transfer);                  
        //    securityMock
        //        .Setup(security => security
        //        .IsUserAuthorizedAsync(It.IsAny<Transfer>(), user))
        //        .ReturnsAsync(true);
        //    mapperMock
        //        .Setup(mapper => mapper
        //        .Map<GetTransferDto>(It.IsAny<Transfer>()))
        //        .Returns(getTransferDto);

        //    var transferService = new TransferService(
        //        transferRepositoryMock.Object,
        //        cardRepositoryMock.Object,
        //        new Mock<IHistoryRepository>().Object,
        //        mapperMock.Object,
        //        currencyRepositoryMock.Object,
        //        accountServiceMock.Object,
        //        cardServiceMock.Object,
        //        exchangeRateServiceMock.Object,
        //        securityMock.Object);

        //    // Act
        //    var result = await transferService.GetByIdAsync(id, user);

        //    // Assert
        //    Assert.IsTrue(result.IsSuccessful);
        //    Assert.IsNotNull(result.Data);
        //}


        //[TestMethod]
        //public async Task CreateAsync_UserIsBlocked_ReturnsErrorResponse()
        //{
        //    // Arrange
        //    var createTransferDto = CreateTransferDtoTest();

        //    var transferDto = createTransferDto;
        //    var user = new User { IsBlocked = true };

        //    var transferRepositoryMock = new Mock<ITransferRepository>();
        //    var cardRepositoryMock = new Mock<ICardRepository>();
        //    var currencyRepositoryMock = new Mock<ICurrencyRepository>();
        //    var mapperMock = new Mock<IMapper>();
        //    var accountServiceMock = new Mock<IAccountService>();
        //    var cardServiceMock = new Mock<ICardService>();
        //    var exchangeRateServiceMock = new Mock<IExchangeRateService>();
        //    var securityMock = new Mock<ISecurityService>();

        //    var transferService = new TransferService(
        //        transferRepositoryMock.Object,
        //        cardRepositoryMock.Object,
        //        new Mock<IHistoryRepository>().Object,
        //        mapperMock.Object,
        //        currencyRepositoryMock.Object,
        //        accountServiceMock.Object,
        //        cardServiceMock.Object,
        //        exchangeRateServiceMock.Object,
        //        securityMock.Object);

        //    // Act
        //    var result = await transferService.CreateAsync(transferDto, user);

        //    // Assert
        //    Assert.IsFalse(result.IsSuccessful);
        //    Assert.AreEqual(Constants.ModifyTransferErrorMessage, result.Message);
        //    Assert.IsNull(result.Data);
        //}

        //[TestMethod]
        //public async Task GetByIdAsync_TransferNotFound_ReturnsErrorResponse()
        //{
        //    // Arrange
        //    int id = 1;
        //    User user = new User { IsAdmin = false };

        //    var transferRepositoryMock = new Mock<ITransferRepository>();
        //    var cardRepositoryMock = new Mock<ICardRepository>();
        //    var currencyRepositoryMock = new Mock<ICurrencyRepository>();
        //    var mapperMock = new Mock<IMapper>();
        //    var accountServiceMock = new Mock<IAccountService>();
        //    var cardServiceMock = new Mock<ICardService>();
        //    var exchangeRateServiceMock = new Mock<IExchangeRateService>();
        //    var securityMock = new Mock<ISecurityService>();

        //    transferRepositoryMock
        //        .Setup(repo => repo
        //        .GetByIdAsync(id))
        //        .ReturnsAsync((Transfer?)null!);

        //    var transferService = new TransferService(
        //       transferRepositoryMock.Object,
        //       cardRepositoryMock.Object,
        //       new Mock<IHistoryRepository>().Object,
        //       mapperMock.Object,
        //       currencyRepositoryMock.Object,
        //       accountServiceMock.Object,
        //       cardServiceMock.Object,
        //       exchangeRateServiceMock.Object,
        //       securityMock.Object);

        //    // Act
        //    var result = await transferService.GetByIdAsync(id, user);

        //    // Assert
        //    Assert.IsFalse(result.IsSuccessful);
        //    Assert.AreEqual(Constants.NoRecordsFound, result.Message);
        //    Assert.IsNull(result.Data);
        //}

        //[TestMethod]
        //public async Task GetByIdAsync_UserNotAuthorized_ReturnsErrorResponse()
        //{
        //    // Arrange
        //    int id = 1;
        //    User user = new User { IsAdmin = false };
        //    var transfer = GetTransferTest();
        //    var transferToGet = transfer;

        //    var transferRepositoryMock = new Mock<ITransferRepository>();
        //    var cardRepositoryMock = new Mock<ICardRepository>();
        //    var currencyRepositoryMock = new Mock<ICurrencyRepository>();
        //    var mapperMock = new Mock<IMapper>();
        //    var accountServiceMock = new Mock<IAccountService>();
        //    var cardServiceMock = new Mock<ICardService>();
        //    var exchangeRateServiceMock = new Mock<IExchangeRateService>();
        //    var securityMock = new Mock<ISecurityService>();

        //    transferRepositoryMock
        //        .Setup(repo => repo
        //        .GetByIdAsync(id))
        //        .ReturnsAsync(transferToGet);
                            
        //    securityMock
        //        .Setup(security => security
        //        .IsUserAuthorizedAsync(transferToGet, user))
        //        .ReturnsAsync(false);

        //    var transferService = new TransferService(
        //       transferRepositoryMock.Object,
        //       cardRepositoryMock.Object,
        //       new Mock<IHistoryRepository>().Object,
        //       mapperMock.Object,
        //       currencyRepositoryMock.Object,
        //       accountServiceMock.Object,
        //       cardServiceMock.Object,
        //       exchangeRateServiceMock.Object,
        //       securityMock.Object);

        //    // Act
        //    var result = await transferService.GetByIdAsync(id, user);

        //    // Assert
        //    Assert.IsFalse(result.IsSuccessful);
        //    Assert.AreEqual(Constants.ModifyTransferGetByIdErrorMessage, result.Message);
        //    Assert.IsNull(result.Data);
        //}

        //[TestMethod]
        //public async Task CreateAsync_CurrencyNotFound_ReturnsErrorResponse()
        //{
        //    // Arrange
        //    var createTransferDto = CreateTransferDtoTest();
        //    var transferDto = createTransferDto;
        //    var user = new User { IsBlocked = false, AccountId = 1 };

        //    var transferRepositoryMock = new Mock<ITransferRepository>();
        //    var cardRepositoryMock = new Mock<ICardRepository>();
        //    var currencyRepositoryMock = new Mock<ICurrencyRepository>();
        //    var mapperMock = new Mock<IMapper>();
        //    var accountServiceMock = new Mock<IAccountService>();
        //    var cardServiceMock = new Mock<ICardService>();
        //    var exchangeRateServiceMock = new Mock<IExchangeRateService>();
        //    var securityMock = new Mock<ISecurityService>();

        //    cardRepositoryMock
        //        .Setup(repo => repo
        //        .GetByAccountId(It.IsAny<int>()))
        //        .Returns(Enumerable.Empty<Card>()
        //        .AsQueryable());

        //    currencyRepositoryMock
        //        .Setup(repo => repo
        //        .GetByCurrencyCodeAsync(transferDto.CurrencyCode))
        //        .ReturnsAsync((Currency)null);

        //    var transferService = new TransferService(
        //      transferRepositoryMock.Object,
        //      cardRepositoryMock.Object,
        //      new Mock<IHistoryRepository>().Object,
        //      mapperMock.Object,
        //      currencyRepositoryMock.Object,
        //      accountServiceMock.Object,
        //      cardServiceMock.Object,
        //      exchangeRateServiceMock.Object,
        //      securityMock.Object);

        //    // Act
        //    var result = await transferService.CreateAsync(transferDto, user);

        //    // Assert
        //    Assert.IsFalse(result.IsSuccessful);
        //    Assert.AreEqual(Constants.NoRecordsFound, result.Message);
        //    Assert.IsNull(result.Data);
        //}

        //[TestMethod]
        //public async Task CreateAsync_InsufficientBalance_ReturnsErrorResponse()
        //{
        //    // Arrange
        //    var createTransferDto = CreateTransferDtoTest();
        //    var transferDto = createTransferDto;
        //    var cardToGet = GetCardTest();
        //    var user = new User { IsBlocked = false, AccountId = 1 };
        //    var card = cardToGet;
        //    var accountToGet = GetAccountRecipient();
        //    var account = accountToGet;
        //    var transferToGet = GetTransferTest();
        //    var transfer = transferToGet;

        //    var transferRepositoryMock = new Mock<ITransferRepository>();
        //    var cardRepositoryMock = new Mock<ICardRepository>();
        //    var currencyRepositoryMock = new Mock<ICurrencyRepository>();
        //    var mapperMock = new Mock<IMapper>();
        //    var accountServiceMock = new Mock<IAccountService>();
        //    var cardServiceMock = new Mock<ICardService>();
        //    var exchangeRateServiceMock = new Mock<IExchangeRateService>();
        //    var securityMock = new Mock<ISecurityService>();

        //    cardRepositoryMock
        //        .Setup(repo => repo
        //        .GetByAccountId(It.IsAny<int>()))
        //        .Returns(new List<Card> { card }
        //        .AsQueryable());

        //    var transferService = new TransferService(
        //      transferRepositoryMock.Object,
        //      cardRepositoryMock.Object,
        //      new Mock<IHistoryRepository>().Object,
        //      mapperMock.Object,
        //      currencyRepositoryMock.Object,
        //      accountServiceMock.Object,
        //      cardServiceMock.Object,
        //      exchangeRateServiceMock.Object,
        //      securityMock.Object);

        //    //Act
        //    var result = await transferService.CreateAsync(transferDto, user);

        //    // Assert
        //    Assert.IsFalse(result.IsSuccessful);
        //    Assert.AreEqual(Constants.ModifyAccountBalancetErrorMessage, result.Message);
        //    Assert.IsNull(result.Data);
        //}

        //[TestMethod]
        //public async Task CreateAsync_TransferCreation_ReturnsSuccessResponse()
        //{
        //    // Arrange

        //    var transferDtoToGet = CreateTransferDtoTest();
        //    var transferDto = transferDtoToGet;
        //    var user = new User { IsBlocked = false, AccountId = 1 };
        //    var cardToGet = GetCardTest();
        //    var card = cardToGet;
        //    var currencyToGet = GetCurrency();
        //    var currency = currencyToGet;
        //    var transfer = transferToGet;
        //    var transferToGet = GetTransferTest();

        //    var transferRepositoryMock = new Mock<ITransferRepository>();
        //    var cardRepositoryMock = new Mock<ICardRepository>();
        //    var currencyRepositoryMock = new Mock<ICurrencyRepository>();
        //    var mapperMock = new Mock<IMapper>();
        //    var accountServiceMock = new Mock<IAccountService>();
        //    var cardServiceMock = new Mock<ICardService>();
        //    var exchangeRateServiceMock = new Mock<IExchangeRateService>();
        //    var securityMock = new Mock<ISecurityService>();

             
        //    transferRepositoryMock
        //        .Setup(repo => repo
        //        .CreateAsync(It.IsAny<Transfer>()))
        //        .ReturnsAsync(transfer);


        //    var transferService = new TransferService(
        //      transferRepositoryMock.Object,
        //      cardRepositoryMock.Object,
        //      new Mock<IHistoryRepository>().Object,
        //      mapperMock.Object,
        //      currencyRepositoryMock.Object,
        //      accountServiceMock.Object,
        //      cardServiceMock.Object,
        //      exchangeRateServiceMock.Object,
        //      securityMock.Object);



        //    // Act
        //    var result = await transferService.CreateAsync(transferDto, user);

        //    // Assert
        //    Assert.IsTrue(result.IsSuccessful);
        //    Assert.IsNull(result.Message);
        //    Assert.IsNotNull(result.Data);
        //}
    }
}
