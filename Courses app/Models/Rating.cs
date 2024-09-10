using Courses_app.Dto;

namespace Courses_app.Models
{
    public class Rating
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public int RatingValue { get; set; }
        public DateTime RatingDate { get; set; }

        public User User { get; set; }
        public Course Course { get; set; }

        public string? Review { get; set; }

        public Rating()
        {
            
        }

        public Rating(RatingDto ratingDto)
        {
            UserId = ratingDto.UserId;
            CourseId = ratingDto.CourseId;
            RatingValue = ratingDto.RatingValue;
            RatingDate = DateTime.UtcNow;
            Review = ratingDto.Review;
        }
    }
}
