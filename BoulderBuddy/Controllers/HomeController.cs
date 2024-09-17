using BoulderBuddy.Data;
using BoulderBuddy.Models;
using BoulderBuddy.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
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

        public IActionResult Browse(int id = 1)
        {
            var testGym = "Mood";

            List < Routes > availableRoutes = (from route in _db.Routes.ToList()
                                               join grade in _db.Grades.ToList() on route.GradeId equals grade.Id
                                               join gradingSystem in _db.GradingSystems.ToList() on grade.GradingSystemId equals gradingSystem.Id
                                               join gym in _db.Gyms.ToList() on gradingSystem.Id equals gym.GradingSystemId
                                               where gym.Name.Equals(testGym)
                                               select route).ToList<Routes>();

            //Check if route image exists - if not, insert placeholder instead
            string placeholderPath = @"\res\route_previews\placeholder.jpg";
            string previewsPath = @"\res\route_previews\";

            availableRoutes.ForEach(x => x.Image = ( !System.IO.File.Exists(Path.Combine(_webHostEnviroment.WebRootPath, previewsPath.Substring(1) + x.Image)) ? placeholderPath : previewsPath+x.Image));

            BrowseViewModel result = new BrowseViewModel(availableRoutes);

            if(result.ColumnsPerPage>0 && result.RowsPerPage > 0)
            {
                result.PagesTotal = availableRoutes.Count() / (result.ColumnsPerPage * result.ColumnsPerPage);
            }

            int perPage = (result.ColumnsPerPage * result.RowsPerPage);
            result.PagesTotal = (int)MathF.Ceiling((float)availableRoutes.Count() / perPage);


            return View(result);
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
    }
}
