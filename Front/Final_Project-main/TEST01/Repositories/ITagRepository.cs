
using FifthGroup_front.Models;

namespace FifthGroup_front.Repositories
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
