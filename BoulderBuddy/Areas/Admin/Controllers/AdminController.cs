using BoulderBuddy.DataAccess.Data;
using BoulderBuddy.Models.Models;
using BoulderBuddy.Models.Models.ViewModels;
using BoulderBuddy.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BoulderBuddy.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            return File(ImageUtility.GetPreviewOrPlaceholder(_webHostEnviroment, previewName), "image/jpg");
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
        public IActionResult UpsertRoute(UpsertRouteViewModel model)
        {
            if (_signInManager.IsSignedIn(User))
            {
                string newFileName = "";

                try
                {
                    if (ModelState.IsValid)
                    {
                        Routes route = model.Route;

                        if (model.PreviewImage != null)
                        {
                            if (verifyPreviewFile(model.PreviewImage))
                            {
                                string targetPath = ImageUtility.GetFullPath(_webHostEnviroment, ImageUtility.CreateNewPreviewPath(_webHostEnviroment, model.PreviewImage));
                                newFileName = Path.GetFileName(targetPath);
                                using (var fileStream = new FileStream(targetPath, FileMode.Create))
                                {
                                    model.PreviewImage.CopyTo(fileStream);
                                }

                                ImageUtility.RemovePreview(_webHostEnviroment, route.Image);

                                route.Image = newFileName;
                            }
                            else
                            {
                                return NotFound();
                            }
                        }

                        _db.Routes.Update(route);
                        _db.SaveChanges();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception e)
                {
                    return NotFound();
                }

                return RedirectToAction("Show", "Route", new { id = model.Route.Id });
            }

            return NotFound();
        }

        #region verification

        private bool verifyPreviewFile(IFormFile formFile)
        {
            if (!verifyFileExtension(formFile))
            {
                return false;
            }
            else if (!verifyFileSignature(formFile))
            {
                return false;
            }
            else if (!verifyFileSize(formFile))
            {
                return false;
            }

            return true;
        }

        private bool verifyFileSize(IFormFile formFile)
        {
            if (formFile.Length == 0 || formFile.Length > 5 * 1024 * 1024) //5mb max
            {
                return false;
            }
            return true;
        }

        private bool verifyFileExtension(IFormFile formFile)
        {
            string[] permittedExtensions = { ".jpg", ".jpeg" };

            var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }

            return true;
        }

        private bool verifyFileSignature(IFormFile formFile)
        {
            try
            {
                Dictionary<string, List<byte[]>> _fileSignature =
                new Dictionary<string, List<byte[]>>
                {
                    { ".jpeg", new List<byte[]>
                        {
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xEE },
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xDB },
                        }
                    },
                    { ".jpg", new List<byte[]>
                        {
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xEE },
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xDB },
                        }
                    }
                };

                using (var reader = new BinaryReader(formFile.OpenReadStream()))
                {
                    var signatures = _fileSignature[Path.GetExtension(formFile.FileName).ToLowerInvariant()];
                    var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                    return signatures.Any(signature =>
                        headerBytes.Take(signature.Length).SequenceEqual(signature));
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion
    }
}
