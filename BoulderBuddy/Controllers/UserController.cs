using BoulderBuddy.Models;
using BoulderBuddy.Models.ViewModels;
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
