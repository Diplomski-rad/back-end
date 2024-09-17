using Courses_app.Dto;
using Courses_app.Exceptions;
using Courses_app.Services;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("category-group")]
        public async Task<IActionResult> AddCategoryGreoup([FromBody] CreateCategoryGroupModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var id = await _categoryService.AddCategoryGroup(model.Name);
                return Ok(id);

            }catch (Exception ex)
            {
                return StatusCode(500, "An error occured while creating category group");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("category-group/{id}")]
        public async Task<IActionResult> AddCategoryGreoup(long id, [FromBody] CreateCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var categoryId = await _categoryService.AddCategoryToCategoryGroup(model.Name, id);
                return Ok(categoryId);

            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occured while creating category group");
            }
        }
    }
}
