using System.ComponentModel.DataAnnotations;

namespace Courses_app.Dto
{
    public class CreateCategoryGroupModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        public CreateCategoryGroupModel()
        {
            
        }
    }
}
