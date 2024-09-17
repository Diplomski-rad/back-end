using Courses_app.Dto;

namespace Courses_app.Services
{
    public interface ICategoryService
    {
        public Task<List<CategoryGroupDto>> GetAllCategoryGroups();
        public Task<long> AddCategoryToCategoryGroup(string categoryName, long categoryGroupId);
        public Task<long> AddCategoryGroup(string categryGroupName);
    }
}
