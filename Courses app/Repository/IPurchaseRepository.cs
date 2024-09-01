using Courses_app.Models;

namespace Courses_app.Repository
{
    public interface IPurchaseRepository
    {
        public Task<long> Add(Purchase purchase);
        public Task<List<Course>> GetPurchasedCourses(long userId);
    }
}
