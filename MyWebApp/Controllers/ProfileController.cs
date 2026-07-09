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
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            var session = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("session"));

            if (session is not null)
                await ProfileService.UploadPhoto(photo, session.User.Id);

            return RedirectToAction("Index", "Home");
        }
    }
}
