namespace BoulderBuddy.Models.ViewModels
{
    public class RouteViewModel(Routes route, List<CommentsViewModel> comments, List<AscentsViewModel> ascents)
    {
        public Routes Route { get; set; } = route;
        public List<CommentsViewModel> RouteComments { get; set; } = comments;
        public List<AscentsViewModel> AscentData { get; set; } = ascents;
    }
}
