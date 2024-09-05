namespace Courses_app.Models
{
    public class Purchase
    {
        public long Id { get; set; }
        public Course Course { get; set; }
        public BasicUser User { get; set; }
        public string PaymentId { get; set; }
        public string PayerId { get; set; }
        public string PaymentMethod { get; set; }
    }
}
