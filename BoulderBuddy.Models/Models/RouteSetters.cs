using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoulderBuddy.Models.Models
{
    public class RouteSetters
    {
        [Key]
        public int Id { get; set; }

        [ValidateNever]
        public string Image { get; set; } = "";

        [Required]
        public string SetterName { get; set; } = "";
    }
}
