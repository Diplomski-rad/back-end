namespace Courses_app.Dto
{
    public class CreatePurchaseModel
    {
        public long UserId { get; set; }
        public List<long> CoursesIds { get; set;}
    }
}
