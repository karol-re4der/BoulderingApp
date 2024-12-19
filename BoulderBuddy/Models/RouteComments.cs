using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoulderBuddy.Models
{
    public class RouteComments
    {
        [Key]
        public int Id { get; set; }
        public DateTime CommentDateTime { get; set; }
        public string Comment { get; set; } = "";


        [ForeignKey("Routes")]
        public int RouteId { get; set; }
        [ForeignKey("UserData")]
        public int UserDataId { get; set; }
    }
}
