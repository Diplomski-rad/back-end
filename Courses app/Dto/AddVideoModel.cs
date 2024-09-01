namespace Courses_app.Dto
{
    public class AddVideoModel
    {
        public IFormFile file { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public long authorId { get; set; }
        public long courseId { get; set; }
    }
}
