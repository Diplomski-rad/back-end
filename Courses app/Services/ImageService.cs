using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Courses_app.Services
{
    public class ImageService : IImageService
    {
        public async Task<string> Upload(IFormFile file)
        {
            

            try
            {
                // Path
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // File name
                var uniqueFileName = Guid.NewGuid().ToString() + ".jpg";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Resize and save
                using (var image = await Image.LoadAsync(file.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(640, 360)
                    }));

                    await image.SaveAsync(filePath, new JpegEncoder { Quality = 85 });
                }

                if (File.Exists(filePath))
                {
                    return uniqueFileName;
                }
                else
                {
                    return null;
                }



            }
            catch (Exception ex)
            {
                throw;
            }

            
        }
    }
}
