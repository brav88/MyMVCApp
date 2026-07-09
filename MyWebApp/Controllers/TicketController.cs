using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using MyWebApp.Services;
using Newtonsoft.Json;
using Supabase.Gotrue;

namespace MyWebApp.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("session")))
                return RedirectToAction("Index", "Login");

            ViewData["CustomNavMenu"] = NavigationService.GetMenuPages(2);

            List<Ticket> ticketList = TicketService.getAll().Result;

            return View(ticketList);
        }

        public IActionResult Detail(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("session")))
                return RedirectToAction("Index", "Login");

            Supabase.Gotrue.Session? session = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("session"));

            ViewData["CustomNavMenu"] = NavigationService.GetMenuPages(2);

            Ticket detail = TicketService.getTicketById(id).Result;

            detail.ActiveSessionUserId = session.User.Id;

            return View(detail);
        }

        public IActionResult PostComment(String ticketId, String commentText)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("session")))
                return RedirectToAction("Index", "Login");

            Supabase.Gotrue.Session? session = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("session"));

            Comment comment = new Comment
            {
                CommentText = commentText,
                TicketId = Convert.ToInt32(ticketId),
                CreatedBy = session.User.Id,
                CreatedAt = DateTime.Now,
            };

            TicketService.postComment(comment);

            return Redirect("Detail?id=" + ticketId);
        }

        public IActionResult DeleteComment(String ticketId, int commentId)
        {
            TicketService.deleteComment(commentId);

            return Redirect("Detail?id=" + ticketId);
        }
    }
}
