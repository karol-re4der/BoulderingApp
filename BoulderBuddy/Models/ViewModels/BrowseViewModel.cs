namespace BoulderBuddy.Models.ViewModels
{
    public class BrowseViewModel(List<Routes> routes)
    {
        public List<Routes> Routes { get; set; } = routes;
    }
}
