using HamBurger.Data;
using HamBurger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HamBurger.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult CategoryDetails(int? id)
        {
            var product = _context.Products.Where(x => x.CategoryId == id).ToList();
            ViewBag.CategoryListId = id;
            return View(product);
        }
        public IActionResult Index()
        {
            var categoryList = _context.Products.ToList();
            var claimIdentity = (ClaimsIdentity)User.Identity; // Sipariş veren kullanıcıyı belirleme
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier); // Sipariş veren kullanıcıyı belirleme
            if (claim != null)
            {
                var count = _context.ShoppingCarts.Where(x => x.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(Other.SShoppingCart, count); // Sipariş veren kullanıcı sayısını aldık
            }
            return View(categoryList);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart SCart)
        {
            SCart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                SCart.ApplicationUserId = claim.Value;
                ShoppingCart cart = _context.ShoppingCarts.FirstOrDefault(x => x.ApplicationUserId == SCart.ApplicationUserId && x.ProductId == SCart.ProductId);
                if (cart == null)
                {
                    _context.ShoppingCarts.Add(SCart);
                }
                else
                {
                    cart.Count += SCart.Count;
                }
            }
            else
            {
                var product = _context.Products.FirstOrDefault(x => x.Id == SCart.Id);
                ShoppingCart cart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.Id
                };
                return View(SCart);
            }
            _context.SaveChanges();
            var count = _context.ShoppingCarts.Where(x => x.ApplicationUserId == SCart.ApplicationUserId).ToList().Count();
            HttpContext.Session.SetInt32(Other.SShoppingCart, count);
            return RedirectToAction("Index");

        }
        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == id);
            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id
            };
            return View(cart);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Contact()
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
