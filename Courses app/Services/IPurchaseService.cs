using Courses_app.Dto;
using Courses_app.Models;

namespace Courses_app.Services
{
    public interface IPurchaseService
    {
        public Task<List<CourseDto>> GetPurchasedCourses(long userId);
        public Task<List<Purchase>> CreateMultiplePurchases(CreatePurchaseModel purchaseModels);
        public Task<CourseDto> GetPurchasedCourse(long userId, long courseId);
    }
}
