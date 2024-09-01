namespace Courses_app.Models
{
    public class Author : User
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PayPalEmail { get; set; }
    }
}
