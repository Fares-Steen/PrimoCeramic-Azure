using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PrimoCeramic.Data;
using PrimoCeramic.Extensions;
using PrimoCeramic.Models;
using PrimoCeramic.TempModels;
using PrimoCeramic.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PrimoCeramic.Areas.Customer.Controllers
{
    [Authorize(Roles = SD.CustomerUser)]
    [Area("Customer")]
    public class RedirectController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public List<CustomerShoppingCart> CustomerShoppingCart { get; set; }

        public RedirectController( ApplicationDbContext db)
        {
            _db = db;
        }
        public async  Task<IActionResult> Index(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Customer/Home");
            bool isAuthenticated = User.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                CustomerShoppingCart = new List<CustomerShoppingCart>();
                List<ShoppingCart> lstShoppingMirge = new List<ShoppingCart>();
                List<ShoppingCart> lstShoppingCart = HttpContext.Session.Get<List<ShoppingCart>>("ssShoppingCart");
                string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                List<CustomerShoppingCart> CustomerShoppingDb = _db.CustomerShoppingCart.Where(x => x.PersonID == UserId).ToList();
                if (lstShoppingCart != null && lstShoppingCart.Count > 0)
                {


                    // string UserId = _userManager.GetUserId(User);
                    

                    

                    foreach (ShoppingCart cartItem in lstShoppingCart)
                    {
                        if (CustomerShoppingDb.Contains(CustomerShoppingDb.Find(x => x.ProductId == cartItem.ProductId)))
                        {
                            // lstShoppingCart.Remove(cartItem);

                        }
                        else
                        {
                            var item = new CustomerShoppingCart()
                            {
                                PersonID = UserId,
                                ProductId = cartItem.ProductId,
                                Quantity = cartItem.Quantity
                            };
                            CustomerShoppingDb.Add(item);
                            _db.CustomerShoppingCart.Add(item);
                        }


                    }
                    lstShoppingCart = new List<ShoppingCart>();
                    foreach (var customercart in CustomerShoppingDb)
                    {
                        lstShoppingCart.Add(new ShoppingCart
                        {
                            ProductId = customercart.ProductId,
                            Quantity = customercart.Quantity
                        });
                    }
                    
                }else if (CustomerShoppingDb != null && CustomerShoppingDb.Count > 0)
                {
                    lstShoppingCart = new List<ShoppingCart>();
                    foreach (var customercart in CustomerShoppingDb)
                    {
                        lstShoppingCart.Add(new ShoppingCart
                        {
                            ProductId = customercart.ProductId,
                            Quantity = customercart.Quantity
                        });
                    }
                }
                await _db.SaveChangesAsync();
                HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);
            }
           
            return LocalRedirect(returnUrl);
          //  return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
           // returnUrl = returnUrl ?? Url.Content("~/");
            bool isAuthenticated = User.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                CustomerShoppingCart = new List<CustomerShoppingCart>();
                List<ShoppingCart> lstShoppingMirge = new List<ShoppingCart>();
                List<ShoppingCart> lstShoppingCart = HttpContext.Session.Get<List<ShoppingCart>>("ssShoppingCart");
                if (lstShoppingCart != null && lstShoppingCart.Count > 0)
                {


                    // string UserId = _userManager.GetUserId(User);
                    string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                    List<CustomerShoppingCart> CustomerShoppingDb = _db.CustomerShoppingCart.Where(x => x.PersonID == UserId).ToList();

                    foreach (ShoppingCart cartItem in lstShoppingCart)
                    {
                        if (CustomerShoppingDb.Contains(CustomerShoppingDb.Find(x => x.ProductId == cartItem.ProductId)))
                        {
                            // lstShoppingCart.Remove(cartItem);

                        }
                        else
                        {
                            _db.CustomerShoppingCart.Add(new CustomerShoppingCart()
                            {
                                PersonID = UserId,
                                ProductId = cartItem.ProductId,
                                Quantity = cartItem.Quantity
                            });

                        }


                    }

                    foreach (var customercart in CustomerShoppingDb)
                    {
                        lstShoppingCart.Add(new ShoppingCart
                        {
                            ProductId = customercart.ProductId,
                            Quantity = customercart.Quantity
                        });
                    }
                    await _db.SaveChangesAsync();
                    HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);
                }
            }

            return RedirectToAction("~/Customer");
        }

    }
}