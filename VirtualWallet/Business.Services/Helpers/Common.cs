using DataAccess.Models.Models;

namespace Business.Services.Helpers
{
    public static class Common
    {
        public static async Task<bool> IsAdminAsync(User loggedUser)
        {
            if (!loggedUser.IsAdmin)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public static async Task<bool> IsAuthorizedAsync(User user, User loggedUser)
        {
            bool isAuthorized = false;

            if (user.Id == loggedUser.Id || loggedUser.IsAdmin)
            {
                isAuthorized = true;
            }
            return await Task.FromResult(isAuthorized);
        }

        public static async Task<bool> IsAuthorizedAsync(Card card, User loggedUser)
        {
            bool isAuthorized = false;

            if (card.Account.User.Id == loggedUser.Id || loggedUser.IsAdmin)
            {
                isAuthorized = true;
            }
            return await Task.FromResult(isAuthorized);
        }

    }
}
