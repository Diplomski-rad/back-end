namespace Courses_app.Models
{
    public class Course
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Author Author { get; set; }
        public ICollection<Video> Videos { get; set; } = new List<Video>(); 
        public double Price { get; set; }
        public string PlaylistId { get; set; }
        public CourseStatus Status { get; set; }
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public DifficultyLevel DifficultyLevel { get; set; }

        public Course(){}

        public Course(string name, string description, Author author, double price, string playlistId, CourseStatus status, DifficultyLevel difficultyLevel)
        {
            Name = name;
            Description = description;
            Author = author;
            Price = price;
            PlaylistId = playlistId;
            Status = status;
            DifficultyLevel = difficultyLevel;
        }

    }
}
