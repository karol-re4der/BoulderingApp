namespace BoulderBuddy.Models.ViewModels
{
    public class RouteViewModel(Routes route, List<CommentsViewModel> comments)
    {
        public Routes Route { get; set; } = route;
        public List<CommentsViewModel> RouteComments { get; set; } = comments;

        public AscentsSectionViewModel AscentsSectionViewModel { get; set; }
        public GradingSectionViewModel GradingSectionViewModel { get; set; }
    }
}
