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
            string? sessionJson = HttpContext.Session.GetString("session");

            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Index", "Login");

            ViewData["photo"] = HttpContext.Session.GetString("photoPath");
            ViewData["CustomNavMenu"] = NavigationService.GetMenuPages(2);

            return View(HotelService.getAll());
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
