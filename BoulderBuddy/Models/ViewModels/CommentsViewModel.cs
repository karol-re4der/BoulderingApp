namespace BoulderBuddy.Models.ViewModels
{
    public class CommentsViewModel(UserData userData, RouteComments comment)
    {
        public UserData UserData { get; set; } = userData;
        public RouteComments Comment { get; set; } = comment;
    }
}
