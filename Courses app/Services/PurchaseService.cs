using Courses_app.Dto;
using Courses_app.Models;
using Courses_app.Repository;

namespace Courses_app.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;

        public PurchaseService(IPurchaseRepository purchaseRepository, IUserService userService, ICourseService courseService)
        {
            _purchaseRepository = purchaseRepository;
            _userService = userService;
            _courseService = courseService;
        }

        public async Task<long> CreatePurchase(CreatePurchaseModel model)
        {

            BasicUser user = await _userService.GetBasicUser(model.UserId);
            Course course = await _courseService.Get(model.CourseId);

            Purchase purchase = new Purchase();
            purchase.User = user;
            purchase.Course = course;

            return await _purchaseRepository.Add(purchase);
        }

        public async Task<List<PurchasedCourse>> GetPurchasedCourses(long userId)
        {
            var purchasedCourses = await _purchaseRepository.GetPurchasedCourses(userId);

            var purchasedCourseDtos = purchasedCourses
                .Select(course => new PurchasedCourse(course))
                .ToList();

            return purchasedCourseDtos;
        }
    }
}
