using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;


namespace BoulderBuddy.Models.ViewModels
{
    public class UpsertRouteViewModel
    {
        public Routes Route { get; set; }

        [ValidateNever]
        public List<SelectListItem> GradesAvailable { get; set; }

        [ValidateNever]
        public List<SelectListItem> GymsAvailable { get; set; }

        [ValidateNever]
        public List<Tuple<int, int>> GymGradeTable { get; set; }

        [ValidateNever]
        public IFormFile PreviewImage { get; set; }
    }
}
