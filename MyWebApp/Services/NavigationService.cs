using Microsoft.AspNetCore.Identity;
using MyWebApp.Models;

namespace MyWebApp.Services
{
    public static class NavigationService
    {
        public static List<NavigationPages> GetMenuPages(int userRole)
        {
            var pages = new List<NavigationPages>
            {
                new() { Title = "Home", Controller = "Home", Action = "Index" }
            };

            switch (userRole)
            {
                case 1:
                    pages.Add(new NavigationPages
                    {
                        Title = "Product",
                        Controller = "Product",
                        Action = "Index"
                    });
                    break;

                case 2:
                    pages.AddRange(
                    [
                        new() { Title = "Ticket", Controller = "Ticket", Action = "Index" },
                new() { Title = "Product", Controller = "Product", Action = "Index" }
                    ]);
                    break;
            }

            pages.Add(new NavigationPages
            {
                Title = "Logout",
                Controller = "Login",
                Action = "Logout"
            });

            return pages;
        }
    }
}
