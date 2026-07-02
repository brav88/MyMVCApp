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
        public IActionResult ValidateLogin(UserModel user)
        {
            Supabase.Gotrue.Session? session = SupabaseAuthentication.SignIn(user.Email, user.Pwd).Result;
            
            if (session != null)
            {
                HttpContext.Session.SetString("session", JsonConvert.SerializeObject(session));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
