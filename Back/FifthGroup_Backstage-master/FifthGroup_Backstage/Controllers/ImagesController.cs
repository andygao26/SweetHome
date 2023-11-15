using FifthGroup_Backstage.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FifthGroup_Backstage.Controllers
{
        [Route("api/[controller]")]
        [ApiController]


    public class ImagesController : Controller
    {

        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }
        [HttpPost]
        public async Task<IActionResult> UploadAaync(IFormFile file)
        {
            //call a repository
            var imageURL = await imageRepository.UploadAsync(file);
            if (imageURL == null)
            {
                return Problem("Something went wrong!", null, (int)HttpStatusCode.InternalServerError);

            }
            return new JsonResult(new { link = imageURL });
        }
    }
}
