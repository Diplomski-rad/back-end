namespace Courses_app.Dto
{
    public class CreatePurchaseModel
    {
        public long UserId { get; set; }
        public List<long> CoursesIds { get; set;}
        public string PaymentId { get; set; }
        public string PayerId { get; set; }
        public string PaymentMethod { get; set; }
    }
}
