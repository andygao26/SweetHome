
using FifthGroup_front.Models;
using Microsoft.EntityFrameworkCore;

namespace FifthGroup_front.Repositories
{
    public class TagRepository :ITagRepository
    {
        private readonly DbHouseContext dbHouseContext;

        public TagRepository(DbHouseContext dbHouseContext)
        {
            this.dbHouseContext = dbHouseContext;
        }

        public async Task<Tag> AddAsync(Tag tag)
        {
            await dbHouseContext.AddAsync(tag);
            await dbHouseContext.SaveChangesAsync();
            return tag;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await dbHouseContext.Tags.ToListAsync();
        }

        public async Task<Tag?> UpdateAsync(Tag tag)
        {
            var existingTag = await dbHouseContext.Tags.FindAsync(tag.Id);

            if (existingTag != null)
            {
                existingTag.Name = tag.Name;
                existingTag.DisplayName = tag.DisplayName;

                await dbHouseContext.SaveChangesAsync();
                return existingTag;

            }
            return null;
        }


        public async Task<Tag?> DeleteAsync(int id)
        {
            var existingTag = await dbHouseContext.Tags.FindAsync(id);

            if (existingTag != null)
            {
                dbHouseContext.Tags.Remove(existingTag);
                await dbHouseContext.SaveChangesAsync();
                return existingTag;
            }
            return null;
        }



        public async Task<Tag?> GetAsync(int id)
        {
            return await dbHouseContext.Tags.FirstOrDefaultAsync(x => x.Id == id);
        }


    }
}
