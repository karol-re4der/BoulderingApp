﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoulderBuddy.Models.Models
{
    public class Ascents
    {

        [Key]
        public int Id { get; set; }
        public DateTime AscentDateTime { get; set; }
        public bool Success { get; set; }


        [ForeignKey("Users")]
        public int UserId { get; set; }
        [ForeignKey("Routes")]
        public int RouteId { get; set; }
    }
}
