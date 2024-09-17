using System.ComponentModel.DataAnnotations;

namespace Courses_app.Dto
{
    public class CreateCategoryModel
    {
        [Required(ErrorMessage = "Category name is required.")]
        public string Name { get; set; }

        public CreateCategoryModel()
        {
            
        }
    }
}
