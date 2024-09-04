using Courses_app.Models;

namespace Courses_app.Dto
{
    public class CategoryGroupDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<CategoryDto> Categories { get; set; }

        public CategoryGroupDto(CategoryGroup categoryGroup)
        {
            Id = categoryGroup.Id;
            Name = categoryGroup.Name;
            Categories = categoryGroup.Categories.Select(category => new CategoryDto(category)).ToList();
        }
    }
}
