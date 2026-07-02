using Microsoft.AspNetCore.Identity;
using MyWebApp.Models;

namespace MyWebApp.Services
{
    public static class NavigationService
    {     
        public static List<NavigationPages> GetMenuPages(int userRole)
        {            
            if (userRole == 1) 
            {
                return new List<NavigationPages>
                {
                    new NavigationPages { Title="Home", Controller="Home", Action = "Index"},
                    new NavigationPages { Title="Product", Controller="Product", Action = "Index"},                
                    new NavigationPages { Title="Logout", Controller="Login", Action = "Logout"}
                };
            }
            if (userRole == 2)
            {
                return new List<NavigationPages>
                {
                    new NavigationPages { Title="Home", Controller="Home", Action = "Index"},
                    new NavigationPages { Title="Tickets", Controller="Tickets", Action = "Index"},
                    new NavigationPages { Title="Logout", Controller="Login", Action = "Logout"},
                    new NavigationPages { Title="Privacy", Controller="Privacy", Action = "Privacy"},
                    new NavigationPages { Title="Settings", Controller="Settings", Action = "Settings"},
                    new NavigationPages { Title="Profile", Controller="Profile", Action = "Profile"}
                };
            }

            return new List<NavigationPages>
                {
                    
                };
        }
    }
}
