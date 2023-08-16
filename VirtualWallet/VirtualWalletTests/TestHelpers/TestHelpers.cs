using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Models;
using DataAccess.Models.Enums;
using Business.Services.Helpers;

namespace VirtualWalletTests.TestHelpers
{
    public static class TestHelpers
    {
        //Helpers for UserServices Tests
        public static User GetTestUser()
        {
            return new User
            {
                Id = 2,
                FirstName = "Mariq",
                LastName = "Petrova",
                Email = "m.petrova@gmail.com",
                Username = "mariicheto",
                Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                PhoneNumber = "0898568569",
                AccountId = 2,
                IsAdmin = false,
                IsBlocked = false,
                Address = "Kajmakchalan 1",
                City = "Plovdiv",
                Country = "Bulgaria",
            };
        }
        public static User GetTestUserAdmin()
        {
            return new User
            {
                Id = 1,
                FirstName = "Ivan",
                LastName = "Draganov",
                Email = "i.draganov@gmail.com",
                Username = "ivanchoDraganchov",
                Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                PhoneNumber = "0878558547",
                AccountId = 1,
                IsAdmin = true,
                IsBlocked = false,
                Address = "Blvd Patriarh Evtimii 72",
                City = "Sofia",
                Country = "Bulgaria",
            };
        }
        public static User GetTestCreateUser()
        {
            return new User
            {
                Id = 6,
                FirstName = "Dimitar",
                LastName = "Peev",
                Email = "dim@gmail.com",
                Username = "dimitarDimitrov",
                Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                PhoneNumber = "0887885778",
                AccountId = 6,
                IsAdmin = false,
                IsBlocked = false,
                Address = "50-52 Krum Popov",
                City = "Sofia",
                Country = "Bulgaria",
            };
        }
        public static GetUserDto GetTestUserDto()
        {
            return new GetUserDto
            {
                Id = 2,
                FirstName = "Mariq",
                LastName = "Petrova",
                Email = "m.petrova@gmail.com",
                Username = "mariicheto",
                PhoneNumber = "0898568569",
                Address = "Kajmakchalan 1",
                City = "Plovdiv",
                Country = "Bulgaria",
            };
        }
        public static GetCreatedUserDto GetTestCreatedUserDto()
        {
            return new GetCreatedUserDto
            {
                FirstName = "Dimitar",
                LastName = "Peev",
                Email = "dim@gmail.com",
                Username = "dimitarDimitrov",
                PhoneNumber = "0887885778",
            };
        }
        public static UpdateUserDto GetTestUpdateUserDto()
        {
            return new UpdateUserDto
            {
                Email = "test@gmail.com"
            };
        }
        public static User GetTestDeleteUser()
        {
            return new User
            {
                Id = 2,
                FirstName = "Mariq",
                LastName = "Petrova",
                Email = "m.petrova@gmail.com",
                Username = "mariicheto",
                Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                PhoneNumber = "0898568569",
                AccountId = 2,
                IsAdmin = false,
                IsBlocked = false,
                Address = "Kajmakchalan 1",
                City = "Plovdiv",
                Country = "Bulgaria",
            };
        }
        public static User GetTestUpdateUser()
        {
            return new User
            {
                Id = 2,
                FirstName = "Mariq",
                LastName = "Petrova",
                Email = "test@gmail.com",
                Username = "mariicheto",
                Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                PhoneNumber = "0897554285",
                IsAdmin = false,
                IsBlocked = false
            };
        }
        public static User GetTestExpectedUserAsAdmin()
        {
            return new User
            {
                Id = 2,
                FirstName = "Mariq",
                LastName = "Petrova",
                Email = "m.petrova@gmail.com",
                Username = "mariicheto",
                Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                PhoneNumber = "0897554285",
                IsAdmin = true,
                IsBlocked = false
            };
        }
        public static User GetTestExpectedUserAsBlocked()
        {
            return new User
            {
                Id = 2,
                FirstName = "Mariq",
                LastName = "Petrova",
                Email = "m.petrova@gmail.com",
                Username = "mariicheto",
                Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                PhoneNumber = "0897554285",
                IsAdmin = false,
                IsBlocked = true
            };
        }
        public static User GetTestExpectedUserAsUnblocked()
        {
            return new User
            {
                Id = 2,
                FirstName = "Mariq",
                LastName = "Petrova",
                Email = "m.petrova@gmail.com",
                Username = "mariicheto",
                Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                PhoneNumber = "0897554285",
                IsAdmin = false,
                IsBlocked = false
            };
        }
        public static User GetTestUpdateUserInfo()
        {
            return new User
            {
                Id = 2,
                FirstName = "Mareto",
                LastName = "Petrovka",
            };
        }
        public static List<User> GetTestListUsers()
        {
            return new List<User>()
            {
                new User
                {
                    Id = 1,
                    FirstName = "Ivan",
                    LastName = "Draganov",
                    Email = "i.draganov@gmail.com",
                    Username = "ivanchoDraganchov",
                    Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                    PhoneNumber = "0897556285",
                    IsAdmin = true,
                    IsBlocked = false
                },

                new User
                {
                    Id = 2,
                    FirstName = "Mariq",
                    LastName = "Petrova",
                    Email = "m.petrova@gmail.com",
                    Username = "mariicheto",
                    Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                    PhoneNumber = "0897554285",
                    IsAdmin = false,
                    IsBlocked = false
                },

                new User
                {
                    Id = 3,
                    FirstName = "Mara",
                    LastName = "Dobreva",
                    Email = "m.dobreva@gmail.com",
                    Username = "marcheto",
                    Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                    PhoneNumber = "0797556285",
                    IsAdmin = false,
                    IsBlocked = false
                }
            };
        }
        public static List<User> GetTestExpectedListUsers()
        {
            return new List<User>()
            {
                new User
                {
                    Id = 1,
                    FirstName = "Ivan",
                    LastName = "Draganov",
                    Email = "i.draganov@gmail.com",
                    Username = "ivanchoDraganchov",
                    Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                    PhoneNumber = "0897556285",
                    IsAdmin = true,
                    IsBlocked = false
                }
            };
        }
        public static List<GetUserDto> GetTestExpectedListDtoUsers()
        {
            return new List<GetUserDto>()
            {
                new GetUserDto
                {
                    FirstName = "Ivan",
                    LastName = "Draganov",
                    Email = "i.draganov@gmail.com",
                    Username = "ivanchoDraganchov",
                }
            };
        }

