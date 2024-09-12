namespace Courses_app.Models
{
    public class Payout
    {
        public long Id { get; set; }
        public decimal Amount { get; set; } 
        public DateTime? PayoutDate { get; set; }
        public PayoutStatus Status { get; set; } 

        public long AuthorId { get; set; }
        public Author Author { get; set; }

        public string? ControlGuid { get; set; }

        public List<AuthorEarning> AuthorEarnings { get; set; }

        public string? Payout_item_id { get; set; }
        public string? Payout_batch_id { get; set; }

        public Payout(){}

        public Payout(List<AuthorEarning> authorEarnings)
        {
            if (authorEarnings == null || authorEarnings.Count == 0)
            {
                throw new ArgumentException("Author earnings cannot be null or empty", nameof(authorEarnings));
            }

            AuthorEarnings = authorEarnings;
            Author = authorEarnings[0].Author;
            AuthorId = authorEarnings[0].AuthorId;
            Status = PayoutStatus.Pending;
            Amount = authorEarnings.Sum(ae => ae.Amount);
            ControlGuid = Guid.NewGuid().ToString();
        }
    }
}
