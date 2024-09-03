using Courses_app.Dto;
using Courses_app.Exceptions;
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
                .Include(p => p.Course) 
                .ThenInclude(c => c.Author) 
                .Include(p => p.Course) 
                .ThenInclude(c => c.Videos) 
                .Select(p => p.Course) 
            .ToListAsync();

            return purchasedCourses;
        }

        public async Task<List<Purchase>> CreateMultiplePurchases(CreatePurchaseModel purchaseModel)
        {
            var user = await _context.BasicUsers.FindAsync(purchaseModel.UserId);

            if(user == null)
            {
                throw new RepositoryException("Cannot find user ith given id");
            }


            var purchaseTasks = purchaseModel.CoursesIds.Select(async courseId => new Purchase
            {
                User = user,
                Course = await _context.Course.FindAsync(courseId)
            }).ToList();

            var purchases = await Task.WhenAll(purchaseTasks);

            _context.Purchases.AddRange(purchases);
            await _context.SaveChangesAsync();
            return purchases.ToList();
        }
    }
}
