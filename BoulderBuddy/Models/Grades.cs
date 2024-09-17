using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoulderBuddy.Models
{
    public class Grades
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Difficulty { get; set; }
        [ForeignKey("GradingSystems")]
        public int GradingSystemId { get; set; }
    }
}
