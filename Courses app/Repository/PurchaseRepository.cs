using Courses_app.Models;
using Microsoft.EntityFrameworkCore;

namespace Courses_app.Repository
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly CoursesAppDbContext _context;
        public PurchaseRepository(CoursesAppDbContext context)
        {
            _context = context;
        }

        public async Task<long> Add(Purchase purchase)
        {
            try
            {
                _context.Purchases.Add(purchase);
                var purchaseId = await _context.SaveChangesAsync();
                return purchaseId;
            }catch (DbUpdateException ex) {
                throw;
            }
        }

        public async Task<List<Course>> GetPurchasedCourses(long userId)
        {
            var purchasedCourses = await _context.Purchases
                .Where(p => p.User.Id == userId)
                .Include(p => p.Course) // Include the Course itself first
                .ThenInclude(c => c.Author) // Include the Author of the Course
                .Include(p => p.Course) // Include the Course itself again for the Videos navigation
                .ThenInclude(c => c.Videos) // Include the Videos of the Course
                .Select(p => p.Course) // Now you can select the Course
        .ToListAsync();

            return purchasedCourses;
        }
    }
}
