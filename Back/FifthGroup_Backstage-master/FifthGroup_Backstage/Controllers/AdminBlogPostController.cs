using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.Repositories;
using FifthGroup_Backstage.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FifthGroup_Backstage.Controllers
{
    public class AdminBlogPostController : Controller
    {
        private readonly ITagRepository tagRepository;
        private readonly IBlogPostRepository blogPostRepository;

        public AdminBlogPostController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
        {
            this.tagRepository = tagRepository;
            this.blogPostRepository = blogPostRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var tags = await tagRepository.GetAllAsync();
            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
            };
            //get tags from repository
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            var selectedTags = new List<Tag>();
            //Map view modle to domain model
            var blogPost = new BlogPost
            {
                Heading = addBlogPostRequest.Heading,
                PageTitle = addBlogPostRequest.PageTitle,
                Content = addBlogPostRequest.Content,
                ShortDescription = addBlogPostRequest.ShortDescription,
                FeacturedImageUrl = addBlogPostRequest.FeacturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible,
            };
            //Map Tags from selected tags

            foreach (var selectedTagId in addBlogPostRequest.SelectedTags)
            {
                var selectedTagIdAsGuid = int.Parse(selectedTagId);
                var existingTag = await tagRepository.GetAsync(selectedTagIdAsGuid);

                if (existingTag != null)
                {
                    selectedTags.Add(existingTag);
                }

            }
            //Mapping tags back to domain model
            blogPost.Tags = selectedTags;
            await blogPostRepository.AddAsync(blogPost);

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            //call the repositoery to get data for us
            var blogPosts = await blogPostRepository.GetAllAsync();

            return View(blogPosts);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //Retrieve the result from the repository
            var blogPost = await blogPostRepository.GetAsync(id);
            var tagsDomainModle = await tagRepository.GetAllAsync();

            if (blogPost != null)
            {
                //map the domian model into the view model
                var model = new EditBlogPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    Author = blogPost.Author,
                    FeacturedImageUrl = blogPost.FeacturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    ShortDescription = blogPost.ShortDescription,
                    PublishedDate = blogPost.PublishedDate,
                    Visible = blogPost.Visible,
                    Tags = tagsDomainModle.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                    }),
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray(),
                };
                return View(model);
            }
            //pass data to view
            return RedirectToAction("List", "AdminBlogPost");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            // map view model back to domain model
            var blogPostDomainModel = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                PageTitle = editBlogPostRequest.PageTitle,
                Content = editBlogPostRequest.Content,
                Author = editBlogPostRequest.Author,
                ShortDescription = editBlogPostRequest.ShortDescription,
                FeacturedImageUrl = editBlogPostRequest.FeacturedImageUrl,
                PublishedDate = editBlogPostRequest.PublishedDate,
                UrlHandle = editBlogPostRequest.UrlHandle,
                Visible = editBlogPostRequest.Visible,

            };
            //Map tags into domain model
            var selectedTags = new List<Tag>();
            foreach (var selectedTag in editBlogPostRequest.SelectedTags)
            {
                if (int.TryParse(selectedTag, out var tag))
                {
                    var foundTag = await tagRepository.GetAsync(tag);

                    if (foundTag != null)
                    {
                        selectedTags.Add(foundTag);

                    }

                }

            }

            blogPostDomainModel.Tags = selectedTags;
            //submit information to repository to update
            var updatedBlog = await blogPostRepository.UpdateAsync(blogPostDomainModel);
            if (updatedBlog != null)
            {
                //show success notification
                return RedirectToAction("Edit");

            }
            //show error notification
            return RedirectToAction("Edit");

        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            //Talk to repositor to delete this blog post and tags 
            var deletedBlogPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);
            if (deletedBlogPost != null)
            {
                //show success notifications
                return RedirectToAction("List");


            }
            //Show error notification
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });

        }


    }
}

