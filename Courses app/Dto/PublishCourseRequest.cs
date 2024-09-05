using Courses_app.Models;
using Courses_app.Validation;
using System.ComponentModel.DataAnnotations;

namespace Courses_app.Dto
{
    public class PublishCourseRequest
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double Price { get; set; }

        [MinLength(3, ErrorMessage = "At least 3 categories are required.")]
        public List<CategoryDto> Categories { get; set; }

        [ValidDifficultyLevel(typeof(DifficultyLevel), ErrorMessage = "Invalid difficulty level.")]
        public string DifficultyLevel { get; set; }

        public PublishCourseRequest() { }
    }
}
