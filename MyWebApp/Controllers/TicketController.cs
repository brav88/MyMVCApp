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
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("session")))
                return RedirectToAction("Index", "Login");

            ViewData["CustomNavMenu"] = NavigationService.GetMenuPages(2);

            return View(await TicketService.GetAll());
        }

        public async Task<IActionResult> Detail(int id)
        {
            string? sessionJson = HttpContext.Session.GetString("session");

            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Index", "Login");

            var session = JsonConvert.DeserializeObject<Session>(sessionJson);

            ViewData["CustomNavMenu"] = NavigationService.GetMenuPages(2);

            var detail = await TicketService.GetTicketById(id);

            if (detail is null)
                return NotFound();

            detail.ActiveSessionUserId = session!.User.Id;

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

            TicketService.PostComment(comment);

            return Redirect("Detail?id=" + ticketId);
        }

        public IActionResult DeleteComment(String ticketId, int commentId)
        {
            TicketService.DeleteComment(commentId);

            return Redirect("Detail?id=" + ticketId);
        }
    }
}
