using Courses_app.Models;

namespace Courses_app.Dto
{
    public class CourseRatingsDto
    {
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public Dictionary<int, int> RatingBreakdown { get; set; } = new Dictionary<int, int>();

        public CourseRatingsDto()
        {
            
        }

        public CourseRatingsDto(List<Rating> ratings)
        {
            if (!ratings.Any())
            {
                AverageRating = 0.0;
                TotalRatings = 0;
                RatingBreakdown = new Dictionary<int, int>();
            }
            else
            {
                AverageRating = Math.Round(ratings.Average(r => r.RatingValue), 1);
                TotalRatings = ratings.Count();
                RatingBreakdown =  ratings
                    .GroupBy(r => r.RatingValue)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
        }

    }
}
