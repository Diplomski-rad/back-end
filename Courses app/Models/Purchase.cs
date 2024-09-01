namespace Courses_app.Models
{
    public class Purchase
    {
        public long Id { get; set; }
        public Course Course { get; set; }
        public BasicUser User { get; set; }
    }
}