        public static User GetLoggedUser()
        {
            return new User
            {
                Id = 1,
                FirstName = "Ivan",
                LastName = "Draganov",
                Email = "i.draganov@gmail.com",
                Username = "ivanchoDraganchov",
                Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                PhoneNumber = "0878558547",
                AccountId = 1,
                IsAdmin = false,
                IsBlocked = false,
                Address = "Blvd Patriarh Evtimii 72",
                City = "Sofia",
                Country = "Bulgaria",
            };
        }

        //Helpers for AccountServices Tests
        public static Account GetAccountSender()
        {
            return new Account()
            {
                Id = 1,
                Balance = 1000,
                CurrencyId = 1,
                IsDeleted = false,
                DateCreated = DateTime.Now,
                UserId = 1
            };

        }

        public static Account GetAccountRecipient()
        {
            return new Account()
            {
                Id = 2,
                Balance = 1000,
                CurrencyId = 2,
                IsDeleted = false,
                DateCreated = DateTime.Now,
                UserId = 2
            };
        }
        //Helpers for CurrencyServices Tests
        public static Currency GetCurrency()
        {
            return new Currency()
            {
                Id = 1,
                Name = "Bulgarian Lev",
                CurrencyCode = "BGN",
                IsDeleted = false
            };
        }

        //Helpers for TransactionServices Tests
        public static Transaction GetTransaction()
        {
            return new Transaction()
            {
                Id=1,
                AccountSenderId = 1,
                AccountRecepientId = 2,
                Amount = 10,
                CurrencyId = 1,
                Description = "Test transaction.",
                IsConfirmed = false,
                IsDeleted = false,
                Date = DateTime.Now,
                Direction = DirectionType.Out,
                ExchangeRate = Constants.ExchangeRateDefault,
                AmountExchange=10
            };
        }

        public static GetTransactionDto GetTransactionDto()
        {
            return new GetTransactionDto()
            {
                Id = 1,
                SenderUsername = "ivanchoDraganchov",
                RecipientUsername = "mariicheto",
                Amount = 10,
                CurrencyCode = "BGN",
                Description = "Test transaction.",
                IsConfirmed = false,
                IsDeleted = false,
                Date = DateTime.Now,
                Direction = DirectionType.Out.ToString(),
                ExchangeRate = 1,
                AmountExchange = 10 * 1
            };
        }

        public static CreateTransactionDto GetCreateTransactionDto()
        {
            return new CreateTransactionDto()
            {
                RecipientUsername = "mariicheto",
                Description = "Test transaction.",
                Amount = 10,
                CurrencyCode = "BGN"
            };    
        }

        public static Response<decimal> GerExchangeRateCorrect()
        {
            return new Response<decimal>()
            {
                IsSuccessful = true,
                Data = Constants.ExchangeRateDefault
            };
        
        }

    }
}
