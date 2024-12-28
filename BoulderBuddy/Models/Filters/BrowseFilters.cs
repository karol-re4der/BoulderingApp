using Microsoft.AspNetCore.Mvc.Rendering;

namespace BoulderBuddy.Models.Filters
{
    public class BrowseFilters
    {
        public List<SelectListItem> GymsAvailable { get; set; }
        public string GymSelected { get; set; } = "null";

        public List<SelectListItem> GradesAvailable { get; set; }
        public string GradeSelected { get; set; } = "null";

        public DateTime SettingDate_From { get; set; }
        public DateTime SettingDate_To { get; set; }

        public bool ShowAscents { get; set; }
        public bool ShowAttempts { get; set; }
        public bool ShowNotAttempted { get; set; }


    }
}
