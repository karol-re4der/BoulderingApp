﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoulderBuddy.Models
{
    public class Routes
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public string Image { get; set; } = "";
        public DateTime AddDateTime { get; set; }

        [ForeignKey("Grades")]
        public int GradeId { get; set; }
        [ForeignKey("Gyms")]
        public int GymId { get; set; }
    }
}
