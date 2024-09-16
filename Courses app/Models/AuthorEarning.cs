using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Courses_app.Models
{
    public class AuthorEarning
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long PurchaseId { get; set; }

        public long AuthorId { get; set; }
        public Author Author { get; set; }

        public decimal Amount { get; set; }

        public bool IsIncludedInPayout { get; set; }
        public long? PayoutId { get; set; }
    }
}
