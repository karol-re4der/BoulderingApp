using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BoulderBuddy.Models.Models.ViewModels
{
    public class UpsertRoutesetterViewModel
    {
        [Key]
        public RouteSetters RouteSetter { get; set; }

        [ValidateNever]
        public IFormFile PreviewImage { get; set; }
    }
}
