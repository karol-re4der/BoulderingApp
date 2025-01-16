using Microsoft.AspNetCore.Mvc;

namespace BoulderBuddy.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
