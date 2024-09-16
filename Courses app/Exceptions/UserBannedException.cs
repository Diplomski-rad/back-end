namespace Courses_app.Exceptions
{
    public class UserBannedException : Exception
    {
        public UserBannedException()
        {
            
        }

        public UserBannedException(string message) : base(message) { }

        public UserBannedException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
