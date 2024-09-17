using BoulderBuddy.Models;
using Microsoft.AspNetCore.Mvc;
using BoulderBuddy.Models.ViewModels;

namespace BoulderBuddy.Controllers
{
    public class CommentSectionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PostComment(Routes route)
        {

            RouteComments newComment = new RouteComments();
            newComment.Comment = Request.Form["commentTextArea"].ToString();
            newComment.CommentDateTime = DateTime.Now;
            newComment.RouteId = route.Id;
            newComment.UserId = 1;

            //_db.RouteComments.Add(newComment);
            //_db.SaveChanges();
            return View();
        }
    }
}
