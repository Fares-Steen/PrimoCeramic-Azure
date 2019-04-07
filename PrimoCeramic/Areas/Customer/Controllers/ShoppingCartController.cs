using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PrimoCeramic.Data;
using PrimoCeramic.Extensions;
using PrimoCeramic.HTMLTemplate;
using PrimoCeramic.Models;
using PrimoCeramic.Models.ViewModel;
using PrimoCeramic.Services;
using PrimoCeramic.TempModels;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PrimoCeramic.Areas.Customer.Controllers
{
    [Area("Customer")]
    [BindProperties]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly HostingEnvironment _hostingEnvironment;

        public ShoppingCartViewModel ShoppingCartVM { get; set; }
        public CustomerAddressRole CustomerAddressRole { get; set; }

       private UserManager<IdentityUser> UserManager { get; set; }


        public ShoppingCartController(ApplicationDbContext db, UserManager<IdentityUser> UserManager, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            this.UserManager = UserManager;
            _hostingEnvironment = hostingEnvironment;
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Products>(),
                ShoppingCart = new List<ShoppingCart>(),

                Vat = _db.Statics.Where(a => a.Name == "Vat").FirstOrDefault()

                
            };
        }

        public async Task<IActionResult> Index()
        {
            List<ShoppingCart> lstShoppingCart = HttpContext.Session.Get<List<ShoppingCart>>("ssShoppingCart");
            if (lstShoppingCart != null && lstShoppingCart.Count > 0)
            {



                foreach (ShoppingCart cartItem in lstShoppingCart)
                {
                    
                    Products prod = await _db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).Where(p => p.Id == cartItem.ProductId).FirstOrDefaultAsync();

                    ShoppingCartVM.Products.Add(prod);


                    ShoppingCartVM.ShoppingCart.Add(cartItem);

                    
                }
            }
            
            

            return View(ShoppingCartVM);
        }

        public async Task<IActionResult> Remove(int id)
        {
            List<ShoppingCart> lstShopping = HttpContext.Session.Get<List<ShoppingCart>>("ssShoppingCart");
            bool isAuthenticated = User.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var prod = await _db.CustomerShoppingCart.Where(x => x.PersonID == UserId && x.ProductId == id).FirstOrDefaultAsync();

                if (prod != null)
                {



                    _db.CustomerShoppingCart.Remove(prod);

                    await _db.SaveChangesAsync();
                }
                List<ShoppingCart> lstShoppingCart = HttpContext.Session.Get<List<ShoppingCart>>("ssShoppingCart");
                if (lstShoppingCart == null)
                {
                    lstShoppingCart = new List<ShoppingCart>();
                }



                lstShoppingCart.Remove(lstShoppingCart.Find(x => x.ProductId == id));


                HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);
            }
            else
            {


                List<ShoppingCart> lstShoppingCart = HttpContext.Session.Get<List<ShoppingCart>>("ssShoppingCart");
                if (lstShoppingCart == null)
                {
                    lstShoppingCart = new List<ShoppingCart>();
                }



                lstShoppingCart.Remove(lstShoppingCart.Find(x => x.ProductId == id));


                HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChooseAddress()
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            CustomerAddressRole = new CustomerAddressRole()
            {
                CustomerAddress = await _db.CustomerAddresses.Where(x => x.PersonID == UserId).ToListAsync()
            };
          

            return View(CustomerAddressRole);

        }

        [HttpPost,ActionName("ChooseAddress")]
        public async Task<IActionResult> ChooseAddressPost()
        {
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Products>(),
                ShoppingCart = new List<ShoppingCart>(),
                Vat=_db.Statics.Where(a=>a.Name=="Vat").FirstOrDefault()



            };

            List<ShoppingCart> lstShoppingCart = HttpContext.Session.Get<List<ShoppingCart>>("ssShoppingCart");
            if (lstShoppingCart != null && lstShoppingCart.Count > 0)
            {



                foreach (ShoppingCart cartItem in lstShoppingCart)
                {

                    Products prod = await _db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).Where(p => p.Id == cartItem.ProductId).FirstOrDefaultAsync();

                    ShoppingCartVM.ShoppingCart.Add(cartItem);
                    ShoppingCartVM.Products.Add(prod);


                    


                }
            }
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int ChooenAddress = CustomerAddressRole.SelectedRole;
            var adrressDb =await _db.CustomerAddresses.Where(x => x.Id == ChooenAddress).FirstOrDefaultAsync();
            Orders Order = new Orders()
            {
                PersonID = UserId,
                Country = adrressDb.Country,
                City = adrressDb.City,
                Address = adrressDb.Address,
                Phone = adrressDb.Phone,
                OrderDate = DateTime.Now,
                IsSent = false,
                IsPaid = false,
                OrderStatus = "Confirmed",
                Vat = ShoppingCartVM.Vat.DoubleValue


            };
            _db.Orders.Add(Order);
            await _db.SaveChangesAsync();

            int OrderId = Order.Id;

            foreach(var Cart in ShoppingCartVM.ShoppingCart)
            {
                var prod = await _db.Products.Where(p => p.Id == Cart.ProductId).FirstOrDefaultAsync();
                ProductsSelectedForOrder productsSelectedForOrder = new ProductsSelectedForOrder()
                {
                    OrderId = OrderId,
                    ProductId = Cart.ProductId,
                    Quantity = Cart.Quantity,
                    Prise = prod.Price
                };
                _db.ProductsSelectedForOrder.Add(productsSelectedForOrder);
            }
            
            List<ShoppingCart> lstShopping = new List<ShoppingCart>();
             HttpContext.Session.Set("ssShoppingCart",lstShopping);
           
            _db.CustomerShoppingCart.RemoveRange(_db.CustomerShoppingCart.Where(x => x.PersonID == UserId));
            
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(OrderConfirmation), Order);
        }

        public async Task<IActionResult> OrderConfirmation(Orders Order)
        {
            var OrderVM = await (from a in _db.ProductsSelectedForOrder where a.OrderId == Order.Id
                                 
                                 select new OrderViewModel

                                 {
                                     OrderId = a.OrderId,
                                     ProductId = a.ProductId,
                                     ProductName = a.Products.Name,
                                     ProductImge = a.Products.Image,
                                     Price = a.Prise,
                                     Quantity = a.Quantity,
                                     Date = a.Orders.OrderDate,
                                     Country = a.Orders.Country,
                                     City = a.Orders.City,
                                     Address = a.Orders.Address,
                                     Phone = a.Orders.Phone,
                                     IsSent = a.Orders.IsSent,
                                     IsPaid = a.Orders.IsPaid,
                                     ProductType = a.Products.ProductTypes.Name,
                                     OrderStatus = a.Orders.OrderStatus,
                                     Vat= ShoppingCartVM.Vat.DoubleValue


                                 }
                                 ).ToListAsync();

            OrderViewModelWithId orderViewModelWithId = new OrderViewModelWithId()
            {
                OrderViewModel = OrderVM
            };



            //htmlemail+= OrderHTML.OrderViewModel;
            var Emails = await _db.Emails.Where(a => a.EmailType == "order").FirstOrDefaultAsync();
            if (Emails != null)
            {
              
                OrderHTML orderHTML = new OrderHTML(OrderVM);
                string htmlemail = orderHTML.HtmalGenerated();
                string Email = UserManager.GetUserName(User);
                SendEmail sendEmail = new SendEmail(Email, "Order Confirmation", htmlemail, Emails);
                sendEmail.SendOrder();
            }
            
            return View(orderViewModelWithId);
        }
    }
}