using DataAccess.Models.Models;
using static Business.Services.Helpers.Constants;

namespace Business.Services.Helpers
{
    public static class Common
    {
        public async static Task<Response<bool>> CheckForNullEntryAsync(string username, string password)
        {
            var result = new Response<bool>();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                result.IsSuccessful = false;
                result.Message = CredentialsErrorMessage;
                result.Error = new Error(PropertyName.Credentials);
            }

            return await Task.FromResult(result);
        }

        public static async Task<bool> HasEnoughBalanceAsync(Account account, decimal amount)
        {
            if (account.Balance < amount)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public static async Task<bool> HasEnoughCardBalanceAsync(Card card, decimal amount)
        {
            if (card.Balance < amount)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

    }
    public static class Common<T>
    {
        public static async Task<IQueryable<T>> PaginateAsync(IQueryable<T> result, int pageNumber, int pageSize)
        {
            return await Task.FromResult(result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize));
        }
    }
}
