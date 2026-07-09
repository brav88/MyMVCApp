using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Controllers
{
    public class ProductController : Controller
    {       
        public IActionResult Index()
        {
            ViewData["CustomNavMenu"] = NavigationService.GetMenuPages(2);

            return View(ProductService.GetAll());
        }

        public IActionResult Detail(int id)
        {
            ViewData["CustomNavMenu"] = NavigationService.GetMenuPages(2);

            return View(ProductService.GetProductDetail(id));
        }

        public IActionResult Edit(ProductDetail detail)
        {
            ViewData["CustomNavMenu"] = NavigationService.GetMenuPages(2);

            ProductService.SaveProductDetail(detail);

            return RedirectToAction("Detail", new { Id = detail.ProductId });
        }
    }
}
