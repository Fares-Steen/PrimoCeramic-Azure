using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrimoCeramic.Models;
using PrimoCeramic.Data;
using Microsoft.EntityFrameworkCore;
using PrimoCeramic.Extensions;
using PrimoCeramic.TempModels;
using System.Security.Claims;
using PrimoCeramic.Models.ViewModel;
using System.Text;

namespace PrimoCeramic.Controllers
{
    [Area("Customer")]
    public class BuildingMaterialsController : Controller


    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        ProductsPagingViewModel productsPagingVM { get; set; } 

        private int PageSize = 18;

        public BuildingMaterialsController(ApplicationDbContext db)
        {
            _db = db;
            productsPagingVM = new ProductsPagingViewModel()
            {
                Products = new List<Products>(),

                SpecialTags =new List<SpecialTags>(),
                SelectedTag = 0,
                Quantity=0,
                Id=0
                
            };

        }
        [HttpGet]
        public async Task<IActionResult> Index(int productPage = 1,int tagnumber=0)
        {
            productsPagingVM.Products = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags)
               .Include(m => m.ProductCountry).Include(m => m.ProductApplication).Include(m => m.ProducSurface)
               .Where(a => a.ProductTypes.Name == "BuildingMaterials"&&a.Deleted==false).ToListAsync();

            productsPagingVM.SpecialTags = (from a in productsPagingVM.Products.GroupBy(i => i.SpecialTags).Select(grp => grp.FirstOrDefault())
                                            select new SpecialTags
                              {
                                  Id = a.SpecialTagsId,
                                  Name = a.SpecialTags.Name
                              }
                            ).ToList();

            

            var newList = productsPagingVM.SpecialTags.ToList();


            StringBuilder param = new StringBuilder();
            param.Append("/Customer/BuildingMaterials?productPage=:");


            newList.Add(new SpecialTags()
            {

                Id = 0,
                Name = "Show All"


            });
            newList.Reverse();
            productsPagingVM.SpecialTags = newList;

            if (tagnumber != 0)
            {
                productsPagingVM.Products = productsPagingVM.Products.Where(a => a.SpecialTagsId == tagnumber).ToList();



                

                productsPagingVM.SelectedTag = tagnumber;
                param.Append("&tagnumber=");
                param.Append(tagnumber);
            }
           

        

            var count = productsPagingVM.Products.Count;

            productsPagingVM.Products = productsPagingVM.Products.Skip((productPage - 1) * PageSize).Take(PageSize).ToList();

            productsPagingVM.PagingInfo = new PagingInfo()
            {
                CurrentPage = productPage,
                ItemPerPage = PageSize,
                TotalItem = count,
                urlParam = param.ToString()
            };


            return View(productsPagingVM);
        }

      

        public async Task<IActionResult> Details(int id,string Message)
        {
            var product = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Include(m => m.ProductApplication).Include(m => m.ProductCountry).Include(m => m.ProducSurface).Where(m => m.Id == id).FirstOrDefaultAsync();
            var ProductImages = await _db.ProductImages.Where(x => x.ProductId == id).OrderByDescending(x => x.IsDefault).ToListAsync();
            ProductDetailsCartViewModel productDetailsCartVM = new ProductDetailsCartViewModel()
            {
                Products = product,
                Quantity=0,
                ProductImages=ProductImages
            };
            TempData["meesage"] = Message;
            return View(productDetailsCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToFromSample(int id)
        {
            List<string> lstSampleCart = HttpContext.Session.Get<List<string>>("ssSampleCart");
            if (lstSampleCart == null)
            {
                lstSampleCart = new List<string>();
                Debug.WriteLine("***********************Heelo*****************");
            }

            lstSampleCart.Add(id.ToString());
            HttpContext.Session.Set("ssSampleCart", lstSampleCart);
            Debug.WriteLine("****************************************" + lstSampleCart.Count);
            return RedirectToAction("Details", "BuildingMaterials", new { area = "Customer",id=id,Message="The Products has been added to sample cart" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromSample(int id)
        {
            List<string> lstSampleCart = HttpContext.Session.Get<List<string>>("ssSampleCart");
            if (lstSampleCart == null)
            {
                lstSampleCart = new List<string>();
            }

            lstSampleCart.Remove(id.ToString());
            HttpContext.Session.Set("ssSampleCart", lstSampleCart);

            return RedirectToAction("Details", "BuildingMaterials", new { area = "Customer",id=id, Message = "The Products has been removed" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddtoShopping(int id, int Quantity=1, string fromUrl= "Details")
        {
            bool isAuthenticated = User.Identity.IsAuthenticated;
            

            if (isAuthenticated)
            {
                string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (Quantity < 0)
                {
                    return NotFound();
                }
                var product = await _db.Products.Where(a => a.Id == id).FirstOrDefaultAsync();
                if (product == null)
                {
                    return NotFound();
                }

                var productincart =await _db.CustomerShoppingCart.Where(a=>a.PersonID==UserId&&a.ProductId==id).FirstOrDefaultAsync();

                if (productincart == null) { 
                _db.CustomerShoppingCart.Add(new CustomerShoppingCart()
                {
                    PersonID = UserId,
                    ProductId = id,
                    Quantity = Quantity


                });

                await _db.SaveChangesAsync();
                List<ShoppingCart> lstShoppingCart = HttpContext.Session.Get<List<ShoppingCart>>("ssShoppingCart");
                if (lstShoppingCart == null)
                {
                    lstShoppingCart = new List<ShoppingCart>();

                }

                lstShoppingCart.Add(new ShoppingCart { Quantity = Quantity, ProductId = id });

                HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);
                }
                else
                {
                    productincart.Quantity += Quantity;
                    await _db.SaveChangesAsync();

                    List<ShoppingCart> lstShoppingCart = HttpContext.Session.Get<List<ShoppingCart>>("ssShoppingCart");
                    if (lstShoppingCart == null)
                    {
                        lstShoppingCart = new List<ShoppingCart>();

                    }

                    lstShoppingCart.Find(a => a.ProductId == id).Quantity += Quantity;

                    HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);
                }
            }
            else
            {



                List<ShoppingCart> lstShoppingCart = HttpContext.Session.Get<List<ShoppingCart>>("ssShoppingCart");
                if (lstShoppingCart == null)
                {
                    lstShoppingCart = new List<ShoppingCart>();

                }
                var productincart = lstShoppingCart.Find(a => a.ProductId == id);

                if (productincart == null)
                {
                    lstShoppingCart.Add(new ShoppingCart { Quantity = Quantity, ProductId = id });
                }
                else
                {
                    lstShoppingCart.Find(a => a.ProductId == id).Quantity += Quantity;
                }
                

                HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);

            }
            if(fromUrl== "Details")
            return RedirectToAction("Details", "BuildingMaterials", new { area = "Customer",id=id, Message = "The Products has been added to shopping cart" });
            else
            return RedirectToAction(fromUrl, "BuildingMaterials", new { area = "Customer", id = id, Message = "The Products has been added to shopping cart" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovefromShopping(int id)
        {
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
            return RedirectToAction("Details", "BuildingMaterials", new { area = "Customer",id=id, Message = "The Products has been removed" });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
