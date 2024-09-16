using System.ComponentModel.DataAnnotations;

namespace Courses_app.Models
{
    public class Video
    {
        [Key]
        public string Id { get; set; }
        public Author Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Thumbnail { get; set; }
        public bool IsPublished { get; set; }

        public Video(){}

        public Video(string id,Author author, string title, string description, bool isPublished)
        {
            Id = id;
            Author = author;
            Title = title;
            Description = description;
            IsPublished = isPublished;

        }
    }
}