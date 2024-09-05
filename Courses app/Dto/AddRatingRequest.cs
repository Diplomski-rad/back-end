using System.ComponentModel.DataAnnotations;

namespace Courses_app.Dto
{
    public class AddRatingRequest
    {
        public long CourseId { get; set; }
        [Range(1, 5, ErrorMessage = "Rating value must be between 1 and 5.")]
        public int RatingValue { get; set; }

        public AddRatingRequest()
        {
            
        }
    }

}
