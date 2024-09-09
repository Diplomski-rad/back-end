using Courses_app.Validation;
using System.ComponentModel.DataAnnotations;

namespace Courses_app.Dto
{
    public class UpdateUserRequest
    {
        [NotEmptyIfNotNull]
        public string? Name { get; set; }

        [NotEmptyIfNotNull]
        public string? Surname { get; set; }

        [NotEmptyIfNotNull]
        public string? Username { get; set; }

        public UpdateUserRequest(){}
    }
}
