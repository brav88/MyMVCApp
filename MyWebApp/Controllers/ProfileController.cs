using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using MyWebApp.Services;
using MyWebApp.SupabaseClient;
using Newtonsoft.Json;
using Supabase;
using Supabase.Gotrue;

namespace MyWebApp.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult UploadPhoto(IFormFile photo)
        {
            Supabase.Gotrue.Session? session = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("session"));

            if (session != null)
            {
                var service = ProfileService.UploadPhoto(photo, session.User.Id).Result;
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
