using FifthGroup_Backstage.Models;

namespace FifthGroup_Backstage.Repositories
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
