using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using PrimoCeramic.Data;
using PrimoCeramic.Models;
using PrimoCeramic.Models.ViewModel;
using PrimoCeramic.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PrimoCeramic.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public CustomerAllOrdersViewModel CustomerAllOrdersVM { get; set; }


        [BindProperty]
        public ProductsSelectedForOrder ProductsSelectedFOrder { get; set; }

        private int PageSize =30;

        public OrdersController(ApplicationDbContext db)
        {
            _db = db;
            CustomerAllOrdersVM = new CustomerAllOrdersViewModel()
            {
                OrderViewModelWithId = new List<OrderViewModelWithId>(),
                Filter=true
                
            };
          

            ProductsSelectedFOrder = new ProductsSelectedForOrder();
        }


        public async Task<IActionResult> Index(bool IsSent, bool IsPaid,bool Filter ,int productPage = 1, string searchName = null, string searchEmail = null, string searchPhone = null, string searchDate = null)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            StringBuilder param = new StringBuilder();

            param.Append("/Admin/Orders?productPage=:");


            param.Append("&searchName=");

            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");

            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");

            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchDate=");

            if (searchDate != null)
            {
                param.Append(searchDate);
            }
            param.Append("&IsPaid=");
           
                param.Append(IsPaid);
            

            param.Append("&IsSent=");
           
                param.Append(IsSent);
            

            param.Append("&Filter=");
            
                param.Append(Filter);
            
            List<Orders> OrdersIds;

          
            //if (Filter==null||Filter==true)
            //{
            //    OrdersIds = _db.Orders.OrderByDescending(x => x.OrderDate).ToList();
            //}
            //else
            //{
            //    OrdersIds = _db.Orders.Where(x => x.IsPaid == IsPaid && x.IsSent == IsSent).OrderByDescending(x => x.OrderDate).ToList();
                
            //}
          

            var OrdersDetails = await (from a in _db.ProductsSelectedForOrder
                                       join b in _db.ApplicationUsers

                                       on a.Orders.PersonID equals b.Id
                                      
                                       select new OrderViewModel

                                       {
                                     OrderId = a.OrderId,
                                     PersonName = b.Name,
                                     ProductId = a.ProductId,
                                     ProductName = a.Products.Name,
                                     PersonEmail = b.Email,
                                     ProductImge = a.Products.Image,
                                     Price = a.Prise,
                                     Quantity = a.Quantity,
                                     Date = a.Orders.OrderDate,
                                     Country=a.Orders.Country,
                                     City=a.Orders.City,
                                     Address = a.Orders.Address,
                                     Phone=a.Orders.Phone,
                                     IsSent = a.Orders.IsSent,
                                     IsPaid = a.Orders.IsPaid,
                                     ProductType = a.Products.ProductTypes.Name,
                                     OrderStatus = a.Orders.OrderStatus,
                                     Vat=a.Orders.Vat

                                 }
                                 ).ToListAsync();


            if (searchName != null)
            {
                OrdersDetails = OrdersDetails.Where(a => a.PersonName.ToLower().Contains(searchName.ToLower())).ToList();


            }
            if (searchEmail != null)
            {
                OrdersDetails = OrdersDetails.Where(a => a.PersonEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            if (searchPhone != null)
            {
                OrdersDetails = OrdersDetails.Where(a => a.Phone.ToLower().Contains(searchPhone.ToLower())).ToList();
            }

            if ( Filter ==true)
            {
                OrdersDetails = OrdersDetails.Where(a => a.IsPaid==IsPaid&&a.IsSent==IsSent).ToList();
            }

                if (searchDate != null)
            {
                try
                {
                    var appDate = Convert.ToDateTime(searchDate);

                    OrdersDetails = OrdersDetails.Where(a => a.Date.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();
                }
                catch (Exception ex)
                {

                }
            }
            OrdersIds = (from a in OrdersDetails.OrderByDescending(d => d.Date).GroupBy(i => i.OrderId).Select(grp => grp.FirstOrDefault())
                         select new Orders
                         {
                             Id = a.OrderId
                         }).ToList();


           foreach (var order in OrdersIds)
            {
                CustomerAllOrdersVM.OrderViewModelWithId.Add(new OrderViewModelWithId()
                {
                    OrderViewModel = OrdersDetails.Where(a => a.OrderId == order.Id).ToList()
                });
            }
              

            //fpr paging numbers
            var count = CustomerAllOrdersVM.OrderViewModelWithId.Count;


            CustomerAllOrdersVM.OrderViewModelWithId = CustomerAllOrdersVM.OrderViewModelWithId
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize).ToList();


            CustomerAllOrdersVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemPerPage = PageSize,
                TotalItem = count,
                urlParam = param.ToString()
            };
            CustomerAllOrdersVM.IsSent = IsSent;
            CustomerAllOrdersVM.IsPaid = IsPaid;



           
            CustomerAllOrdersVM.Filter = Filter;


           

            CustomerAllOrdersVM.CustomerName = searchName;
            CustomerAllOrdersVM.Phone = searchPhone;
            CustomerAllOrdersVM.Email = searchEmail;
            CustomerAllOrdersVM.Date = searchDate;
            return View(CustomerAllOrdersVM);
        }
        [HttpPost]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var OrderDb = _db.Orders.Where(a => a.Id == id).FirstOrDefault();

            OrderDb.OrderStatus = "Canceled";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var OrdersDetails = (from a in _db.ProductsSelectedForOrder
                                 where a.Orders.Id == id
                                 select new OrderViewModel

                                 {
                                     OrderId = a.OrderId,
                                     ProductId = a.ProductId,
                                     ProductsSelectedForOrderid = a.Id,
                                     ProductName = a.Products.Name,
                                     ProductImge = a.Products.Image,
                                     Price = a.Prise,
                                     Quantity = a.Quantity,
                                     Date = a.Orders.OrderDate,
                                     Address = a.Orders.Address,
                                     IsSent = a.Orders.IsSent,
                                     IsPaid = a.Orders.IsPaid,
                                     ProductType = a.Products.ProductTypes.Name,
                                     OrderStatus = a.Orders.OrderStatus


                                 }
                                 ).ToList();


            return View(OrdersDetails);
        }
        [HttpPost,ActionName("Edit")]
        public async Task<IActionResult>Edit(int id,bool IsSent,bool IsPaid)
        {
            var order = await _db.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.IsPaid = IsPaid;
            order.IsSent = IsSent;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> EditOrderProduct(int? id, int ProductsOrder)
        {

            ProductsSelectedFOrder =await _db.ProductsSelectedForOrder.Where(x => x.Id == ProductsOrder).FirstOrDefaultAsync();
            if (ProductsSelectedFOrder == null)
            {
                return NotFound();
            }


            return View(ProductsSelectedFOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrderProduct()
        {
            var productsSelectedForOrderDb = _db.ProductsSelectedForOrder.Where(x => x.OrderId == ProductsSelectedFOrder.Id && x.ProductId == ProductsSelectedFOrder.ProductId).FirstOrDefault();
            if (productsSelectedForOrderDb == null)
            {
                return NotFound();
            }
            productsSelectedForOrderDb.Quantity = ProductsSelectedFOrder.Quantity;
            productsSelectedForOrderDb.Prise = ProductsSelectedFOrder.Prise;

            await _db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = productsSelectedForOrderDb.OrderId });
        }

        public async Task<IActionResult> DeleteOrderProduct(int id ,int Product)

        {
            var productsSelectedForOrderDb = _db.ProductsSelectedForOrder.Where(x => x.OrderId == id && x.ProductId == Product).FirstOrDefault();
            if (productsSelectedForOrderDb == null)
            {
                return NotFound();
            }
            _db.ProductsSelectedForOrder.Remove(productsSelectedForOrderDb);

            if(_db.ProductsSelectedForOrder.Count(x=>x.OrderId== productsSelectedForOrderDb.OrderId) == 0)
            {
                _db.Orders.Remove(_db.Orders.Where(a=>a.Id==productsSelectedForOrderDb.OrderId).FirstOrDefault());

                return RedirectToAction(nameof(Index));
            }
            _db.ProductsSelectedForOrder.Remove(productsSelectedForOrderDb);
            await _db.SaveChangesAsync();

            
            return RedirectToAction("Edit", new { id = productsSelectedForOrderDb.OrderId });
        }
    }
}