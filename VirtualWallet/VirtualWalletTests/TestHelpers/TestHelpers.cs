using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;
using DataAccess.Models.Enums;
using Business.Services.Helpers;

namespace VirtualWalletTests.TestHelpers
{
    public static class TestHelpers
    {
        //Constants for Tests

        public const int NonExistingUserId = 666;
        public const string WebRootPath = "D:\\Virtual Wallet\\VirtualWallet\\VirtualWallet\\wwwroot";

        public const string UsernameAfterFilter = "ivanchoDraganchov";
        public const string FirstNameAfterFilter = "Ivan";
        public const string LastNameAfterFilter = "Draganov";
        public const string EmailAfterFilter = "i.draganov@gmail.com";
        public const string PhoneNumberAfterFilter = "0897554285";

        public const string RandomUsername = "Username";
        public const string RandomPassword = "Password";


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
        public static CreateUserModel GetCreateUserModel()
        {
            return new CreateUserModel
            {

                //FirstName = "Dimitar",
                //LastName = "Peev",
                Email = "dim@gmail.com",
                Username = "dimitarDimitrov",
                PhoneNumber = "0887885778",

                //Email = "m.petrova@gmail.com",
                //Username = "mariicheto",
                //PhoneNumber = "0898568569",
                //Password = "pa^3ddwp;z",
                //CurrencyCode = "BGN", 

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
                },

                 new User
                {
                    Id = 4,
                    FirstName = "Andreq",
                    LastName = "Dobreva",
                    Email = "aaee331@gmail.com",
                    Username = "andreicheto",
                    Password = new byte[] { 0x65, 0x23, 0x25, 0x55 },
                    PhoneNumber = "0697556285",
                    IsAdmin = false,
                    IsBlocked = true
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
                Account = GetAccountSender(),
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
                Currency = GetCurrency(),
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
                UserId = 2,
                User = GetTestExpectedUserAsUnblocked()
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
                IsDeleted = false,
                Country="Bulgaria"
            };
        }

        public static List<Currency> GetListCurrency()
        {
            return new List<Currency>()
            {
                new Currency
                {
                Id = 1,
                Name = "Bulgarian Lev",
                CurrencyCode = "BGN",
                IsDeleted = false,
                Country = "Bulgaria"
                }
            };
        }

        public static CreateCurrencyDto GetCurrencyDto()
        {
            return new CreateCurrencyDto()
            {
                Name = "Bulgarian Lev",
                CurrencyCode = "BGN",
                Country = "Bulgaria"
            };
        }

        public static Currency GetNewCurrency()
        {
            return new Currency()
            {
                Id = 4,
                Name = "Bahamian Dollar",
                CurrencyCode = "BSD",
                Country = "BAHAMAS(THE)"
            };
        }

        public static List<Currency> GetListDelete()
        {
           return   new List<Currency>
            {
                new Currency { IsDeleted = true },
                new Currency { IsDeleted = false},
                new Currency { IsDeleted = true }
            };
        }
        //Helpers for TransactionServices Tests
        public static Transaction GetTransaction()
        {
            return new Transaction()
            {
                Id = 1,
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
                AmountExchange = 10
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

        //Helpers for TransferServices Tests
        public static Transfer GetTransferTest()
        {
            return new Transfer()
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
        }

        public static GetTransferDto GetTransferDtoTest()
        {
            return new GetTransferDto()
            {
                Id = 1,
                Username = "mariicheto",
                Amount = 10,
                CurrencyCode = "BGN",
                CardNumber = "1234567899875642",
                TransferType = TransferDirection.Deposit.ToString(),
                IsConfirmed = false,
                IsCancelled = false
            };
        }

        public static CreateTransferDto CreateTransferDtoTest()
        {
            return new CreateTransferDto()
            {
                Amount = 10,
                CurrencyCode = "BGN",
                CardNumber = "1234567899875642",
                TransferType = TransferDirection.Deposit.ToString(),

            };
        }

        public static Card GetCardTest()
        {
            return new Card()
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
        }



    }
}
