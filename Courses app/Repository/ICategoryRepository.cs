using Courses_app.Models;

namespace Courses_app.Repository
{
    public interface ICategoryRepository
    {
        public Task<List<CategoryGroup>> GetAllCategoryGroups();
        public Task<List<Category>> GetCategories(List<long> ids);
        public Task<long> AddCategoryGroup(CategoryGroup categryGroup);
        public Task<long> AddCategoryToCategoryGroup(Category category, long categoryGroupId);
    }
}
