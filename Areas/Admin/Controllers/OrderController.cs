using HamBurger.Data;
using HamBurger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace HamBurger.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        [BindProperty]
        public OrderDetailVM OrderDetailVM { get; set; }
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Authorize(Roles =Other.Role_Admin)]
        public IActionResult Onaylandi()
        {
            OrderHeader orderHeader = _context.OrderHeaders.FirstOrDefault(x => x.Id == OrderDetailVM.OrderHeader.Id);
            orderHeader.OrderStatus = Other.Siparis_Onaylandi;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Yolda()
        {
            OrderHeader orderHeader = _context.OrderHeaders.FirstOrDefault(x => x.Id == OrderDetailVM.OrderHeader.Id);
            orderHeader.OrderStatus = Other.Siparis_Yolda;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            OrderDetailVM = new OrderDetailVM
            {
                OrderHeader = _context.OrderHeaders.FirstOrDefault(x => x.Id == id),
                OrderDetail = _context.OrderDetails.Where(x => x.OrderId == id).Include(x => x.Product)
            };
            return View(OrderDetailVM);
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity; // Sipariş veren kullanıcıyı belirleme
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier); // Sipariş veren kullanıcıyı belirleme
            IEnumerable<OrderHeader> orderHeadersList;
            if (User.IsInRole(Other.Role_Admin))
            {
                orderHeadersList = _context.OrderHeaders.ToList();
            }
            else
            {
                orderHeadersList = _context.OrderHeaders.Where(x => x.ApplicationUserId == claim.Value).Include(x => x.ApplicationUser);
            }
            return View(orderHeadersList);
        }
        public IActionResult Siparis_Bekleyen()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity; // Sipariş veren kullanıcıyı belirleme
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier); // Sipariş veren kullanıcıyı belirleme
            IEnumerable<OrderHeader> orderHeadersList;
            if (User.IsInRole(Other.Role_Admin))
            {
                orderHeadersList = _context.OrderHeaders.Where(x => x.OrderStatus == Other.Siparis_Bekleyen);
            }
            else
            {
                orderHeadersList = _context.OrderHeaders.Where(x => x.ApplicationUserId == claim.Value && x.OrderStatus == Other.Siparis_Bekleyen).Include(x => x.ApplicationUser);
            }
            return View(orderHeadersList);
        }
        public IActionResult Siparis_Onaylandi()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity; // Sipariş veren kullanıcıyı belirleme
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier); // Sipariş veren kullanıcıyı belirleme
            IEnumerable<OrderHeader> orderHeadersList;
            if (User.IsInRole(Other.Role_Admin))
            {
                orderHeadersList = _context.OrderHeaders.Where(x => x.OrderStatus == Other.Siparis_Onaylandi);
            }
            else
            {
                orderHeadersList = _context.OrderHeaders.Where(x => x.ApplicationUserId == claim.Value && x.OrderStatus == Other.Siparis_Onaylandi).Include(x => x.ApplicationUser);
            }
            return View(orderHeadersList);
        }
        public IActionResult Siparis_Yolda()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity; // Sipariş veren kullanıcıyı belirleme
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier); // Sipariş veren kullanıcıyı belirleme
            IEnumerable<OrderHeader> orderHeadersList;
            if (User.IsInRole(Other.Role_Admin))
            {
                orderHeadersList = _context.OrderHeaders.Where(x => x.OrderStatus == Other.Siparis_Yolda);
            }
            else
            {
                orderHeadersList = _context.OrderHeaders.Where(x => x.ApplicationUserId == claim.Value && x.OrderStatus == Other.Siparis_Yolda).Include(x => x.ApplicationUser);
            }
            return View(orderHeadersList);
        }
    }
}
