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

    }
}
