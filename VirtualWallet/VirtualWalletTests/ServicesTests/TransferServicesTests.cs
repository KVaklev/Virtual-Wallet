using AutoMapper;
using Business.DTOs.Responses;
using Business.Mappers;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Models;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using Moq;

namespace VirtualWalletTests.ServicesTests
{
    [TestClass]
    public class TransferServicesTests
    {
        [TestMethod]
        public async Task GetByIdAsync_ValidIdAndUser_ReturnsValidResponseWithData()
        {
            // Arrange
            int id = 1;
            User user = new User { IsAdmin = false }; // Assuming User class definition

            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                                 .ReturnsAsync(new Transfer { });

            var securityMock = new Mock<ISecurityService>();
            securityMock.Setup(security => security.IsUserAuthorizedAsync(It.IsAny<Transfer>(), user))
                        .ReturnsAsync(true);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<GetTransferDto>(It.IsAny<Transfer>()))
                      .Returns(new GetTransferDto { });

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
            // Additional assertions can be added based on your specific requirements
        }

        [TestMethod]
        public async Task GetByIdAsync_NullTransfer_ReturnsErrorResponse()
        {
            // Arrange
            int id = 1;
            User user = new User { IsAdmin = false }; 

            var transferRepositoryMock = new Mock<ITransferRepository>();
            transferRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                                 .ReturnsAsync((Transfer)null); 

            var securityMock = new Mock<ISecurityService>();
            securityMock.Setup(security => security.IsUserAuthorizedAsync(null, user))
                        .ReturnsAsync(false); 

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<GetTransferDto>(It.IsAny<Transfer>()))
                      .Returns(new GetTransferDto()); 

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
            Assert.AreEqual("No records found.", result.Message);
            Assert.IsNull(result.Data);
            
        }
    }
}
