namespace BoulderBuddy.Models.DB
{
    public class BoulderRouteData(Routes route, List<CommentData> comments, List<AscentData> ascents)
    {
        public Routes Route { get; set; } = route;
        public List<CommentData> RouteComments { get; set; } = comments;
        public List<AscentData> AscentData { get; set; } = ascents;
    }
}
