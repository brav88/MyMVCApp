using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index()
        {
            List<Ticket> ticketList = TicketService.getAll().Result;

            return View(ticketList);
        }

        public IActionResult Detail(int id)
        {
            Ticket detail = TicketService.getTicketById(id).Result;

            return View(detail);
        }

        public IActionResult PostComment(String ticketId, String commentText)
        {
            Comment comment = new Comment
            {
                CommentText = commentText,
                TicketId = Convert.ToInt32(ticketId),
                CreatedBy = "9d647c29-0fca-4072-81f4-ff2e255e7152",
                CreatedAt = DateTime.Now,
            };

            TicketService.postComment(comment);

            return Redirect("Detail?id=" + ticketId);
        }
    }
}
