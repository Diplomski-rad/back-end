using Courses_app.Dto;

namespace Courses_app.Services
{
    public interface ICategoryService
    {
        public Task<List<CategoryGroupDto>> GetAllCategoryGroups();
    }
}
