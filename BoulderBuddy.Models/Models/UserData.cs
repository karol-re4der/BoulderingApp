using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoulderBuddy.Models.Models
{
    public class UserData
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AspNetUsers")]
        public string AspNetUserId { get; set; } = "";

        public string DisplayName { get; set; } = "";
    }
}
