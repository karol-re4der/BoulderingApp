using BoulderBuddy.Data;
using BoulderBuddy.Models;
using BoulderBuddy.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Browse()
        {
            var testGym = "Mood";

            List<Routes> availableRoutes = (from route in _db.Routes.ToList()
                                            join grade in _db.Grades.ToList() on route.GradeId equals grade.Id
                                            join gradingSystem in _db.GradingSystems.ToList() on grade.GradingSystemId equals gradingSystem.Id
                                            join gym in _db.Gyms.ToList() on gradingSystem.Id equals gym.GradingSystemId
                                            where gym.Name.Equals(testGym)
                                            select route).ToList<Routes>();

            //Check if route image exists - if not, insert placeholder instead
            string placeholderPath = "res/route_previews/placeholder.jpg";
            availableRoutes.ForEach(x => x.Image = ( !System.IO.File.Exists(Path.Combine(_webHostEnviroment.WebRootPath, x.Image)) ? placeholderPath : x.Image));

            BrowseViewModel result = new BrowseViewModel(availableRoutes);

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
