namespace Courses_app.Dto
{
    public class SearchUserModel
    {
        public string Query { get; set; }
        public string Flag { get; set; }

        public SearchUserModel()
        {
            
        }
    }

    public enum SearchUserFlag{
        username,
        email
    }
}
