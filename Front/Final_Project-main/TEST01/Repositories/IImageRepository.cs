namespace FifthGroup_front.Repositories
{
    public interface IImageRepository
    {
        Task<string> UploadAsync(IFormFile file);
    }
}
