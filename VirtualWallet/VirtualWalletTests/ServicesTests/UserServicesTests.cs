using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using static VirtualWalletTests.TestHelpers.TestHelpers;
using Business.Services.Models;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;
using Business.Services.Contracts;
using Business.DTOs.Responses;
using Business.Services.Helpers;
using Moq;
using Business.QueryParameters;

namespace VirtualWalletTests.ServicesTests
{
    [TestClass]
    public class UserServicesTests
    {
        [TestMethod]
        public async Task GetById_Should_ReturnCorrectUser_When_ParametersAreValidAsync()
        {
            //Arrange
            var expectedUserDto = GetTestUserDto();
            var loggedUser = GetTestUserAdmin();
            var expectedUser = GetTestUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetByIdAsync(expectedUser.Id))
                .ReturnsAsync(expectedUser);
            mapperMock.Setup(mapper => mapper
                .Map<GetUserDto>(expectedUser))
                .Returns(expectedUserDto);
            securityWrapperMock
                .Setup(security => security
                .IsAuthorizedAsync(expectedUser, loggedUser))
                .ReturnsAsync(true);

            var sut = new UserService(userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.GetByIdAsync(expectedUser.Id, loggedUser);

            // Assert
            Assert.AreEqual(expectedUserDto, actualResponse.Data);
        }

        [TestMethod]
        public async Task GetById_Should_ReturnUnsuccessfulResult_When_NotFound()
        {
            //Arrange
            var loggedUser = GetTestUserAdmin();
            var expectedUser = GetTestUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            var sut = new UserService(userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.GetByIdAsync(expectedUser.Id, loggedUser);

            // Assert
            Assert.IsFalse(actualResponse.IsSuccessful);
        }

        [TestMethod]
        public async Task GetById_Should_ReturnUnsuccessfulResult_When_LoggedUserIsNotAuthorized()
        {
            //Arrange
            var expectedUserDto = GetTestUserDto();
            var loggedUser = GetTestUserAdmin();
            var expectedUser = GetTestUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                 .Setup(repo => repo
                 .GetByIdAsync(expectedUser.Id))
                 .ReturnsAsync(expectedUser);
            mapperMock
                 .Setup(mapper => mapper
                 .Map<GetUserDto>(expectedUser))
                 .Returns(expectedUserDto);
            securityWrapperMock
                 .Setup(security => security
                 .IsAuthorizedAsync(expectedUser, loggedUser))
                 .ReturnsAsync(false);

            var sut = new UserService(userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.GetByIdAsync(expectedUser.Id, loggedUser);

            // Assert
            Assert.IsFalse(actualResponse.IsSuccessful); 

        }

        [TestMethod]
        public void GetAll_Should_ReturnListOfUsers_When_UsersExist()
        {
            // Arrange
            var expectedUsers = GetTestListUsers();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetAll())
                .Returns(expectedUsers.AsQueryable());

            var sut = new UserService(userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var actualResponse = sut.GetAll();

            // Assert
            Assert.AreEqual(expectedUsers.Count, actualResponse.Data.Count());
        }

        [TestMethod]
        public void GetAll_Should_ReturnUnsuccessfulResult_When_NoUsersExist()
        {
            // Arrange
            var expectedUsers = GetTestListUsers();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetAll())
                .Returns(new List<User>()
                .AsQueryable());

            var sut = new UserService(userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var actualResponse = sut.GetAll();

            // Assert
            Assert.IsFalse(actualResponse.IsSuccessful);
        }

        [TestMethod]
        public async Task FilterBy_Should_ReturnCorrectList_When_ParametersAreValid()
        {
            //Arrange
            var expectedUsers = GetTestListUsers();
            var filterParameters = new UserQueryParameters { Username = "ivanchoDraganchov", };

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetAll())
                .Returns(expectedUsers
                .AsQueryable());

            var sut = new UserService(userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.FilterByAsync(filterParameters);

            //Assert
            Assert.IsTrue(actualResponse.IsSuccessful);
        }

        //[TestMethod]
        //public async Task FilterBy_Should_ReturnMessageResult_When_NoUsersExist()
        //{
        //    // Arrange
        //    var expectedUsers = GetTestListUsers();
        //    var filterParameters = new UserQueryParameters { Username = "anatolssssss" };
        //    var expectedMessage = Constants.NoRecordsFound;

        //    var userRepositoryMock = new Mock<IUserRepository>();
        //    var accountServiceMock = new Mock<IAccountService>();
        //    var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
        //    var mapperMock = new Mock<IMapper>();
        //    var securityWrapperMock = new Mock<ISecurityService>();

        //    userRepositoryMock
        //        .Setup(repo => repo
        //        .GetAll())
        //        .Returns(new List<User>()
        //        .AsQueryable());

        //    var sut = new UserService(
        //        userRepositoryMock.Object,
        //        accountServiceMock.Object,
        //        mapperMock.Object,
        //        webHostEnvironmentMock.Object,
        //        securityWrapperMock.Object);

        //    // Act
        //    var actualResponse = await sut.FilterByAsync(filterParameters);

        //    // Assert
        //    //Assert.IsFalse(actualResponse.IsSuccessful);
        //    //Assert.AreEqual(expectedMessage, actualResponse.Message);
        //    Assert.Fail(expectedMessage);
        //}

        [TestMethod]
        public async Task GetByUsername_Should_ReturnUser_ParametersAreValid()
        {
            // Arrange
            var expectedUser = GetTestUser();
            var expectedUserDto = GetTestUserDto();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetByUsernameAsync(expectedUser.Username))
                .ReturnsAsync(expectedUser);
            mapperMock
                .Setup(mapper => mapper
                .Map<GetUserDto>(expectedUser))
                .Returns(expectedUserDto);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.GetByUsernameAsync(expectedUser.Username);

            // Assert
            Assert.IsTrue(actualResponse.IsSuccessful);
        }

        [TestMethod]
        public async Task GetByUsername_Should_ReturnUnsuccessfulResult_When_UserDoesNotExist()
        {
            // Arrange
            var nonExistingUsername = "nonexistingusername";

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetByUsernameAsync(nonExistingUsername))
                .ReturnsAsync((User)null);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.GetByUsernameAsync(nonExistingUsername);

            // Assert
            Assert.IsFalse(actualResponse.IsSuccessful);
            Assert.IsNull(actualResponse.Data);
            Assert.AreEqual(Constants.NoUsersErrorMessage, actualResponse.Message);
        }

    }
}
