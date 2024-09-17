namespace BoulderBuddy.Models.DB
{
    public class CommentData(Users user, RouteComments comment)
    {
        public Users User { get; set; } = user;
        public RouteComments Comment { get; set; } = comment;
    }
}
