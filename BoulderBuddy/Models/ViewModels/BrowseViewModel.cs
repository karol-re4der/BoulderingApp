using BoulderBuddy.Models.Filters;

namespace BoulderBuddy.Models.ViewModels
{
    public class PagingModel()
    {
        public int ColumnsPerPage { get; set; } = 4;
        public int RowsPerPage { get; set; } = 2;
        public int PagesTotal { get; set; } = 1;

        public int CurrentPage { get; set; } = 1;
    }
    public class BrowseViewModel()
    {
        public BrowseFilters Filters { get; set; }
        public Gyms TestFilter { get; set; }

        public List<Routes> Routes { get; set; }

        public PagingModel PagingModel { get; set; }
        
    }
}
