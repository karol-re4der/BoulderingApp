using BoulderBuddy.Data;
using BoulderBuddy.Models;
using BoulderBuddy.Models.ViewModels;
using BoulderBuddy.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.IO;

namespace BoulderBuddy.Controllers
{
    public class AdminController : Controller
    {

        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnviroment;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ILogger<AdminController> logger, ApplicationDbContext dbContext, IWebHostEnvironment env, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _webHostEnviroment = env;
            _db = dbContext;
            _logger = logger;

            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetPreview(string previewName)
        {
            return File(ImageUtility.GetPreviewOrPlaceholder(_webHostEnviroment , previewName), "image/jpg");
        }

        [HttpGet]
        public IActionResult UpsertRoute(string id)
        {
            if (_signInManager.IsSignedIn(User))
                {
                int routeId = 0;
                int.TryParse(id, out routeId);

                Routes route;
                if (id.Equals("new"))
                {
                    route = new Routes();
                    route.AddDateTime = DateTime.Now;
                }
                else
                {
                    try
                    {
                        route = _db.Routes.FirstOrDefault(x => x.Id == routeId);
                    }
                    catch (Exception e)
                    {
                        return NotFound();
                    }
                }

                List<SelectListItem> gymsSelection = (from gym in _db.Gyms
                                                      select new SelectListItem(gym.Name, gym.Id + "")).ToList();
                List<SelectListItem> gradesSelection = (from grade in _db.Grades
                                                        select new SelectListItem(grade.Name, grade.Id + "")).ToList();
                List<Tuple<int, int>> gymGradeTable = (from gym in _db.Gyms
                                                       join grading in _db.GradingSystems on gym.GradingSystemId equals grading.Id
                                                       join grade in _db.Grades on grading.Id equals grade.GradingSystemId
                                                       select new Tuple<int, int>(gym.Id, grade.Id)).ToList();

                UpsertRouteViewModel newModel = new UpsertRouteViewModel()
                {
                    Route = route,
                    GymsAvailable = gymsSelection,
                    GradesAvailable = gradesSelection,
                    GymGradeTable = gymGradeTable
                };

                return View(newModel);
            }
            
            return NotFound();

        }

        [HttpPost]
        public IActionResult UpsertRoute(UpsertRouteViewModel model, IFormFile? previewImage)
        {
            if (_signInManager.IsSignedIn(User))
            {
                string newFileName = "";

                try
                {
                    if (ModelState.IsValid)
                    {
                        Routes route = model.Route;

                        if (previewImage != null)
                        {
                            string targetPath = ImageUtility.GetFullPath(_webHostEnviroment, ImageUtility.CreateNewPreviewPath(_webHostEnviroment, previewImage));
                            newFileName = Path.GetFileName(targetPath);
                            using (var fileStream = new FileStream(targetPath, FileMode.Create))
                            {
                                previewImage.CopyTo(fileStream);
                            }

                            ImageUtility.RemovePreview(_webHostEnviroment, route.Image);

                            route.Image = newFileName;
                        }

                        _db.Routes.Update(route);
                        _db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    return NotFound();
                }

                return RedirectToAction("Show", "Route", new {id=model.Route.Id});
            }

            return NotFound();
        }
    }
}
