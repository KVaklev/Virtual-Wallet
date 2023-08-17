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
                .ReturnsAsync((User)null);

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
    }
}
