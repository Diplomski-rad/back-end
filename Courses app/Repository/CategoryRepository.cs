using Courses_app.Exceptions;
using Courses_app.Models;
using Microsoft.EntityFrameworkCore;

namespace Courses_app.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CoursesAppDbContext _context;

        public CategoryRepository(CoursesAppDbContext context)
        {
            _context = context;
        }


        public async Task<List<CategoryGroup>> GetAllCategoryGroups()
        {
            try
            {
                List<CategoryGroup> categoryGroups = await _context.CategoryGroup
                    .Include(cg => cg.Categories)
                    .OrderBy(cg => cg.Name)
                    .ToListAsync();

                // Sort Categories by name within each CategoryGroup
                foreach (var categoryGroup in categoryGroups)
                {
                    categoryGroup.Categories = categoryGroup.Categories
                        .OrderBy(c => c.Name)
                        .ToList();
                }

                return categoryGroups;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while retrieving category groups", ex);
            }
        }

        public async Task<List<Category>> GetCategories(List<long> ids)
        {
            try
            {
                List<Category> categories = await _context.Category
                    .Where(c => ids.Contains(c.Id)).ToListAsync();
                return categories;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while retrieving category groups", ex);
            }
        }
    }
}
