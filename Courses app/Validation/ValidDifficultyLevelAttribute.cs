using System.ComponentModel.DataAnnotations;

namespace Courses_app.Validation
{
    public class ValidDifficultyLevelAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public ValidDifficultyLevelAttribute(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type must be an enum.", nameof(enumType));
            }

            _enumType = enumType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Difficulty level is required.");
            }

            var valueAsString = value.ToString();
            if (!Enum.IsDefined(_enumType, valueAsString))
            {
                return new ValidationResult($"Invalid value for {_enumType.Name} enum.");
            }

            return ValidationResult.Success;
        }
    }
}
