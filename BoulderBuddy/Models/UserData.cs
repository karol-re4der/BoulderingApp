using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoulderBuddy.Models
{
    public class UserData
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AspNetUsers")]
        public int AspNetUserId { get; set; }
    }
}
