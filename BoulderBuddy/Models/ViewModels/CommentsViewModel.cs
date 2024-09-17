namespace BoulderBuddy.Models.ViewModels
{
    public class CommentsViewModel(Users user, RouteComments comment)
    {
        public Users User { get; set; } = user;
        public RouteComments Comment { get; set; } = comment;
    }
}
