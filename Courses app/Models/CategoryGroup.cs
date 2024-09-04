namespace Courses_app.Models
{
    public class CategoryGroup
    {
        public long Id { get; set; } 
        public string Name { get; set; }
        public ICollection<Category> Categories { get; set;} = new List<Category>();
    }
}
