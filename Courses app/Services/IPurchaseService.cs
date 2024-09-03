using Courses_app.Dto;
using Courses_app.Models;

namespace Courses_app.Services
{
    public interface IPurchaseService
    {
        //public Task<long> CreatePurchase(CreatePurchaseModel model);
        public Task<List<CourseDto>> GetPurchasedCourses(long userId);
        public Task<List<Purchase>> CreateMultiplePurchases(CreatePurchaseModel purchaseModels);
    }
}
