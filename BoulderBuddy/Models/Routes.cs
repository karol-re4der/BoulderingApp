using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoulderBuddy.Models
{
    public class Routes
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public string Image { get; set; } = "";

        [ForeignKey("Grades")]
        public int GradeId { get; set; }
    }
}
