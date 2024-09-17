using System.ComponentModel.DataAnnotations;

namespace BoulderBuddy.Models
{
    public class GradingSystems
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; } = "";
    }
}
