using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoulderBuddy.Models
{
    public class Gyms
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        [ForeignKey("GradingSystems")]
        public int GradingSystemId { get; set; }
    }
}
