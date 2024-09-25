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
                .Include(p => p.Course)
                .ThenInclude(c => c.Categories)
                .Select(p => p.Course) 
            .ToListAsync();

            return purchasedCourses;
        }

        public async Task<Course> GetPurchasedCourse(long userId, long courseId)
        {
            try
            {
                var purchase = await _context.Purchases
                    .Include(p => p.Course)
                    .ThenInclude(c => c.Author)
                    .Include(p => p.Course)
                    .ThenInclude(c => c.Videos)
                    .Include(p => p.Course)
                    .ThenInclude(c => c.Categories)
                    .FirstOrDefaultAsync(p => p.User.Id == userId && p.Course.Id == courseId);

                if(purchase == null)
                {
                    return null;
                }

                return purchase.Course;


            }catch(Exception ex)
            {
                throw new RepositoryException("An unexpected error occured while retrieving course.");
            }
        }

        public async Task<List<Purchase>> CreateMultiplePurchases(CreatePurchaseModel purchaseModel)
        {
            try
            {
                var user = await _context.BasicUsers.FindAsync(purchaseModel.UserId);
                if (user == null)
                {
                    throw new RepositoryException("Cannot find user with the given id");
                }

                var courses = await _context.Course
                    .Include(c => c.Author)
                    .Where(c => purchaseModel.CoursesIds.Contains(c.Id))
                    .ToListAsync();

                // Create and save purchases
                var purchases = new List<Purchase>();
                foreach (var course in courses)
                {
                    var purchase = new Purchase
                    {
                        User = user,
                        Course = course,
                        PaymentId = purchaseModel.PaymentId,
                        PayerId = purchaseModel.PayerId,
                        PaymentMethod = purchaseModel.PaymentMethod
                    };

                    purchases.Add(purchase);
                }

                await _context.Purchases.AddRangeAsync(purchases);
                await _context.SaveChangesAsync();

                // Create AuthorEarnings
                var authorEarnings = new List<AuthorEarning>();
                foreach (var purchase in purchases)
                {
                    var course = courses.First(c => c.Id == purchase.Course.Id);
                    var authorEarning = new AuthorEarning
                    {
                        PurchaseId = purchase.Id, 
                        AuthorId = course.Author.Id,
                        Amount = (decimal)course.Price * 0.70m,
                        IsIncludedInPayout = false
                    };

                    authorEarnings.Add(authorEarning);
                }

                await _context.AuthorEarning.AddRangeAsync(authorEarnings);
                await _context.SaveChangesAsync();

                return purchases;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


    }
}
