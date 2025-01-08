using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoulderBuddy.Models
{
    public class Routes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; } = "";

        [ValidateNever]
        public string Image { get; set; } = "";

        [Required]
        public DateTime AddDateTime { get; set; }

        [ForeignKey("Grades")]
        [Required]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage="No grade selected!")]
        public int GradeId { get; set; }

        [ForeignKey("Gyms")]
        [Required]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "No gym selected!")]
        public int GymId { get; set; }
    }
}
