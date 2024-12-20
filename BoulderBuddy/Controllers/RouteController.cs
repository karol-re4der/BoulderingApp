using BoulderBuddy.Data;
using BoulderBuddy.Models;
using BoulderBuddy.Models.ViewModels;
using BoulderBuddy.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using static BoulderBuddy.Utility.ImageUtility;

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

        [HttpGet]
        public IActionResult Show(int id)
        {
            List<Routes> matchingRoutes = _db.Routes.Where(x => x.Id==id).ToList();

            if (!matchingRoutes.IsNullOrEmpty())
            {
                Routes resultRoute = matchingRoutes.First();
                IdentityUser identityUser = _userManager.GetUserAsync(User).Result!;
                int userId = UserUtility.GetUserByNetId(_db, identityUser.Id).Id;

                //translate image path
                resultRoute.Image = ImageUtility.GetImagePath(_env, resultRoute.Image, ImageType.Preview);

                //Load route comments
                List<CommentsViewModel> routeComments = loadComments(id, 0, 10);

                //Load route ascents
                int ascentsTotal = 0;
                int ascentsSuccessful = 0;
                int progressBarAscents = 0;
                int progressBarAttempts = 0;
                float averageAscents = 0f;

                List<AscentsViewModel> routeAscents = (from ascent in _db.Ascents
                                                       join user in _db.UserData on ascent.UserId equals user.Id
                                                       where ascent.RouteId == resultRoute.Id
                                                       select new AscentsViewModel(user, ascent)).ToList();

                if (routeAscents.Count>0)
                {
                    averageAscents = (float) routeAscents.Count() / (DateTime.Now - resultRoute.AddDateTime).Days;
                }
                ascentsTotal = routeAscents.Count();
                ascentsSuccessful = routeAscents.Where(x => x.Ascent.Success).Count();
                progressBarAscents = (int)(((float)ascentsSuccessful / ascentsTotal) * 100);
                progressBarAttempts = 100 - progressBarAscents;

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

                RouteViewModel newModel = new RouteViewModel(resultRoute, routeComments, routeAscents, gradeRatings, averageAscents);
                newModel.Progress_Grading_Easy = progressBarEasy;
                newModel.Progress_Grading_Fair = progressBarHard;
                newModel.Progress_Grading_Hard = progressBarOk;
                newModel.Progress_Ascents_Success = progressBarAscents;
                newModel.Progress_Ascents_Attempt = progressBarAttempts;
                newModel.AscentsSuccessful = ascentsSuccessful;
                newModel.AscentsTotal = ascentsTotal;

                //
                Ascents userAscent = _db.Ascents.Where(x => x.UserId == userId).FirstOrDefault();
                newModel.Radio_Ascents_Status_Success = userAscent?.Success == true?"checked":"";
                newModel.Radio_Ascents_Status_Attempt = userAscent?.Success == false ? "checked" : ""; ;
                newModel.Radio_Ascents_Status_Blank = userAscent == null ? "checked" : ""; ;

                return View(newModel);
            }
            {
                throw new NotImplementedException("Route not found");
            }
        }

        #region radios
        [HttpPost]
        public IActionResult MarkAscent(int id, string value)
        {
            int action = 0; //0 - clear, 1 - ascent, other - not attempted

            if (value.EndsWith("success"))
            {
                action = 1;
            }
            else if(value.EndsWith("attempt"))
            {
                action = 2;
            }

            if (_signInManager.IsSignedIn(User))
            {
                try
                {
                    IdentityUser identityUser = _userManager.GetUserAsync(User).Result!;
                    UserData user = UserUtility.GetUserByNetId(_db, identityUser.Id);

                    Ascents ascent = _db.Ascents.Where(x => x.UserId == user.Id && x.RouteId == id).FirstOrDefault();
                    if (ascent == null && action!=0)
                    {
                        ascent = new Ascents()
                        {
                            RouteId = id,
                            UserId = user.Id
                        };
                    }

                    if (action != 0)
                    {
                        ascent.Success = action==1 ? true : false;
                        _db.Ascents.Update(ascent);
                        _db.SaveChanges();
                    }
                    else if(action==0 && ascent!=null)
                    {
                        _db.Ascents.Remove(ascent);
                        _db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception();
                }
            }
            return View();
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
