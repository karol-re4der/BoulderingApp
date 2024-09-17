using BoulderBuddy.Data;
using BoulderBuddy.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BoulderBuddy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _db = dbContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var testGym = "Mood";

            List<Routes> result = (from route in _db.Routes.ToList()
                                   join grade in _db.Grades.ToList() on route.GradeId equals grade.Id
                                   join gradingSystem in _db.GradingSystems.ToList() on grade.GradingSystemId equals gradingSystem.Id
                                   join gym in _db.Gyms.ToList() on gradingSystem.Id equals gym.GradingSystemId
                                   where gym.Name.Equals(testGym)
                                   select route).ToList<Routes>();
            return View(result);
        }

        public IActionResult Privacy()
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
