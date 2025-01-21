namespace BoulderBuddy.Models.Models.ViewModels
{
    public class RouteViewModel(Routes route, List<CommentsViewModel> comments)
    {
        public Routes Route { get; set; } = route;
        public List<CommentsViewModel> RouteComments { get; set; } = comments;

        public RouteSetters RouteSetter { get; set; }
        public Grades Grade { get; set; }
        public Gyms Gym { get; set; }

        public AscentsSectionViewModel AscentsSectionViewModel { get; set; }
        public GradingSectionViewModel GradingSectionViewModel { get; set; }

        public string ShareCode { get; set; }
    }
}
