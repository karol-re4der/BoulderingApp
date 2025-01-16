using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BoulderBuddy.Models.Models.ViewModels
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
