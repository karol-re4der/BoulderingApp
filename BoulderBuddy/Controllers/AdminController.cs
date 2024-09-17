using Microsoft.AspNetCore.Mvc;

namespace BoulderBuddy.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
