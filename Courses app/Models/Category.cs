using Courses_app.Dto;

namespace Courses_app.Models
{
    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();

        public Category(){}

    }
}
