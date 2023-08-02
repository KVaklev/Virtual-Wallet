using DataAccess.Models.Models;

namespace DataAccess.Repositories.Helpers
{
    public static class Common<T>
    {
        
        public static async Task<IQueryable<T>> PaginateAsync(IQueryable<T> result, int pageNumber, int pageSize)
        {
            return await Task.FromResult(result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize));
        }
        public static async Task<bool> IsAdminAsync(User loggedUser)
        {
            if (!loggedUser.IsAdmin)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }
    }
}
