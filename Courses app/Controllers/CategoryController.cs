using Courses_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace Courses_app.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("category-groups")]
        public async Task<IActionResult> GetAllCategoryGroups()
        {
            try
            {
                var res = await _categoryService.GetAllCategoryGroups();
                return Ok(res);
            }catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
