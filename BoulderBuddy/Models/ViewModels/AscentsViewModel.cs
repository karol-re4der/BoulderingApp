namespace BoulderBuddy.Models.ViewModels
{
    public class AscentsViewModel(UserData userData, Ascents ascent)
    {
        public Ascents Ascent { get; set; } = ascent;
        public UserData User { get; set; } = userData;
    }
}
