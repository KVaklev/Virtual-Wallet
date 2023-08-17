
    using AutoMapper;
    using Business.DTOs.Responses;
    using Business.Services.Contracts;
    using Business.Services.Models;
    using DataAccess.Models.Models;
    using DataAccess.Repositories.Contracts;
    using Moq;
    using Business.Mappers.Contracts;

    namespace VirtualWalletTests.ServicesTests
    {
        [TestClass]
        public class TransactionServiceTests
        {
            //        [TestMethod]
            //        public async Task GetByIdAsync_ValidTransaction_ReturnsSuccessfulResponseWithData()
            //        {
            //            // Arrange
            //            //int transactionId = 1;
            //            //var loggedUser = GetTestUser();
            //            //var transaction = GetTransaction();
            //            //var mappedTransactionDto = GetTransactionDto();

            //            var transactionRepositoryMock = new Mock<ITransactionRepository>();
            //            transactionRepositoryMock.Setup(repo => repo.GetByIdAsync(transactionId)).ReturnsAsync(transaction);

            //            var transactionCheckerServiceMock = new Mock<ITransactionCheckerService>();
            //            transactionCheckerServiceMock.Setup(check => check.ChecksGetByIdAsync(transaction, loggedUser))
            //                                        .ReturnsAsync(new Response<GetTransactionDto> { IsSuccessful = true });
            //            var accountRepositoryMock = new Mock<IAccountRepository>();
            //            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            //            var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            //            var accountServiceMock = new Mock<IAccountService>();
            //            var historyRepositoryMock = new Mock<IHistoryRepository>();
            //            var transactionsMapperMock = new Mock<ITransactionsMapper>();

            //            var mapperMock = new Mock<IMapper>();
            //            mapperMock.Setup(m => m.Map<GetTransactionDto>(transaction)).Returns(mappedTransactionDto);

            //            var service = new TransactionService(
            //            transactionRepositoryMock.Object,
            //            accountRepositoryMock.Object,
            //            currencyRepositoryMock.Object,
            //            exchangeRateServiceMock.Object,
            //            accountServiceMock.Object,
            //            historyRepositoryMock.Object,
            //            transactionCheckerServiceMock.Object,
            //            mapperMock.Object);

            //            // Act
            //            var result = await service.GetByIdAsync(transactionId, loggedUser);

            //            // Assert
            //            Assert.IsTrue(result.IsSuccessful);
            //            Assert.AreEqual(mappedTransactionDto, result.Data);

            //        }
            //    }
            //}
            //[TestMethod]
            //public async Task GetByIdAsync_ValidTransaction_ReturnsUnSuccessfulResponseWithoutData()
            //{
            //    // Arrange
            //    int transactionId = 1;
            //    var loggedUser = GetTestUser();
            //    var transaction = GetTransaction();
            //    var mappedTransactionDto = GetTransactionDto();

            //    var transactionRepositoryMock = new Mock<ITransactionRepository>();
            //    transactionRepositoryMock.Setup(repo => repo.GetByIdAsync(transactionId)).ReturnsAsync(transaction);

            //    var transactionCheckerServiceMock = new Mock<ITransactionCheckerService>();
            //    transactionCheckerServiceMock.Setup(check => check.ChecksGetByIdAsync(transaction, loggedUser))
            //                                .ReturnsAsync(new Response<GetTransactionDto> { IsSuccessful = false });
            //    var accountRepositoryMock = new Mock<IAccountRepository>();
            //    var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            //    var exchangeRateServiceMock = new Mock<IExchangeRateService>();
            //    var accountServiceMock = new Mock<IAccountService>();
            //    var historyRepositoryMock = new Mock<IHistoryRepository>();
            //    var transactionsMapperMock = new Mock<ITransactionsMapper>();

            //    var mapperMock = new Mock<IMapper>();
            //    mapperMock.Setup(m => m.Map<GetTransactionDto>(transaction)).Returns(mappedTransactionDto);

            //    var service = new TransactionService(
            //    transactionRepositoryMock.Object,
            //    accountRepositoryMock.Object,
            //    currencyRepositoryMock.Object,
            //    exchangeRateServiceMock.Object,
            //    accountServiceMock.Object,
            //    historyRepositoryMock.Object,
            //    transactionCheckerServiceMock.Object,
            //    mapperMock.Object);

            //    // Act
            //    var result = await service.GetByIdAsync(transactionId, loggedUser);

            //    // Assert
            //    Assert.IsFalse(result.IsSuccessful);
            //    Assert.IsNotNull(result.Message);

            //}

        [TestMethod]
        public async Task CreateOutTransactionAsync_ValidTransaction_ReturnsSuccessfulResponseWithData()
        {
            // Arrange
        var transactionDto = GetCreateTransactionDto();
        var loggedUser = GetLoggedUser();
        var loggedUserCurrency = "BGN";
        var recipient = GetAccountRecipient();
        var currency = GetCurrency();
        var exchangeRate = GerExchangeRateCorrect();
        var transaction = GetTransaction();
        var getTransactionDto = GetTransactionDto();

            //var accountRepositoryMock = new Mock<IAccountRepository>();
            //accountRepositoryMock.Setup(repo => repo.GetByUsernameAsync(transactionDto.RecipientUsername)).ReturnsAsync(recipient);

            //var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            //currencyRepositoryMock.Setup(repo => repo.GetByCurrencyCodeAsync(transactionDto.CurrencyCode)).ReturnsAsync(currency);

        var exchangeRateServiceMock = new Mock<IExchangeRateService>();
        exchangeRateServiceMock
                .Setup(service => service
                .GetExchangeRateAsync(transactionDto.CurrencyCode, loggedUserCurrency))
                .ReturnsAsync(exchangeRate);

            //var transactionCheckerMock = new Mock<ITransactionCheckerService>();
            //transactionCheckerMock
            //        .Setup(checker => checker
            //        .ChecksCreateOutTransactionAsync(transactionDto, loggedUser, recipient, currency, exchangeRate))
            //        .ReturnsAsync(new Response<GetTransactionDto> { IsSuccessful = true });

        var transactionRepositoryMock = new Mock<ITransactionRepository>();
        transactionRepositoryMock
                .Setup(repo => repo
                .CreateTransactionAsync(transaction))
                .ReturnsAsync(transaction);

            //            var mapperMock = new Mock<IMapper>();
            //            mapperMock.Setup(m => m.Map<GetTransactionDto>(transaction)).Returns(getTransactionDto);

            //            var accountServiceMock = new Mock<IAccountService>();

            //            var service = new TransactionService(
            //            transactionRepositoryMock.Object,
            //            accountRepositoryMock.Object,
            //            currencyRepositoryMock.Object,
            //            exchangeRateServiceMock.Object,
            //            accountServiceMock.Object,
            //            historyRepositoryMock.Object,
            //            transactionCheckerServiceMock.Object,
            //          //  transactionsMapperMock.Object,
            //            mapperMock.Object);


            //// Act
            //var result = await service.CreateOutTransactionAsync(transactionDto, loggedUser);

            // Act
            var result = await service.CreateOutTransactionAsync(transactionDto, loggedUser);

            // Assert
            Assert.IsNotNull(result.Data);
        }

       


    }   
}
