using Courses_app.Models;

namespace Courses_app.Repository
{
    public interface ICategoryRepository
    {
        public Task<List<CategoryGroup>> GetAllCategoryGroups();
        public Task<List<Category>> GetCategories(List<long> ids);
    }
}
