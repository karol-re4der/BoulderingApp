using System.ComponentModel.DataAnnotations;

namespace BoulderBuddy.Models.Models
{
    public class GradingSystems
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; } = "";
    }
}
