using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using static VirtualWalletTests.TestHelpers.TestHelpers;
using Business.Services.Models;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;
using Business.Services.Contracts;
using Business.DTOs.Responses;
using Moq;
using Business.QueryParameters;
using Business.DTOs.Requests;
using static Business.Services.Helpers.Constants;
using Business.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Http;
using Business.Services.Helpers;

namespace VirtualWalletTests.ServicesTests
{
    [TestClass]
    public class UserServicesTests
    {
        [TestMethod]
        public async Task FindLoggedUser_Should_Return_When_ParametersAreValid()
        {
            // Arrange
            var userToFind = GetTestUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetByUsernameAsync(userToFind.Username))
                .ReturnsAsync(userToFind);

            var sut = new UserService(userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var result = await sut.FindLoggedUserAsync(userToFind.Username);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(userToFind, result.Data);
        }

        [TestMethod]
        public async Task FindLoggedUser_Should_ReturnMessage_When_UsernameIsNull()
        {
            // Arrange
            string username = null;

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
            var result = await sut.FindLoggedUserAsync(username);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task FindLoggedUser_Should_ReturnMessage_When_UserDoesNotExist()
        {
            // Arrange
            string username = RandomUsername;

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetByUsernameAsync(username))
                .ReturnsAsync((User?)null!);

            var sut = new UserService(userRepositoryMock.Object,
                 accountServiceMock.Object,
                 mapperMock.Object,
                 webHostEnvironmentMock.Object,
                 securityWrapperMock.Object);

            // Act
            var result = await sut.FindLoggedUserAsync(username);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(NoUsersErrorMessage, result.Message);
        }

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
            var filterParameters = new UserQueryParameters { Username = UsernameAfterFilter, };

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
            var nonExistingUsername = RandomUsername;

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetByUsernameAsync(nonExistingUsername))
                .ReturnsAsync((User?)null!);

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
            Assert.AreEqual(NoUsersErrorMessage, actualResponse.Message);
        }

        [TestMethod]
        public async Task Create_Should_When_ParametersAreValid()
        {
            //Arrange
            var createdUserDto = GetTestCreatedUserDto();
            var createdUser = GetCreateUserModel();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .UsernameExistsAsync(createdUserDto.Username))
                .ReturnsAsync(false);
            userRepositoryMock
                .Setup(repo => repo
                .EmailExistsAsync(createdUserDto.Email))
                .ReturnsAsync(false);
            userRepositoryMock
                .Setup(repo => repo
                .PhoneNumberExistsAsync(createdUserDto.PhoneNumber))
                .ReturnsAsync(false);
            userRepositoryMock
                .Setup(repo => repo
                .CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(new User());

            securityWrapperMock
                .Setup(security => security
                .ComputePasswordHashAsync<CreateUserModel>(createdUserDto, It.IsAny<User>()))
                .ReturnsAsync(new User());

            mapperMock.Setup(mapper => mapper
                .Map<GetCreatedUserDto>(It.IsAny<User>()))
                .Returns(new GetCreatedUserDto());

            var sut = new UserService(
               userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.CreateAsync(createdUser);

            // Assert
            Assert.IsNotNull(actualResponse.Data);
        }

        [TestMethod]
        public async Task Create_Should_ReturnResult_When_UsernameExists()
        {
            //Arrange
            var createdUserDto = GetTestCreatedUserDto();
            var createdUser = GetCreateUserModel();
            var user = GetTestCreateUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .UsernameExistsAsync(createdUserDto.Username))
                .ReturnsAsync(true);

            userRepositoryMock
                 .Setup(repo => repo
                 .CreateAsync(It.IsAny<User>()))
                 .ReturnsAsync(user);

            mapperMock.Setup(mapper => mapper
                .Map<GetCreatedUserDto>(It.IsAny<User>()))
                .Returns(createdUserDto);

            var sut = new UserService(
               userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.CreateAsync(createdUser);

            // Assert
            Assert.AreEqual(UsernameExistsErrorMessage, actualResponse.Message);
            Assert.AreEqual(PropertyName.Username, actualResponse.Error.InvalidPropertyName);
        }

        [TestMethod]
        public async Task Create_Should_ReturnResult_When_EmailExists()
        {
            //Arrange
            var createdUserDto = GetTestCreatedUserDto();
            var createdUser = GetCreateUserModel();
            var user = GetTestCreateUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .EmailExistsAsync(createdUserDto.Email))
                .ReturnsAsync(true);

            userRepositoryMock
                 .Setup(repo => repo
                 .CreateAsync(It.IsAny<User>()))
                 .ReturnsAsync(user);

            mapperMock.Setup(mapper => mapper
                .Map<GetCreatedUserDto>(It.IsAny<User>()))
                .Returns(createdUserDto);

            var sut = new UserService(
               userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.CreateAsync(createdUser);

            // Assert
            Assert.AreEqual(EmailExistsErrorMessage, actualResponse.Message);
            Assert.AreEqual(PropertyName.Email, actualResponse.Error.InvalidPropertyName);
        }

        [TestMethod]
        public async Task Create_Should_ReturnResult_When_PhoneNumberExists()
        {
            //Arrange
            var createdUserDto = GetTestCreatedUserDto();
            var createdUser = GetCreateUserModel();
            var user = GetTestCreateUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .PhoneNumberExistsAsync(createdUserDto.PhoneNumber))
                    .ReturnsAsync(true);
            userRepositoryMock
                 .Setup(repo => repo
                 .CreateAsync(It.IsAny<User>()))
                 .ReturnsAsync(user);

            mapperMock.Setup(mapper => mapper
                .Map<GetCreatedUserDto>(It.IsAny<User>()))
                .Returns(createdUserDto);

            var sut = new UserService(
               userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.CreateAsync(createdUser);

            // Assert
            Assert.AreEqual(PhoneNumberExistsErrorMessage, actualResponse.Message);
            Assert.AreEqual(PropertyName.PhoneNumber, actualResponse.Error.InvalidPropertyName);
        }

        [TestMethod]
        public async Task Update_Should_When_ParametersAreValid()
        {
            //Arrange
            UpdateUserDto updateUserDto = GetTestUpdateUserDto();
            var loggedUser = GetTestCreateUser();
            var existingUser = GetTestCreateUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetByIdAsync(existingUser.Id))
                .ReturnsAsync(existingUser);

            securityWrapperMock
                .Setup(security => security
                .IsAuthorizedAsync(existingUser, It.IsAny<User>()))
                .ReturnsAsync(true);

            userRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(existingUser);

            mapperMock.Setup(mapper => mapper
                .Map<GetUpdatedUserDto>(It.IsAny<User>()))
                .Returns(new GetUpdatedUserDto());

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.UpdateAsync(existingUser.Id, updateUserDto, loggedUser);

            // Assert
            Assert.IsTrue(actualResponse.IsSuccessful);
        }

        [TestMethod]
        public async Task Update_Should_ReturnResult_When_UserIsNotFound()
        {
            //Arrange
            UpdateUserDto updateUserDto = GetTestUpdateUserDto();
            var loggedUser = GetTestCreateUser();
            var existingUser = GetTestCreateUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetByIdAsync(existingUser.Id))
                .ReturnsAsync((User?)null!);

            userRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(existingUser);

            mapperMock.Setup(mapper => mapper
                .Map<GetUpdatedUserDto>(It.IsAny<User>()))
                .Returns(new GetUpdatedUserDto());

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.UpdateAsync(existingUser.Id, updateUserDto, loggedUser);

            // Assert
            Assert.IsFalse(actualResponse.IsSuccessful);
            Assert.AreEqual(NoUsersErrorMessage, actualResponse.Message);
        }

        [TestMethod]
        public async Task Update_Should_When_LoggedUserUpdatesHisProfile()
        {
            // Arrange
            UpdateUserDto updateUserDto = GetTestUpdateUserDto();
            var loggedUser = GetTestCreateUser();
            var existingUser = GetTestCreateUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetByIdAsync(existingUser.Id))
                .ReturnsAsync(existingUser);
            userRepositoryMock
                .Setup(repo => repo
                .UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(existingUser);
           
            mapperMock.Setup(mapper => mapper
                .Map<GetUpdatedUserDto>(It.IsAny<User>()))
                .Returns(new GetUpdatedUserDto());

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.UpdateAsync(existingUser.Id, updateUserDto, loggedUser);

            // Assert
            Assert.IsFalse(actualResponse.IsSuccessful);
        }

        [TestMethod]
        public async Task Update_Should_ReturnResult_When_PhoneNumberExists()
        {
            // Arrange
            UpdateUserDto updateUserDto = GetTestUpdateUserDto();
            var loggedUser = GetTestCreateUser();
            var existingUser = GetTestCreateUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetByIdAsync(existingUser.Id))
                .ReturnsAsync(existingUser);
            userRepositoryMock
                .Setup(repo => repo
                .UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(existingUser);
            userRepositoryMock
                .Setup(repo => repo.PhoneNumberExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            mapperMock.Setup(mapper => mapper
                .Map<GetUpdatedUserDto>(It.IsAny<User>()))
                .Returns(new GetUpdatedUserDto());
            securityWrapperMock
                .Setup(security => security
                .IsAuthorizedAsync(It.IsAny<User>(), It.IsAny<User>()))
                .ReturnsAsync(true);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.UpdateAsync(existingUser.Id, updateUserDto, loggedUser);

            // Assert
             // Assert
            Assert.AreEqual(PhoneNumberExistsErrorMessage, actualResponse.Message);
            Assert.AreEqual(PropertyName.PhoneNumber, actualResponse.Error.InvalidPropertyName);
        }

        [TestMethod]
        public async Task Update_Should_ReturnResult_When_EmailExists()
        {
            // Arrange
            UpdateUserDto updateUserDto = GetTestUpdateUserDto();
            var loggedUser = GetTestCreateUser();
            var existingUser = GetTestCreateUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetByIdAsync(existingUser.Id))
                .ReturnsAsync(existingUser);
            userRepositoryMock
                .Setup(repo => repo
                .UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(existingUser);
            userRepositoryMock
                .Setup(repo => repo.EmailExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            mapperMock.Setup(mapper => mapper
                .Map<GetUpdatedUserDto>(It.IsAny<User>()))
                .Returns(new GetUpdatedUserDto());
            securityWrapperMock
                .Setup(security => security
                .IsAuthorizedAsync(It.IsAny<User>(), It.IsAny<User>()))
                .ReturnsAsync(true);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.UpdateAsync(existingUser.Id, updateUserDto, loggedUser);

            // Assert
            Assert.AreEqual(EmailExistsErrorMessage, actualResponse.Message);
        }

        [TestMethod]
        public async Task ChangeStatus_Should_When_ParametersAreValid()
        {
            // Arrange
            var createdUserDto = GetTestCreatedUserDto();
            var createdUser = GetCreateUserModel();
            var user = GetTestCreateUser();
            var userDetailsViewModel = new UserDetailsViewModel
            {
                User = new GetUserDto
                {
                    Admin = true,
                    Blocked = true
                }
            };

            User loggedUser = GetTestUserAdmin();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security
                .IsAdminAsync(loggedUser))
                .ReturnsAsync(true);

            userRepositoryMock
                .Setup(repo => repo
                .GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            userRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(true);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.ChangeStatusAsync(user.Id, userDetailsViewModel, loggedUser);

            // Assert
            Assert.IsNotNull(actualResponse.IsSuccessful);
        }

        [TestMethod]
        public async Task ChangeStatus_Should_ReturnMessage_WhenLoggedUserIsNotAdmin()
        {
            // Arrange
            var userToUpdate = GetTestCreateUser();
            User loggedUser = GetTestUserAdmin();
            var userDetailsViewModel = new UserDetailsViewModel
            {
                User = new GetUserDto
                {
                    Admin = true,
                    Blocked = true
                }
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security
                .IsAdminAsync(loggedUser))
                .ReturnsAsync(false);

            userRepositoryMock
                .Setup(repo => repo
                .GetByIdAsync(userToUpdate.Id))
                .ReturnsAsync(userToUpdate);

            userRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(true);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.ChangeStatusAsync(userToUpdate.Id, userDetailsViewModel, loggedUser);

            // Assert
            Assert.IsNotNull(actualResponse.IsSuccessful);
        }

        [TestMethod]
        public async Task ChangeProfilePicture_Should_When_ParametersAreValid()
        {
            // Arrange
            var userToUpdate = GetTestCreateUser();
            var loggedUser = GetTestCreateUser();
            UserDetailsViewModel userDetailsViewModel = new UserDetailsViewModel
            {
                User = new GetUserDto
                {
                    ImageFile = new FormFile(new MemoryStream(new byte[0]), 0, 0, "ImageFile", "image.png"),
                }
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(userToUpdate.Id))
                .ReturnsAsync(userToUpdate);
            webHostEnvironmentMock
           .Setup(env => env.WebRootPath)
           .Returns("D:\\Virtual Wallet\\VirtualWallet\\VirtualWallet\\wwwroot");

            var sut = new UserService(
                 userRepositoryMock.Object,
                 accountServiceMock.Object,
                 mapperMock.Object,
                 webHostEnvironmentMock.Object,
                 securityWrapperMock.Object);

            // Actt
            var actualResponse = await sut.ChangeProfilePictureAsync(userToUpdate.Id, userDetailsViewModel, loggedUser);

            // Assert

            Assert.IsTrue(actualResponse.IsSuccessful);
        }

        [TestMethod]
        public async Task ChangeProfilePicture_Should_ReturnMessage_When_UserDoesNotExist()
        {
            // Arrange
            var nonExistingUserId = NonExistingUserId; 
            UserDetailsViewModel userDetailsViewModel = new UserDetailsViewModel
            {
                User = new GetUserDto
                {
                    ImageFile = new FormFile(new MemoryStream(new byte[0]), 0, 0, "ImageFile", "image.png"),
                }
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(nonExistingUserId))
                .ReturnsAsync((User?)null!);
            webHostEnvironmentMock
                .Setup(env => env.WebRootPath)
                .Returns(WebRootPath);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var actualResponse = await sut.ChangeProfilePictureAsync(nonExistingUserId, userDetailsViewModel, null);

            // Assert
            Assert.IsFalse(actualResponse.IsSuccessful);
            Assert.AreEqual(NoUsersErrorMessage, actualResponse.Message);
        }

        [TestMethod]
        public async Task Delete_Should__When_ParametersAreValid()
        {
            // Arrange
            var userToDelete = GetTestUser(); 
            var loggedUser = GetTestUserAdmin();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security
                .IsAdminAsync(loggedUser))
                .ReturnsAsync(true);
            userRepositoryMock
                .Setup(repo => repo
                .GetByIdAsync(userToDelete.Id))
                .ReturnsAsync(userToDelete);
            userRepositoryMock
                .Setup(repo => repo.DeleteAsync(userToDelete.Id))
                .ReturnsAsync(true);

            var sut = new UserService(
               userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var result = await sut.DeleteAsync(userToDelete.Id, loggedUser);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsTrue(result.Data);
        }

        [TestMethod]
        public async Task Delete_Should_ReturnMessage_When_UserToDeleteDoesNotExist()
        {
            // Arrange
            var nonExistingUserId = NonExistingUserId; 
            var loggedUser = GetTestUserAdmin();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security
                .IsAdminAsync(loggedUser))
                .ReturnsAsync(true);
            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(nonExistingUserId))
                .ReturnsAsync((User?)null!);

            var sut = new UserService(
                 userRepositoryMock.Object,
                 accountServiceMock.Object,
                 mapperMock.Object,
                 webHostEnvironmentMock.Object,
                 securityWrapperMock.Object);

            // Act
            var result = await sut.DeleteAsync(nonExistingUserId, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(NoUsersErrorMessage, result.Message);
        }

        [TestMethod]
        public async Task Delete_Should_ReturnMessage_When_UserIsNotAdmin()
        {
            // Arrange
            var userToDelete = GetTestUser();
            var loggedUser = GetTestUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security
                .IsAdminAsync(loggedUser))
                .ReturnsAsync(false);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var result = await sut.DeleteAsync(userToDelete.Id, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(ModifyUserErrorMessage, result.Message);
        }

        [TestMethod]
        public async Task Promote_Should__When_ParametersAreValid()
        {
            // Arrange
            var loggedUser = GetTestUserAdmin(); 
            var userToPromote = GetTestUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security
                .IsAdminAsync(loggedUser))
                .ReturnsAsync(true);
            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(userToPromote.Id))
                .ReturnsAsync(userToPromote);
            userRepositoryMock
                .Setup(repo => repo.PromoteAsync(userToPromote.Id))
                .ReturnsAsync(userToPromote); 
            mapperMock
                .Setup(mapper => mapper.Map<GetUserDto>(userToPromote))
                .Returns(new GetUserDto());

            var sut = new UserService(
               userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var result = await sut.PromoteAsync(userToPromote.Id, loggedUser);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNotNull(result.Data);
        }

        [TestMethod]
        public async Task Promote_Should_ReturnMessage_When_UserIsNotAdmin()
        {
            // Arrange
            var loggedUser = GetTestUser(); 
            var userToPromote = GetTestUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security
                .IsAdminAsync(loggedUser))
                .ReturnsAsync(false);

            var sut = new UserService(
               userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var result = await sut.PromoteAsync(userToPromote.Id, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(ModifyUserErrorMessage, result.Message);
        }

        [TestMethod]
        public async Task Promote_Should_ReturnMessage_When_UserToPromoteDoesNotExist()
        {
            // Arrange
            var nonExistingUserId = NonExistingUserId;
            var loggedUser = GetTestUserAdmin();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security.IsAdminAsync(loggedUser))
                .ReturnsAsync(true); 
            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(nonExistingUserId))
                .ReturnsAsync((User?)null!);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var result = await sut.PromoteAsync(nonExistingUserId, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(NoUsersErrorMessage, result.Message);
        }

        [TestMethod]
        public async Task Block_Should__When_ParametersAreValid()
        {
            // Arrange
            var loggedUser = GetTestUserAdmin();
            var userToBlock = GetTestUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security
                .IsAdminAsync(loggedUser))
                .ReturnsAsync(true);
            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(userToBlock.Id))
                .ReturnsAsync(userToBlock);
            userRepositoryMock
                .Setup(repo => repo.BlockUserAsync(userToBlock.Id))
                .ReturnsAsync(userToBlock);
            mapperMock
                .Setup(mapper => mapper.Map<GetUserDto>(userToBlock))
                .Returns(new GetUserDto());

            var sut = new UserService(
               userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var result = await sut.BlockUserAsync(userToBlock.Id, loggedUser);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
        }

        [TestMethod]
        public async Task Block_Should_ReturnMessage_When_UserIsNotAdmin()
        {
            // Arrange
            var loggedUser = GetTestUser();
            var userToBlock = GetTestUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security
                .IsAdminAsync(loggedUser))
                .ReturnsAsync(false);

            var sut = new UserService(
               userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var result = await sut.BlockUserAsync(userToBlock.Id, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(ModifyUserErrorMessage, result.Message);
        }

        [TestMethod]
        public async Task Block_Should_ReturnMessage_When_UserToBlockDoesNotExist()
        {
            // Arrange
            var nonExistingUserId = NonExistingUserId;
            var loggedUser = GetTestUserAdmin();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security.IsAdminAsync(loggedUser))
                .ReturnsAsync(true);
            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(nonExistingUserId))
                .ReturnsAsync((User?)null!);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var result = await sut.BlockUserAsync(nonExistingUserId, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(NoUsersErrorMessage, result.Message);
        }

        [TestMethod]
        public async Task Unblock_Should__When_ParametersAreValid()
        {
            // Arrange
            var loggedUser = GetTestUserAdmin();
            var userToUnblock = GetTestExpectedUserAsBlocked();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security
                .IsAdminAsync(loggedUser))
                .ReturnsAsync(true);
            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(userToUnblock.Id))
                .ReturnsAsync(userToUnblock);
            userRepositoryMock
                .Setup(repo => repo.UnblockUserAsync(userToUnblock.Id))
                .ReturnsAsync(userToUnblock);
            mapperMock
                .Setup(mapper => mapper.Map<GetUserDto>(userToUnblock))
                .Returns(new GetUserDto());

            var sut = new UserService(
               userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var result = await sut.UnblockUserAsync(userToUnblock.Id, loggedUser);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
        }

        [TestMethod]
        public async Task Unblock_Should_ReturnMessage_When_UserIsNotAdmin()
        {
            // Arrange
            var loggedUser = GetTestUser();
            var userToUnblock = GetTestExpectedUserAsBlocked();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security
                .IsAdminAsync(loggedUser))
                .ReturnsAsync(false);

            var sut = new UserService(
               userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var result = await sut.UnblockUserAsync(userToUnblock.Id, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(ModifyUserErrorMessage, result.Message);
        }

        [TestMethod]
        public async Task Unblock_Should_ReturnMessage_When_UserToBlockDoesNotExist()
        {
            // Arrange
            var nonExistingUserId = NonExistingUserId;
            var loggedUser = GetTestUserAdmin();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            securityWrapperMock
                .Setup(security => security.IsAdminAsync(loggedUser))
                .ReturnsAsync(true);
            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(nonExistingUserId))
                .ReturnsAsync((User?)null!);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var result = await sut.UnblockUserAsync(nonExistingUserId, loggedUser);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(NoUsersErrorMessage, result.Message);
        }

        [TestMethod]
        public async Task Login_Should_When_ParametersAreValid()
        {
            // Arrange
            string username = RandomUsername;
            string password = RandomPassword;
            var existingUser = GetTestCreateUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo
                .GetByUsernameAsync(username))
                .ReturnsAsync(existingUser);
            securityWrapperMock
                .Setup(security => security
                .AuthenticateAsync(existingUser, password))
                .ReturnsAsync(new Response<User> { IsSuccessful = true, Data = existingUser });

            var sut = new UserService(
               userRepositoryMock.Object,
               accountServiceMock.Object,
               mapperMock.Object,
               webHostEnvironmentMock.Object,
               securityWrapperMock.Object);

            // Act
            var result = await sut.LoginAsync(username, password);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(existingUser, result.Data);
        }

        [TestMethod]
        public async Task Login_Should_ReturnMessage_When_UsernameAndPasswordAreNull()
        {
            // Arrange
            string username = null;
            string password = null;

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var result = await sut.LoginAsync(username, password);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
        }

        [TestMethod]
        public async Task Login_Should_ReturnMessage_When_UsernameDoesNotExist()
        {
            // Arrange
            string username = RandomUsername;
            string password = RandomPassword;

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
               .Setup(repo => repo
               .GetByUsernameAsync(username))
               .ReturnsAsync((User?)null!); 

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var result = await sut.LoginAsync(username, password);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(UsernameDoesntExistErrorMessage, result.Message);
        }

        [TestMethod]
        public async Task GetLoggedUserByUsername_Should_Return_When_ParametersAreValid()
        {
            // Arrange
            var userToGet = GetTestCreateUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
               .Setup(repo => repo
               .GetByUsernameAsync(userToGet.Username))
               .ReturnsAsync(userToGet);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var result = await sut.GetLoggedUserByUsernameAsync(userToGet.Username);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(userToGet, result.Data);
        }

        [TestMethod]
        public async Task GetLoggedUserByUsername_Should_ReturnMessage_When_UserDoesNotExist()
        {
            // Arrange
            var userToGet = GetTestCreateUser();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
               .Setup(repo => repo
               .GetByUsernameAsync(userToGet.Username))
               .ReturnsAsync((User?)null!);

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var result = await sut.GetLoggedUserByUsernameAsync(userToGet.Username);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(NoUsersErrorMessage, result.Message);
        }

        [TestMethod]
        public async Task FilterByAsync_Should_ReturnMessage_When_GetAllFails()
        {
            // Arrange
            var usersList = GetTestListUsers();
            var filterParameters = new UserQueryParameters();

            var userRepositoryMock = new Mock<IUserRepository>();
            var accountServiceMock = new Mock<IAccountService>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var mapperMock = new Mock<IMapper>();
            var securityWrapperMock = new Mock<ISecurityService>();

            userRepositoryMock
                .Setup(repo => repo.GetAll())
                .Returns(Enumerable.Empty<User>().AsQueryable());

            var sut = new UserService(
                userRepositoryMock.Object,
                accountServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                securityWrapperMock.Object);

            // Act
            var result = await sut.FilterByAsync(filterParameters);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
        }

        [TestMethod]
        public async Task FilterBy_Should_ReturnCorrectList_When_FilterByFirstName()
        {
            //Arrange
            var expectedUsers = GetTestListUsers();
            var filterParameters = new UserQueryParameters { FirstName = FirstNameAfterFilter, };

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

        [TestMethod]
        public async Task FilterBy_Should_ReturnCorrectList_When_FilterByLastName()
        {
            //Arrange
            var expectedUsers = GetTestListUsers();
            var filterParameters = new UserQueryParameters { LastName = LastNameAfterFilter, };

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

        [TestMethod]
        public async Task FilterBy_Should_ReturnCorrectList_When_FilterByPhoneNumber()
        {
            //Arrange
            var expectedUsers = GetTestListUsers();
            var filterParameters = new UserQueryParameters { PhoneNumber = PhoneNumberAfterFilter, };

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

        [TestMethod]
        public async Task FilterBy_Should_ReturnCorrectList_When_FilterByEmail()
        {
            //Arrange
            var expectedUsers = GetTestListUsers();
            var filterParameters = new UserQueryParameters { Email = EmailAfterFilter, };

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

        [TestMethod]
        public async Task FilterBy_Should_ReturnCorrectList_When_FilterByBlockedStatus()
        {
            //Arrange
            var expectedUsers = GetTestListUsers();
            var filterParameters = new UserQueryParameters { Blocked = true, };

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

        [TestMethod]
        public async Task FilterBy_Should_ReturnCorrectList_When_FilterByAdminStatus()
        {
            //Arrange
            var expectedUsers = GetTestListUsers();
            var filterParameters = new UserQueryParameters { Admin = true, };

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

        [TestMethod]
        public async Task FilterBy_Should_ReturnCorrectList_When_FilterSortByUsername()
        {
            //Arrange
            var expectedUsers = GetTestListUsers();
            var filterParameters = new UserQueryParameters { SortBy = SortByUsernameDuringFilter, };

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

        [TestMethod]
        public async Task FilterBy_Should_ReturnCorrectList_When_FilterSortByEmail()
        {
            //Arrange
            var expectedUsers = GetTestListUsers();
            var filterParameters = new UserQueryParameters { SortBy = SortByEmailDuringFilter, };

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

        [TestMethod]
        public async Task FilterBy_Should_ReturnCorrectList_When_FilterSortByPhoneNumber()
        {
            //Arrange
            var expectedUsers = GetTestListUsers();
            var filterParameters = new UserQueryParameters { SortBy = SortByPhoneNumberDuringFilter, };

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

        [TestMethod]
        public async Task FilterBy_Should_ReturnCorrectList_When_FilterSortSortOrderDesc()
        {
            //Arrange
            var expectedUsers = GetTestListUsers();
            var filterParameters = new UserQueryParameters { SortOrder = SortOrderDescDuringFilter, };

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

    }
}
