namespace BoulderBuddy.Models.ViewModels
{
    public class AscentsViewModel(Users user, Ascents ascent)
    {
        public Ascents Ascent { get; set; } = ascent;
        public Users User { get; set; } = user;
    }
}
