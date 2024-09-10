namespace Courses_app.Services
{
    public interface IImageService
    {
        public Task<string> Upload(IFormFile file);
    }
}
