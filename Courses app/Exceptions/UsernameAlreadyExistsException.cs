namespace Courses_app.Exceptions
{
    public class UsernameAlreadyExistsException : Exception
    {
        public UsernameAlreadyExistsException(string message) : base(message) { }
    }
}
