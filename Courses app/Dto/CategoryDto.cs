using Courses_app.Models;

namespace Courses_app.Dto
{
    public class CategoryDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public CategoryDto()
        {
            
        }
        public CategoryDto(Category category)
        {
            Id = category.Id;
            Name = category.Name;
        }
    }
}
