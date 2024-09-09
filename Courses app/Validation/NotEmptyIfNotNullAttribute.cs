using System.ComponentModel.DataAnnotations;

namespace Courses_app.Validation
{
    public class NotEmptyIfNotNullAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return true; // Null is valid

            if (value is string str)
                return !string.IsNullOrWhiteSpace(str); // Check if the string is not empty

            return true; // For other types, return true by default
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} cannot be an empty string if provided.";
        }
    }
}
