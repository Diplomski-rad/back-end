using Courses_app.Models;
using System.ComponentModel.DataAnnotations;

namespace Courses_app.Dto
{
    public class RatingDto
    {
        public long UserId { get; set; }
        public long CourseId { get; set; }
        [Range(1, 5, ErrorMessage = "Rating value must be between 1 and 5.")]
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
