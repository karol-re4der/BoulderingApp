using BoulderBuddy.Data;
using BoulderBuddy.Models;
using BoulderBuddy.Models.ViewModels;
using BoulderBuddy.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using static BoulderBuddy.Utility.ImageUtility;
using static System.Net.Mime.MediaTypeNames;

namespace BoulderBuddy.Controllers
{
    public class RouteController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RouteController(ApplicationDbContext dbContext, IWebHostEnvironment env, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _db = dbContext;
            _env = env;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #region helpers
        private Routes getRouteById(int id)
        {
            List<Routes> matchingRoutes = _db.Routes.Where(x => x.Id == id).ToList();

            if (matchingRoutes.Count == 1)
            {
                return matchingRoutes.First();
            }
            return null;
        }

        private AscentsSectionViewModel loadAscentsSectionViewModel(Routes route, UserData userData)
        {
            AscentsSectionViewModel result = new AscentsSectionViewModel();

            List<AscentsViewModel> routeAscents = (from ascent in _db.Ascents
                                                   join user in _db.UserData on ascent.UserId equals user.Id
                                                   where ascent.RouteId == route.Id
                                                   select new AscentsViewModel(user, ascent)).ToList();

            if (routeAscents.Count > 0)
            {
                result.AscentsAverage = (float)routeAscents.Count() / (DateTime.Now - route.AddDateTime).Days;
            }
            result.AscentsTotal = routeAscents.Count();
            result.AscentsSuccessful = routeAscents.Where(x => x.Ascent.Success).Count();
            result.Progress_Success = (int)(((float)result.AscentsSuccessful / result.AscentsTotal) * 100);
            result.Progress_Attempt = 100 - result.Progress_Success;
            result.AscentData = routeAscents;

            Ascents userAscent = _db.Ascents.Where(x => x.UserId == userData.Id && x.RouteId==route.Id).FirstOrDefault();
            result.Radio_Status_Success = userAscent?.Success == true ? "checked" : "";
            result.Radio_Status_Attempt = userAscent?.Success == false ? "checked" : "";
            result.Radio_Status_Blank = userAscent == null ? "checked" : "";

            return result;
        }

        private GradingSectionViewModel loadGradingSectionViewModel(Routes route, UserData userData)
        {
            GradingSectionViewModel result = new GradingSectionViewModel();

            List<GradeRatings> gradings = (from grading in _db.GradeRatings
                                                   join user in _db.UserData on grading.UserDataId equals user.Id
                                                   where grading.RouteId == route.Id
                                                   select grading).ToList();

            if (gradings.Count() > 0)
            {
                result.Progress_Easy = (int)(((float)gradings.Where(x => x.Rating < 0).Count() / gradings.Count()) * 100);
                result.Progress_Hard = (int)(((float)gradings.Where(x => x.Rating > 0).Count() / gradings.Count()) * 100);
                result.Progress_Fair = (int)(((float)gradings.Where(x => x.Rating == 0).Count() / gradings.Count()) * 100);
            }

            GradeRatings userGrading = _db.GradeRatings.Where(x => x.UserDataId == userData.Id && x.RouteId == route.Id).FirstOrDefault();
            result.Radio_Status_Hard = userGrading?.Rating > 0 ? "checked" : "";
            result.Radio_Status_Easy = userGrading?.Rating < 0 ? "checked" : "";
            result.Radio_Status_Fair = userGrading?.Rating == 0 ? "checked" : "";

            return result;
        }
        #endregion

        [HttpGet]
        public IActionResult Show(int id)
        {
            Routes resultRoute = getRouteById(id);
            if (resultRoute != null)
            {
                IdentityUser identityUser = _userManager.GetUserAsync(User).Result!;
                UserData userData = UserUtility.GetUserByNetId(_db, identityUser.Id);

                //translate image path
                resultRoute.Image = ImageUtility.GetPreviewOrPlaceholder(_env, resultRoute.Image);

                //Load route comments
                List<CommentsViewModel> routeComments = loadComments(id, 0, 10);

                //Load route ascents
                AscentsSectionViewModel ascentsSection = loadAscentsSectionViewModel(resultRoute, userData);

                //Load gradings ascents
                GradingSectionViewModel gradingSection = loadGradingSectionViewModel(resultRoute, userData);


                RouteViewModel newModel = new RouteViewModel(resultRoute, routeComments);
                newModel.AscentsSectionViewModel = ascentsSection;
                newModel.GradingSectionViewModel = gradingSection;

                return View(newModel);
            }
            {
                throw new NotImplementedException("Route not found");
            }
        }

        #region radios
        [HttpPost]
        public IActionResult MarkAscent(int id, string ascentResult)
        {
            TempData["NotificationRequested"] = "true";

            Routes resultRoute = getRouteById(id);
            IdentityUser identityUser = _userManager.GetUserAsync(User).Result!;
            UserData userData = UserUtility.GetUserByNetId(_db, identityUser.Id);

            if (_signInManager.IsSignedIn(User))
            {
                try
                {

                    Ascents ascent = _db.Ascents.Where(x => x.UserId == userData.Id && x.RouteId == id).FirstOrDefault();
                    if (ascent == null && !ascentResult.Equals("blank"))
                    {
                        ascent = new Ascents()
                        {
                            RouteId = id,
                            UserId = userData.Id
                        };
                    }

                    if (!ascentResult.Equals("blank"))
                    {
                        ascent.Success = ascentResult.Equals("success") ? true : false;
                        _db.Ascents.Update(ascent);
                        _db.SaveChanges();
                        TempData["SuccessMessage"] = ascentResult.Equals("success") ? "Route ascent logged!" : "Route attempt logged!";

                    }
                    else if (ascentResult.Equals("blank") && ascent != null)
                    {
                        _db.Ascents.Remove(ascent);
                        _db.SaveChanges();
                        TempData["SuccessMessage"] = "Route attempt cleared!";
                    }
                }
                catch (Exception e)
                {
                    TempData["ErrorMessage"] = "Something went wrong. Try again?";
                }
            }

            AscentsSectionViewModel resultViewModel = loadAscentsSectionViewModel(resultRoute, userData);

            return PartialView("_Section_Ascents", resultViewModel);
        }

        [HttpPost]
        public IActionResult MarkGrading(int id, string gradingResult)
        {
            TempData["NotificationRequested"] = "true";

            Routes resultRoute = getRouteById(id);
            IdentityUser identityUser = _userManager.GetUserAsync(User).Result!;
            UserData userData = UserUtility.GetUserByNetId(_db, identityUser.Id);

            if (_signInManager.IsSignedIn(User))
            {
                try
                {

                    GradeRatings grading = _db.GradeRatings.Where(x => x.UserDataId == userData.Id && x.RouteId == id).FirstOrDefault();
                    if (grading == null && !gradingResult.Equals("fair"))
                    {
                        grading = new GradeRatings()
                        {
                            RouteId = id,
                            UserDataId = userData.Id
                        };
                    }

                    if (!gradingResult.Equals("fair"))
                    {
                        grading.Rating = (short)(gradingResult.Equals("hard") ? 1 : -1);
                        _db.GradeRatings.Update(grading);
                        _db.SaveChanges();
                        TempData["SuccessMessage"] = gradingResult.Equals("hard") ? "Grading marked as harder!" : "Grading marked as easier!";

                    }
                    else if (gradingResult.Equals("fair") && grading != null)
                    {
                        grading.Rating = 0;
                        _db.GradeRatings.Update(grading);
                        _db.SaveChanges();
                        TempData["SuccessMessage"] = "Grading marked as fair!";
                    }
                }
                catch (Exception e)
                {
                    TempData["ErrorMessage"] = "Something went wrong. Try again?";
                }
            }

            GradingSectionViewModel resultViewModel = loadGradingSectionViewModel(resultRoute, userData);

            return PartialView("_Section_Grading", resultViewModel);
        }
        #endregion

        #region CommentBox
        [HttpGet]
        public IActionResult LoadComments(int id)
        {
            List<CommentsViewModel> comments = loadComments(id, 0, 10);
            return PartialView("_CommentBox", comments);
        }

        [HttpPost]
        public IActionResult PostComment(int id, string commentTextArea)
        {
            if (_signInManager.IsSignedIn(User))
            {
                if (postNewComment(commentTextArea, id))
                {

                }
                else
                {

                }
            }
            

            List<CommentsViewModel> comments = loadComments(id, 0, 10);
            return PartialView("_CommentBox", comments);
        }

        private List<CommentsViewModel> loadComments(int routeId, int skip, int amount)
        {
            List<CommentsViewModel> routeComments = (from comment in _db.RouteComments
                                                       join user in _db.UserData on comment.UserDataId equals user.Id
                                                       where comment.RouteId == routeId
                                                       orderby comment.CommentDateTime descending
                                                     select new CommentsViewModel(user, comment)).Skip(skip).Take(amount).ToList();
            return routeComments;
        }

        private bool postNewComment(string commText, int routeId)
        {
            try
            {
                IdentityUser identityUser = _userManager.GetUserAsync(User).Result!;
                UserData user = UserUtility.GetUserByNetId(_db, identityUser.Id);

                RouteComments newComment = new RouteComments();
                newComment.Comment = commText;
                newComment.CommentDateTime = DateTime.Now;
                newComment.RouteId = routeId;
                newComment.UserDataId = user.Id;

                _db.RouteComments.Add(newComment);
                _db.SaveChanges();
            }
            catch(Exception e)
            {
                return false;
            }
            
            return true;
        }

        #endregion
    }
}
