using Microsoft.AspNetCore.Http;

namespace DataAccess.Models.Contracts
{
    public interface IUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int PhoneNumber { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsAdmin { get; set; }
        public string ProfilePhotoPath { get; set; }
        public string ProfilePhotoFileName { get; set; }
        public IFormFile ImageFile { get; set; }

        // public Card WalletCard { get; set; }

        // public List<Card> Cards { get; set; } = new List<Card> { };
        
        //public List<Transfer> Transfers { get; set; } = new List<Transfer> { };
        
        //public List<Transaction> Transactions { get; set; } = new List<Transaction> { };

    }
}
