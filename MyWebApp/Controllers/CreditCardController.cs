using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.DatabaseHelper;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class CreditCardController : Controller
    {
        private ApplicationDbContext _context;

        public CreditCardController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cards = await _context.CreditCard.ToListAsync();

            return View();
        }

        public async Task<IActionResult> Create()
        {
            CreditCard creditCard = new CreditCard()
            {                
                Number = "3456 4657 8374 9863",
                Holder = "Juan Rojas",
                CVV = 225,
                ExpirationDate = DateTime.Now,
                Issuer = "MasterCard",
                Status = (char)1,
            };

            _context.Add(creditCard);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
