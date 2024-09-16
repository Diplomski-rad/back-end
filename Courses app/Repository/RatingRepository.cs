using Courses_app.Dto;
using Courses_app.Exceptions;
using Courses_app.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Courses_app.Repository
{
    public class RatingRepository : IRatingRepository
    {

        private readonly CoursesAppDbContext _context;
        public RatingRepository(CoursesAppDbContext context)
        {

            _context = context;

        }
        public async Task<long> Add(Rating rating)
        {
            try
            {
                var res = _context.Rating.Where(r => r.UserId == rating.UserId && r.CourseId == rating.CourseId).FirstOrDefault();

                if(res == null)
                 {
                    _context.Rating.Add(rating);
                    await _context.SaveChangesAsync();
                    return rating.Id;
                }
                else
                {
                    res.RatingDate = DateTime.UtcNow;
                    res.RatingValue = rating.RatingValue;
                    await _context.SaveChangesAsync();

                    return res.Id;
                }
                
            }
            catch(DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException postgresEx && postgresEx.SqlState == "23503")
                {
                    throw new BadDataException("Foreign key violation: invalid Course or User", postgresEx);
                }

                throw new Exception(ex.Message, ex);
            }
        }

        //public async Task<CourseRatingsDto> GetRatingsForCourse(long courseId)
        //{
        //    try
        //    {
        //        List<Rating> ratings = await _context.Rating
        //            .Where(rating => rating.CourseId == courseId).ToListAsync();

        //        if (!ratings.Any())
        //        {
        //            return new CourseRatingsDto
        //            {
        //                CourseId = courseId,
        //                AverageRating = 0.0,
        //                TotalRatings = 0,
        //                RatingBreakdown = new Dictionary<int, int>()
        //            };
        //        }

        //        var totalRatings = ratings.Count;
        //        var averageRating = ratings.Average(r => r.RatingValue);
        //        var ratingBreakdown = ratings
        //            .GroupBy(r => r.RatingValue)
        //            .ToDictionary(g => g.Key, g => g.Count());

        //        return new CourseRatingsDto 
        //        { 
        //            CourseId = courseId,
        //            AverageRating = averageRating,
        //            TotalRatings = totalRatings,
        //            RatingBreakdown = ratingBreakdown 
        //        };

                

        //    }catch(Exception ex) {
        //        throw new RepositoryException("An error occured while retriving ratings for course.");
        //    }
        //}

        public async Task<Rating> GetUsersRatingForCourse(long userId, long courseId)
        {
            try
            {
                Rating rating = await _context.Rating.FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId && r.IsValid);
                return rating;

            }catch(Exception ex)
            {
                throw new RepositoryException("An error occured while retriving users rating for course");
            }
        }
    }
}
