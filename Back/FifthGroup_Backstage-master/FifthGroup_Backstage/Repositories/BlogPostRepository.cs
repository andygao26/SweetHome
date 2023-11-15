using FifthGroup_Backstage.Models;
using Microsoft.EntityFrameworkCore;

namespace FifthGroup_Backstage.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly DbHouseContext dbHouseContext;

        public BlogPostRepository(DbHouseContext dbHouseContext)
        {
            this.dbHouseContext = dbHouseContext;
        }

        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await dbHouseContext.AddAsync(blogPost);
            await dbHouseContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(int id)
        {
            var existingBlog = await dbHouseContext.BlogPosts.FindAsync(id);
            if (existingBlog != null)
            {
                dbHouseContext.BlogPosts.Remove(existingBlog);
                await dbHouseContext.SaveChangesAsync();
                return existingBlog;

            }
            return null;
        }


        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await dbHouseContext.BlogPosts.Include(x => x.Tags).ToListAsync();
        }

        public async Task<BlogPost?> GetAsync(int id)
        {
            return await dbHouseContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await dbHouseContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);

        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var existingBlog = await dbHouseContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == blogPost.Id);
            if (existingBlog != null)
            {
                existingBlog.Id = blogPost.Id;
                existingBlog.Heading = blogPost.Heading;
                existingBlog.PageTitle = blogPost.PageTitle;
                existingBlog.Content = blogPost.Content;
                existingBlog.ShortDescription = blogPost.ShortDescription;
                existingBlog.Author = blogPost.Author;
                existingBlog.FeacturedImageUrl = blogPost.FeacturedImageUrl;
                existingBlog.UrlHandle = blogPost.UrlHandle;
                existingBlog.Visible = blogPost.Visible;
                existingBlog.PublishedDate = blogPost.PublishedDate;
                existingBlog.Tags = blogPost.Tags;

                await dbHouseContext.SaveChangesAsync();
                return existingBlog;

            }
            return null;
        }
    }
}
