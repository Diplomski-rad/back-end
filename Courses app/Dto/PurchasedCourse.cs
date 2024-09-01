namespace Courses_app.Dto
{
    public class PurchasedCourse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public PurchasedCourseAuthor Author { get; set; }
        public List<PurchasedCourseVideo> Videos { get; set; }

        public PurchasedCourse(Courses_app.Models.Course course)
        {
            Id = course.Id;
            Name = course.Name;
            Description = course.Description;
            Price = course.Price;
            Author = new PurchasedCourseAuthor
            {
                Name = course.Author.Name,
                Surname = course.Author.Surname,
                Username = course.Author.Username
            };
            Videos = course.Videos.Select(video => new PurchasedCourseVideo
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description
            }).ToList();
        }
    }

    public class PurchasedCourseVideo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class PurchasedCourseAuthor
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
    }
}
