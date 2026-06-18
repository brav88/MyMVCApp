using Microsoft.AspNetCore.Mvc;
using MyWebApp.Auth;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class LoginController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ValidateLogin(User user)
        {
            Supabase.Gotrue.Session? session = SupabaseAuthentication.SignIn(user.Email, user.Pwd).Result;
            
            if (session != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
