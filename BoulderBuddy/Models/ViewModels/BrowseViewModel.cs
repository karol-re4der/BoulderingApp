namespace BoulderBuddy.Models.ViewModels
{
    public class BrowseViewModel(List<Routes> routes)
    {
        public List<Routes> Routes { get; set; } = routes;

        public int ColumnsPerPage { get; set; } = 4;
        public int RowsPerPage { get; set; } = 2;
        public int PagesTotal { get; set; } = 1;
    }
}
