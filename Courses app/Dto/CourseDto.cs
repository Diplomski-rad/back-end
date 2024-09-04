using Courses_app.Models;

namespace Courses_app.Dto
{
    public class CourseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public AuthorDto Author { get; set; }
        public List<VideoDto> Videos { get; set; }
        public string Status { get; set; }
        public List<CategoryDto> Categories { get; set; }

        public CourseDto(Course course)
        {
            Id = course.Id;
            Name = course.Name;
            Description = course.Description;
            Price = course.Price;
            Author = new AuthorDto(course.Author);
            Videos = course.Videos.Select(video => new VideoDto(video)).ToList();
            Status = course.Status.ToString();
            Categories = course.Categories.Select(category => new CategoryDto(category)).ToList();
        }
    }

}
