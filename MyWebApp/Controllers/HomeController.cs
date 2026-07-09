using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using MyWebApp.Services;
using Newtonsoft.Json;
using Supabase.Gotrue;
using System.Diagnostics;

namespace MyWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("session")))
                return RedirectToAction("Index", "Login");

            Supabase.Gotrue.Session? session = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("session"));

            ViewData["photo"] = HttpContext.Session.GetString("photoPath");             

            ViewData["CustomNavMenu"] = NavigationService.GetMenuPages(2);

            List<Hotel> hotels = HotelService.getAll();

            return View(hotels);
        }

        public IActionResult Privacy()
        {
            ViewData["Titulo"] = "Mi página de privacidad";
            ViewData["Año"] = 2024;
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
