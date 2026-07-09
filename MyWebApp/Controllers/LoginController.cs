using Microsoft.AspNetCore.Mvc;
using MyWebApp.Auth;
using MyWebApp.Models;
using Newtonsoft.Json;
using Supabase.Gotrue;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace MyWebApp.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
           return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return View("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> ValidateLogin(UserModel user)
        {
            var session = await SupabaseAuthentication.SignIn(user.Email, user.Pwd);

            if (session is null)
                return RedirectToAction("Index", "Login");

            HttpContext.Session.SetString(
                "session",
                JsonConvert.SerializeObject(session));

            return RedirectToAction("Index", "Home");
        }
    }
}
