

using FifthGroup_front.Models;

namespace FifthGroup_front.Repositories
{
    public interface IBlogPostRepository
    {
        Task<IEnumerable<BlogPost>> GetAllAsync();
        Task<BlogPost?> GetAsync(int id);
        Task<BlogPost?> GetByUrlHandleAsync(string urlHandle);

        Task<BlogPost> AddAsync(BlogPost blogPost);
        Task<BlogPost?> UpdateAsync(BlogPost blogPost);
        Task<BlogPost?> DeleteAsync(int id);
    }
}
