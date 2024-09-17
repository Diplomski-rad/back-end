using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Courses_app.Models
{
    public class CategoryGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } 
        public string Name { get; set; }
        public ICollection<Category> Categories { get; set;} = new List<Category>();

        public CategoryGroup()
        {
            
        }
    }
}
