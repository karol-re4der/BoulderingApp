using BoulderBuddy.DataAccess.Data;
using BoulderBuddy.Models.Models;
using BoulderBuddy.Models.Models.Filters;
using BoulderBuddy.Models.Models.ViewModels;
using BoulderBuddy.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace BoulderBuddy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnviroment;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            _webHostEnviroment = env;
            _db = dbContext;
            _logger = logger;
        }

        public IActionResult Browse(int page = 1)
        {
            BrowseFilters filters = loadFilters();
            PagingModel paging = loadDefaultPaging();
            return View(loadRoutesByPage(filters, paging));
        }

        public IActionResult LoadRoutes(BrowseFilters filters, int page = 1)
        {
            PagingModel paging = loadDefaultPaging();
            paging.CurrentPage = page;
            return PartialView("_Browse_RouteGrid", loadRoutesByPage(filters, paging));
        }

        public IActionResult Index()
        {
            return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Helpers
        private BrowseFilters loadFilters()
        {
            BrowseFilters filters = new BrowseFilters();

            filters.GymsAvailable = (
                from gym in _db.Gyms select new SelectListItem(gym.Name, gym.Id.ToString())
                ).ToList();
            filters.GymsAvailable.Insert(0, new SelectListItem("All Gyms", "null"));

            filters.GradesAvailable = (
                from grade in _db.Grades select new SelectListItem(grade.Name, grade.Id.ToString())
                ).ToList();
            filters.GradesAvailable.Insert(0, new SelectListItem("All Grades", "null"));

            return filters;
        }

        private PagingModel loadDefaultPaging()
        {
            PagingModel result = new PagingModel();
            result.CurrentPage = 1;

            return result;
        }

        private BrowseViewModel loadRoutesByPage(BrowseFilters filters, PagingModel paging)
        {
            //Load routes
            List<Routes> availableRoutes = (from route in _db.Routes.ToList()
                                                //join grade in _db.Grades.ToList() on route.GradeId equals grade.Id
                                                //join gradingSystem in _db.GradingSystems.ToList() on grade.GradingSystemId equals gradingSystem.Id
                                                //join gym in _db.Gyms.ToList() on gradingSystem.Id equals gym.GradingSystemId
                                            where (!filters.GymSelected.Equals("null") ? route.GymId == int.Parse(filters.GymSelected) : true)
                                            && (!filters.GradeSelected.Equals("null") ? route.GradeId == int.Parse(filters.GradeSelected) : true)
                                            select route).ToList<Routes>();

            BrowseViewModel result = new BrowseViewModel() { Routes = availableRoutes };
            result.Filters = filters;

            //Load paging settings if not loaded
            if (paging == null)
            {
                paging = loadDefaultPaging();
            }
            int perPage = (paging.ColumnsPerPage * paging.RowsPerPage);
            paging.PagesTotal = (int)MathF.Ceiling((float)availableRoutes.Count() / perPage);
            result.PagingModel = paging;

            //Filter out everything except current page
            result.Routes = result.Routes.Skip(perPage * (result.PagingModel.CurrentPage-1)).Take(perPage).ToList();

            //Prepare previews
            availableRoutes.ForEach(x => x.Image = ImageUtility.GetPreviewOrPlaceholder(_webHostEnviroment, x.Image));


            return result;
        }
        #endregion
    }
}
