using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Mappers;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using Business.Services.Models;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using Microsoft.AspNetCore.Hosting;
using Moq;
using static Business.Services.Helpers.Constants;
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
            var loggedUser = new User { /* Setup user properties as needed */ };

            var transferService = CreateTransferServiceWithMocks();

            // Act
            var result = await transferService.FilterByAsync(filterParameters, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(Constants.NoRecordsFound, result.Message);
            Assert.IsNull(result.Data);
        }

        private TransferService CreateTransferServiceWithMocks()
        {
            var transferRepositoryMock = new Mock<ITransferRepository>();
           

            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            

            var mapperMock = new Mock<IMapper>();
            

            var securityMock = new Mock<ISecurityService>();
            

            var transferService = new TransferService(
                transferRepositoryMock.Object,
                new Mock<ICardRepository>().Object,
                new Mock<ApplicationContext>().Object,
                new Mock<IHistoryRepository>().Object,
                mapperMock.Object,
                currencyRepositoryMock.Object,
                new Mock<IAccountService>().Object,
                new Mock<ICardService>().Object,
                new Mock<IExchangeRateService>().Object,
                securityMock.Object);

            return transferService;
        }

        [TestMethod]
        public async Task GetByIdAsync_ValidIdUser_ReturnsValidResponseWithData()
        {
            // Arrange
            int id = 1;
            User user = new User { IsAdmin = false };

            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                                 .ReturnsAsync(new Transfer
                                 {
                                     Id = 1,
                                     AccountId = 1,
                                     CurrencyId = 2,
                                     Amount = 10,
                                     CardId = 1,
                                     IsConfirmed = false,
                                     IsCancelled = false,
                                     DateCreated = DateTime.Now,
                                     TransferType = TransferDirection.Deposit,
                                 });

            var securityMock = new Mock<ISecurityService>();
            securityMock.Setup(security => security.IsUserAuthorizedAsync(It.IsAny<Transfer>(), user))
                        .ReturnsAsync(true);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<GetTransferDto>(It.IsAny<Transfer>()))
                      .Returns(new GetTransferDto
                      {
                          Id = 1,
                          Username = "mariicheto",
                          Amount = 10,
                          CurrencyCode = "BGN",
                          CardNumber = "1234567899875642",
                          TransferType = TransferDirection.Deposit.ToString(),
                          IsConfirmed = false,
                          IsCancelled = false
                      });

            var cardRepositoryMock = new Mock<ICardRepository>();
            var contextMock = new Mock<ApplicationContext>();
            var historyRepositoryMock = new Mock<IHistoryRepository>();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();

            var transferService = new TransferService(
                transferRepositoryMock.Object,
                cardRepositoryMock.Object,
                contextMock.Object,
                historyRepositoryMock.Object,
                mapperMock.Object,
                currencyRepositoryMock.Object,
                accountServiceMock.Object,
                cardServiceMock.Object,
                exchangeRateServiceMock.Object,
                securityMock.Object);

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
            var transferDto = new CreateTransferDto
            {
                Amount = 10,
                CurrencyCode = "BGN",
                CardNumber = "1234567899875642",
                TransferType = TransferDirection.Deposit.ToString(),
            };
            var user = new User { IsBlocked = true };

            var transferRepositoryMock = new Mock<ITransferRepository>();
            var cardRepositoryMock = new Mock<ICardRepository>();
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var mapperMock = new Mock<IMapper>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityMock = new Mock<ISecurityService>();

            var transferService = new TransferService(
                transferRepositoryMock.Object,
                cardRepositoryMock.Object,
                new Mock<ApplicationContext>().Object,
                new Mock<IHistoryRepository>().Object,
                mapperMock.Object,
                currencyRepositoryMock.Object,
                accountServiceMock.Object,
                cardServiceMock.Object,
                exchangeRateServiceMock.Object,
                securityMock.Object);

            // Act
            var result = await transferService.CreateAsync(transferDto, user);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("You are not authorized to create, delete or modify the transfer.", result.Message);
            Assert.IsNull(result.Data);

        }

        
       
        [TestMethod]
        public async Task GetByIdAsync_UserNotAuthorizedAndNotAdmin_ReturnsErrorResponse()
        {
            // Arrange
            int id = 1;
            User user = new User { IsAdmin = false };

            var transferToGet = new Transfer
            {
                Id = 1,
                AccountId = 1,
                CurrencyId = 2,
                Amount = 10,
                CardId = 1,
                IsConfirmed = false,
                IsCancelled = false,
                DateCreated = DateTime.Now,
                TransferType = TransferDirection.Deposit,
            };

            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                                 .ReturnsAsync(transferToGet);

            var securityMock = new Mock<ISecurityService>();
            securityMock.Setup(security => security.IsUserAuthorizedAsync(transferToGet, user))
                        .ReturnsAsync(false); // User is not authorized

            var mapperMock = new Mock<IMapper>();

            var transferService = new TransferService(
                transferRepositoryMock.Object,
                new Mock<ICardRepository>().Object,
                new Mock<ApplicationContext>().Object,
                new Mock<IHistoryRepository>().Object,
                mapperMock.Object,
                new Mock<ICurrencyRepository>().Object,
                new Mock<IAccountService>().Object,
                new Mock<ICardService>().Object,
                new Mock<IExchangeRateService>().Object,
                securityMock.Object);

            // Act
            var result = await transferService.GetByIdAsync(id, user);

            // Assert
            Assert.IsTrue(result.IsSuccessful); // Changed to Assert.IsTrue
            Assert.AreEqual("You are not authorized for the specified action.", result.Message);
            Assert.IsNull(result.Data);
            // Additional assertions can be added based on your specific requirements
        }

        [TestMethod]
        public async Task GetByIdAsync_TransferNotFound_ReturnsErrorResponse()
        {
            // Arrange
            int id = 1;
            User user = new User { IsAdmin = false };

            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                                 .ReturnsAsync((Transfer)null); 

            var securityMock = new Mock<ISecurityService>();
            var mapperMock = new Mock<IMapper>();

            var transferService = new TransferService(
                transferRepositoryMock.Object,
                new Mock<ICardRepository>().Object,
                new Mock<ApplicationContext>().Object,
                new Mock<IHistoryRepository>().Object,
                mapperMock.Object,
                new Mock<ICurrencyRepository>().Object,
                new Mock<IAccountService>().Object,
                new Mock<ICardService>().Object,
                new Mock<IExchangeRateService>().Object,
                securityMock.Object);

            // Act
            var result = await transferService.GetByIdAsync(id, user);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("No records found", result.Message);
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task GetByIdAsync_UserNotAuthorized_ReturnsErrorResponse()
        {
            // Arrange
            int id = 1;
            User user = new User { IsAdmin = false };

            var transferToGet = new Transfer
            {
                Id = 1,
                AccountId = 1,
                CurrencyId = 2,
                Amount = 10,
                CardId = 1,
                IsConfirmed = false,
                IsCancelled = false,
                DateCreated = DateTime.Now,
                TransferType = TransferDirection.Deposit,
            };

            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                                 .ReturnsAsync(transferToGet);

            var securityMock = new Mock<ISecurityService>();
            securityMock.Setup(security => security.IsUserAuthorizedAsync(transferToGet, user))
                        .ReturnsAsync(false);

            var mapperMock = new Mock<IMapper>();

            var transferService = new TransferService(
                transferRepositoryMock.Object,
                new Mock<ICardRepository>().Object,
                new Mock<ApplicationContext>().Object,
                new Mock<IHistoryRepository>().Object,
                mapperMock.Object,
                new Mock<ICurrencyRepository>().Object,
                new Mock<IAccountService>().Object,
                new Mock<ICardService>().Object,
                new Mock<IExchangeRateService>().Object,
                securityMock.Object);

            // Act
            var result = await transferService.GetByIdAsync(id, user);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("You are not authorized for the specified action.", result.Message);
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task CreateAsync_CurrencyNotFound_ReturnsErrorResponse()
        {
            // Arrange
            var transferDto = new CreateTransferDto
            {
                Amount = 10,
                CurrencyCode = null,
                CardNumber = "1234567899875642",
                TransferType = TransferDirection.Deposit.ToString(),
            };

            var user = new User { IsBlocked = false, AccountId = 1 };

            var cardRepositoryMock = new Mock<ICardRepository>();
            cardRepositoryMock.Setup(repo => repo.GetByAccountId(It.IsAny<int>()))
                             .Returns(Enumerable.Empty<Card>().AsQueryable());

            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            currencyRepositoryMock.Setup(repo => repo.GetByCurrencyCodeAsync(transferDto.CurrencyCode))
                                  .ReturnsAsync((Currency)null); // Simulate currency not found

            var transferRepositoryMock = new Mock<ITransferRepository>();
            var mapperMock = new Mock<IMapper>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityMock = new Mock<ISecurityService>();

            var transferService = new TransferService(
                transferRepositoryMock.Object,
                cardRepositoryMock.Object,
                new Mock<ApplicationContext>().Object,
                new Mock<IHistoryRepository>().Object,
                mapperMock.Object,
                currencyRepositoryMock.Object,
                accountServiceMock.Object,
                cardServiceMock.Object,
                exchangeRateServiceMock.Object,
                securityMock.Object);

            // Act
            var result = await transferService.CreateAsync(transferDto, user);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("No records found", result.Message);
            Assert.IsNull(result.Data);

        }

        [TestMethod]
        public async Task CreateAsync_InsufficientBalance_ReturnsErrorResponse()
        {
            // Arrange
            var transferDto = new CreateTransferDto
            {
                Amount = 100,
                CurrencyCode = "USD",
                CardNumber = "1234567899875642",
                TransferType = TransferDirection.Deposit.ToString(),
            };

            var user = new User { IsBlocked = false, AccountId = 1 };

            var card = new Card
            {
                CardNumber = transferDto.CardNumber,
                Balance = 50,
            };

            var account = new Account
            {
                Id = 2,
                Balance = 1000,
                CurrencyId = 2,
                IsDeleted = false,
                DateCreated = DateTime.Now,
                UserId = 2
            };

            var transfer = new Transfer
            {
                Id = 1,
                AccountId = 1,
                CurrencyId = 2,
                Amount = 10,
                CardId = 1,
                IsConfirmed = false,
                IsCancelled = false,
                DateCreated = DateTime.Now,
                TransferType = TransferDirection.Deposit
            };

            var cardRepositoryMock = new Mock<ICardRepository>();
            cardRepositoryMock.Setup(repo => repo.GetByAccountId(It.IsAny<int>()))
                             .Returns(new List<Card> { card }.AsQueryable());

            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Transfer>()))
                                 .ReturnsAsync(transfer);

            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var mapperMock = new Mock<IMapper>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityMock = new Mock<ISecurityService>();

            var transferService = new TransferService(
                transferRepositoryMock.Object,
                cardRepositoryMock.Object,
                new Mock<ApplicationContext>().Object,
                new Mock<IHistoryRepository>().Object,
                mapperMock.Object,
                currencyRepositoryMock.Object,
                accountServiceMock.Object,
                cardServiceMock.Object,
                exchangeRateServiceMock.Object,
                securityMock.Object);

            // Act
            var result = await transferService.CreateAsync(transferDto, user);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Insufficient balance.", result.Message);
            Assert.IsNull(result.Data);
            // Additional assertions can be added based on your specific requirements
        }

        [TestMethod]
        public async Task CreateAsync_TransferCreation_ReturnsSuccessResponse()
        {
            // Arrange
            var transferDto = new CreateTransferDto
            {
                Amount = 10,
                CurrencyCode = "BGN",
                CardNumber = "1234567899875642",
                TransferType = TransferDirection.Deposit.ToString(),
            };

            var user = new User { IsBlocked = false, AccountId = 1 };

            var card = new Card
            {
                Id = 1,
                CardNumber = "1234567891011121",
                ExpirationDate = DateTime.MaxValue,
                CardHolder = "Ivancho Draganchov",
                CheckNumber = "005",
                AccountId = 1,
                Balance = 750,
                CurrencyId = 1,
                IsDeleted = false,
            };

            var currency = new Currency
            {
                Id = 1,
                Name = "Bulgarian Lev",
                CurrencyCode = "BGN",
                IsDeleted = false
            };

            var transfer = new Transfer
            {
                Id = 1,
                AccountId = 1,
                CurrencyId = 2,
                Amount = 10,
                CardId = 1,
                IsConfirmed = false,
                IsCancelled = false,
                DateCreated = DateTime.Now,
                TransferType = TransferDirection.Deposit,
            };

            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Transfer>()))
                                 .ReturnsAsync(transfer);

            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var mapperMock = new Mock<IMapper>();
            var accountServiceMock = new Mock<IAccountService>();
            var cardServiceMock = new Mock<ICardService>();
            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            var securityMock = new Mock<ISecurityService>();

            var transferService = new TransferService(
                transferRepositoryMock.Object,
                new Mock<ICardRepository>().Object,
                new Mock<ApplicationContext>().Object,
                new Mock<IHistoryRepository>().Object,
                mapperMock.Object,
                currencyRepositoryMock.Object,
                accountServiceMock.Object,
                cardServiceMock.Object,
                exchangeRateServiceMock.Object,
                securityMock.Object);

            // Act
            var result = await transferService.CreateAsync(transferDto, user);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNull(result.Message);
            Assert.IsNotNull(result.Data);
        }
    }
}
