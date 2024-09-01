using Courses_app.Dto;
using Courses_app.Models;

namespace Courses_app.Services
{
    public interface IPurchaseService
    {
        public Task<long> CreatePurchase(CreatePurchaseModel model);
        public Task<List<PurchasedCourse>> GetPurchasedCourses(long userId);
    }
}
