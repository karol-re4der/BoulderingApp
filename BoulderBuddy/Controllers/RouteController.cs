using BoulderBuddy.Data;
using BoulderBuddy.Models;
using BoulderBuddy.Models.ViewModels;
using BoulderBuddy.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            result.Progress_Ascents_Success = (int)(((float)result.AscentsSuccessful / result.AscentsTotal) * 100);
            result.Progress_Ascents_Attempt = 100 - result.Progress_Ascents_Success;
            result.AscentData = routeAscents;

            Ascents userAscent = _db.Ascents.Where(x => x.UserId == userData.Id).FirstOrDefault();
            result.Radio_Ascents_Status_Success = userAscent?.Success == true ? "checked" : "";
            result.Radio_Ascents_Status_Attempt = userAscent?.Success == false ? "checked" : "";
            result.Radio_Ascents_Status_Blank = userAscent == null ? "checked" : "";

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
                resultRoute.Image = ImageUtility.GetImagePath(_env, resultRoute.Image, ImageType.Preview);

                //Load route comments
                List<CommentsViewModel> routeComments = loadComments(id, 0, 10);

                //Load route ascents
                AscentsSectionViewModel ascentsSection = loadAscentsSectionViewModel(resultRoute, userData);

                //Calculate grade rating
                int progressBarEasy = 0;
                int progressBarHard = 0;
                int progressBarOk = 0;

                List<GradeRatings> gradeRatings = (from gradeRating in _db.GradeRatings
                                                   join user in _db.UserData on gradeRating.UserDataId equals user.Id
                                                   where gradeRating.RouteId == resultRoute.Id
                                                   select gradeRating).ToList();

                if (gradeRatings.Count() > 0)
                {
                    progressBarEasy = (int)(((float) gradeRatings.Where(x => x.Rating > 0).Count() / gradeRatings.Count() ) * 100);
                    progressBarHard = (int)(((float)gradeRatings.Where(x => x.Rating < 0).Count() / gradeRatings.Count()) * 100);
                    progressBarOk = (int)(((float)gradeRatings.Where(x => x.Rating == 0).Count() / gradeRatings.Count()) * 100);
                }

                RouteViewModel newModel = new RouteViewModel(resultRoute, routeComments, gradeRatings);
                newModel.Progress_Grading_Easy = progressBarEasy;
                newModel.Progress_Grading_Fair = progressBarHard;
                newModel.Progress_Grading_Hard = progressBarOk;
                newModel.AscentsSectionViewModel = ascentsSection;

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
        public IActionResult MarkGrading(int id, string value)
        {
            if (_signInManager.IsSignedIn(User))
            {

            }
            return View();
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
