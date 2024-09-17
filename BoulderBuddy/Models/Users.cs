using System.ComponentModel.DataAnnotations;

namespace BoulderBuddy.Models
{
    public class Users
    {
        [Key]
        public int ID { get; set; }

        public string Email { get; set; } = "";

        public string Name { get; set; } = "";

        public string Password { get; set; } = "";
    }
}
