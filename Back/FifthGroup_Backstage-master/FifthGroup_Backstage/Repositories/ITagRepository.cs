using FifthGroup_Backstage.Models;

namespace FifthGroup_Backstage.Repositories
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag?> GetAsync(int id);
        Task<Tag> AddAsync(Tag tag);
        Task<Tag?> UpdateAsync(Tag tag);
        Task<Tag?> DeleteAsync(int id);
    }
}
