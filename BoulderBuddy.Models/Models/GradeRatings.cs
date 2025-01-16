using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BoulderBuddy.Models.Models
{
    public class GradeRatings
    {
        [Key]
        public int Id { get; set; }
        public short Rating { get; set; }

        [ForeignKey("UserData")]
        public int UserDataId { get; set; }
        [ForeignKey("Routes")]
        public int RouteId { get; set; }
    }
}
