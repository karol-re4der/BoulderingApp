using BoulderBuddy.Data;
using BoulderBuddy.Models;
using BoulderBuddy.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BoulderBuddy.Controllers
{
    public class RouteController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RouteController(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }

        public IActionResult Show(string id)
        {
            List<Routes> matchingRoutes = _db.Routes.Where(x => x.RouteReference.Equals(id)).ToList();

            if (!matchingRoutes.IsNullOrEmpty())
            {
                Routes resultRoute = matchingRoutes.First();


                //Load route comments
                List<CommentsViewModel> routeComments = (from comment in _db.RouteComments
                                                   join user in _db.Users on comment.UserId equals user.ID
                                                   where comment.RouteId == resultRoute.Id
                                                   select new CommentsViewModel(user, comment)).ToList();

                //Load route ascents
                List<AscentsViewModel> routeAscents = (from ascent in _db.Ascents
                                                 join user in _db.Users on ascent.UserId equals user.ID
                                                 where ascent.RouteId == resultRoute.Id
                                                 select new AscentsViewModel(user, ascent)).ToList();

                return View(new RouteViewModel(resultRoute, routeComments, routeAscents));
            }
            {
                throw new NotImplementedException("Route not found");
            }
        }

        [HttpPost]
        public IActionResult PostComment(Routes route)
        {

            RouteComments newComment = new RouteComments();
            newComment.Comment = Request.Form["commentTextArea"].ToString();
            newComment.CommentDateTime = DateTime.Now;
            newComment.RouteId = route.Id;
            newComment.UserId = 1;

            _db.RouteComments.Add(newComment);
            _db.SaveChanges();
            return View();
        }
    }
}
