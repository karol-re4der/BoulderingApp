using Microsoft.AspNetCore.Mvc;

namespace BoulderBuddy.Areas.User.Controllers
{
    [Area("User")]
    public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
