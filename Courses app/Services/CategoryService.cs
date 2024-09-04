using Courses_app.Dto;
using Courses_app.Models;
using Courses_app.Repository;

namespace Courses_app.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryGroupDto>> GetAllCategoryGroups()
        {
            var categoryGroups = await _categoryRepository.GetAllCategoryGroups();

            return categoryGroups
                .Select(categoryGroup => new CategoryGroupDto(categoryGroup))
                .ToList();
        }
    }
}
