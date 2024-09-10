using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;


namespace Courses_app.Controllers
{
    [ApiController]
    [Route("api/image")]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ImageController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Uploaded file is not an image.");

            // Create the path for storing the image
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate a unique file name
            var uniqueFileName = Guid.NewGuid().ToString() + ".jpg";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Load the image using ImageSharp
            using (var image = await Image.LoadAsync(file.OpenReadStream()))
            {
                // Resize the image to 360p (640x360 for 16:9 aspect ratio)
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(640, 360) // 360p resolution
                }));

                // Save the resized image to the server (JPEG format to save space)
                await image.SaveAsync(filePath, new JpegEncoder { Quality = 85 }); // You can adjust the quality to save more space
            }

            if (System.IO.File.Exists(filePath))
            {
                
                var fileUrl = Path.Combine("/images", uniqueFileName).Replace("\\", "/");
                return Ok(new { FileUrl = fileUrl });
            }
            else
            {
                return StatusCode(500);
            }

            // Return the file URL or path
            

        }
    }
}
