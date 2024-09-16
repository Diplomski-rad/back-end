using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Courses_app.Models
{
    public class Purchase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public Course Course { get; set; }
        public BasicUser User { get; set; }
        public string PaymentId { get; set; }
        public string PayerId { get; set; }
        public string PaymentMethod { get; set; }
        public AuthorEarning AuthorEarning { get; set; }
    }
}
