using Courses_app.Models;

namespace Courses_app.Dto
{
    public class RatingDto
    {
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public int RatingValue { get; set; }


        public RatingDto()
        {
            
        }

        public RatingDto(Rating rating)
        {
            UserId = rating.UserId;
            CourseId = rating.CourseId;
            RatingValue = rating.RatingValue;
        }
    }
}
