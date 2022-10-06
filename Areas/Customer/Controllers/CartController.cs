using HamBurger.Data;
using HamBurger.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace HamBurger.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public ShoppingCartVM ShoppinCartVM { get; set; }
        public CartController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Summary()
        {
            #region Sistemde sipariş verecek olan kullanıcının sepetini getirme
            var claimIdentity = (ClaimsIdentity)User.Identity; // Sipariş veren kullanıcıyı belirleme
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier); // Sipariş veren kullanıcıyı belirleme
            ShoppinCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _context.ShoppingCarts.Where(x => x.ApplicationUserId == claim.Value).Include(x => x.Product)
            };
            #endregion
            foreach (var item in ShoppinCartVM.ListCart)
            {
                item.Price = item.Product.Price;
                ShoppinCartVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppinCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //veriyi post ederken güvenliği sağlar.
        public IActionResult Summary( ShoppingCartVM model)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity; // Sistemde aktif olan müşteriyi bulduk
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppinCartVM.ListCart = _context.ShoppingCarts.Where(x => x.ApplicationUserId == claim.Value).Include(x => x.Product);
            ShoppinCartVM.OrderHeader.OrderStatus = Other.Siparis_Bekleyen;
            ShoppinCartVM.OrderHeader.ApplicationUserId = claim.Value;
            ShoppinCartVM.OrderHeader.OrderDate = DateTime.Now;
            _context.OrderHeaders.Add(ShoppinCartVM.OrderHeader);
            _context.SaveChanges();
            foreach (var item in ShoppinCartVM.ListCart)
            {
                item.Price = item.Product.Price;
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderId = ShoppinCartVM.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };
                ShoppinCartVM.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                model.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                _context.OrderDetails.Add(orderDetail);
            }
            var payment = PaymentProcess(model);
            _context.ShoppingCarts.RemoveRange(ShoppinCartVM.ListCart);
            _context.SaveChanges();
            HttpContext.Session.SetInt32(Other.SShoppingCart, 0);
            return RedirectToAction("SiparisTamamlandi");
        }

        private Payment PaymentProcess(ShoppingCartVM model)
        {
            // İyzipay den aldığımız ödeme sistemi
            Options options = new Options();
            options.ApiKey = "sandbox-tCKTWNbqTdH2AKSIKMlrpaqloHoXwrGZ";
            options.SecretKey = "sandbox-cYvC1iGw7gE3m4akZsOezx58AsoRr0AU";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(1111, 9999).ToString();
            request.Price = model.OrderHeader.OrderTotal.ToString();
            request.PaidPrice = model.OrderHeader.OrderTotal.ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            //PaymentCard paymentCard = new PaymentCard();
            //paymentCard.CardHolderName = "John Doe";
            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";
            //paymentCard.RegisterCard = 0;
            //request.PaymentCard = paymentCard;

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.OrderHeader.CardName;
            paymentCard.CardNumber = model.OrderHeader.CardNumber;
            paymentCard.ExpireMonth = model.OrderHeader.ExpirationMonth;
            paymentCard.ExpireYear = model.OrderHeader.ExpirationYear;
            paymentCard.Cvc = model.OrderHeader.CVC;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = model.OrderHeader.Id.ToString();
            buyer.Name = model.OrderHeader.Name;
            buyer.Surname = model.OrderHeader.Surname;
            buyer.GsmNumber = model.OrderHeader.PhoneNumber;
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = model.OrderHeader.Adress;
            buyer.Ip = "85.34.78.112";
            buyer.City = model.OrderHeader.City;
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            var claimIdentity = (ClaimsIdentity)User.Identity; // Sipariş veren kullanıcıyı belirleme
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier); // Sipariş veren kullanıcıyı belirleme
            foreach (var item in _context.ShoppingCarts.Where(x => x.ApplicationUserId == claim.Value).Include(x =>x.Product))
            {
                basketItems.Add(new BasketItem()
                {
                    Id = item.Id.ToString(),
                    Name = item.Product.Name,
                    Category1 = item.Product.CategoryId.ToString(),
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (item.Price * item.Count).ToString()
                });
            }
            request.BasketItems = basketItems;

            return Payment.Create(request, options);
        }

        public IActionResult SiparisTamamlandi()
        {
            return View();
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity; // Sipariş veren kullanıcıyı belirleme
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier); // Sipariş veren kullanıcıyı belirleme
            ShoppinCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _context.ShoppingCarts.Where(x => x.ApplicationUserId == claim.Value).Include(x => x.Product)
            };
            ShoppinCartVM.OrderHeader.OrderTotal = 0;
            ShoppinCartVM.OrderHeader.ApplicationUser = _context.ApplicationUsers.FirstOrDefault(x => x.Id == claim.Value);
            foreach (var item in ShoppinCartVM.ListCart)
            {
                ShoppinCartVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppinCartVM);
        }
        public IActionResult Success()
        {
            return View();
        }
        public IActionResult Add(int cartId) // niye cartId verdik. Sadece o müşterinin sepetine ekleyebilsin diye
        {
            var cart = _context.ShoppingCarts.FirstOrDefault(x => x.Id == cartId); // Sepeti Getirdik
            cart.Count += 1; // Birer birer arttırıyoruz
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Decrease(int cartId) // niye cartId verdik. Sadece o müşterinin sepetine ekleyebilsin diye
        {
            var cart = _context.ShoppingCarts.FirstOrDefault(x => x.Id == cartId); // Sepeti Getirdik
            if (cart.Count == 1)
            {
                var count = _context.ShoppingCarts.Where(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count(); // Eğer 1 e gelip 0 lanırsa ürünü silsin
                _context.ShoppingCarts.Remove(cart);
                _context.SaveChanges();
                HttpContext.Session.SetInt32(Other.SShoppingCart, count - 1);
            }
            else
            {
                cart.Count -= 1; // Birer birer azaltıyoruz
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId) // niye cartId verdik. Sadece o müşterinin sepetine ekleyebilsin diye
        {
            var cart = _context.ShoppingCarts.FirstOrDefault(x => x.Id == cartId); // Sepeti Getirdik

            var count = _context.ShoppingCarts.Where(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
            _context.ShoppingCarts.Remove(cart);
            _context.SaveChanges();
            HttpContext.Session.SetInt32(Other.SShoppingCart, count - 1);

            return RedirectToAction(nameof(Index));
        }
    }
}
