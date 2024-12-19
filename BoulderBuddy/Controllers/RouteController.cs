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

                //translate image path
                resultRoute.Image = ImageUtility.GetImagePath(_env, resultRoute.Image, ImageType.Preview);

                //Load route comments
                List<CommentsViewModel> routeComments = loadComments(id, 0, 10);

                //Load route ascents
                List<AscentsViewModel> routeAscents = (from ascent in _db.Ascents
                                                       join user in _db.UserData on ascent.UserId equals user.Id
                                                       where ascent.RouteId == resultRoute.Id
                                                       select new AscentsViewModel(user, ascent)).ToList();

                //TempData["LoadedComments"] = routeComments;

                return View(new RouteViewModel(resultRoute, routeComments, routeAscents));
            }
            {
                throw new NotImplementedException("Route not found");
            }
        }

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
