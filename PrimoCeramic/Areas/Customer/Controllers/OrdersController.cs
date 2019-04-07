using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PrimoCeramic.Data;
using PrimoCeramic.Models.ViewModel;
using PrimoCeramic.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using PrimoCeramic.Models;
using DinkToPdf.Contracts;
using DinkToPdf;
using System.IO;

namespace PrimoCeramic.Areas.Customer.Controllers
{
    [Authorize(Roles = SD.CustomerUser)]
    [Area("Customer")]
    public class OrdersController : Controller
    {

        private readonly ApplicationDbContext _db;

        private IConverter _converter;

        [BindProperty]
        public CustomerAllOrdersViewModel CustomerAllOrdersVM { get; set; }


        private int PageSize = 20;


        public OrdersController(ApplicationDbContext db, IConverter converter)
        {
            _db = db;
            _converter = converter;

            CustomerAllOrdersVM = new CustomerAllOrdersViewModel()
            {
                OrderViewModelWithId=new List<OrderViewModelWithId>(),
            };
        }

        public async Task<IActionResult>Index(int productPage=1)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var OrdersIds = _db.Orders.Where(a => a.PersonID == UserId&&a.OrderStatus!= "Canceled").OrderByDescending(x => x.OrderDate);

            var OrdersDetails = await (from a in _db.ProductsSelectedForOrder
                                where a.Orders.PersonID == UserId && a.Orders.OrderStatus=="Confirmed"
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
                                    OrderStatus=a.Orders.OrderStatus,
                                    Vat=a.Orders.Vat
                                    

                                }
                                 ).ToListAsync();


            foreach (var order in OrdersIds)
            {
                CustomerAllOrdersVM.OrderViewModelWithId.Add(new OrderViewModelWithId()
                {
                    OrderViewModel = OrdersDetails.Where(a => a.OrderId == order.Id).ToList()
                });
            }

            StringBuilder param = new StringBuilder();
            param.Append("/Customer/Orders?ordersPage=:");
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


            return View(CustomerAllOrdersVM);
        }
        [HttpPost]
        public async Task<IActionResult> Index(int? id)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (id == null)
            {
                return NotFound();
            }

            var OrderDb = _db.Orders.Where(a => a.Id == id).FirstOrDefault();
            if (UserId == OrderDb.PersonID)
            {

            
            OrderDb.OrderStatus = "Canceled";
           await _db.SaveChangesAsync();
            }
            else
            {
                {
                    return NotFound();
                }
            }
            return RedirectToAction(nameof(Index));
        }


        public IActionResult CreatePDF()
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = @"C:\PDFCreator\Employee_Report.pdf"
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = "<!DOCTYPEhtml><html><head><style>table{font-family:arial,sans-serif;border-collapse:collapse;width:100%;}td,th{border:1pxsolid#dddddd;text-align:left;padding:8px;}tr:nth-child(even){background-color:#dddddd;}</style></head><body><h5>Thank you for your order.....</h5><br /><h5>We will contact you soon for delivery and payment.</h5><br /><h3>Your Order Details:</h3><div><table><tr style='background-color:#1f3933;color:white'><th style='text-align:center;width:20%'>{0}</th><th style='text-align:center;'><label>ProductName</label></th><th style='text-align:center;'><label>Type</label></th><th style='text-align:center;'><label>QTY</label></th><th style='text-align:center;'><label>Price</label></th><th style='text-align:center;'><label>Total</label></th><th style='text-align:center;'><label>Vat</label></th><th style='text-align:center;'><label>Total with VAT</label></th></tr> < tr style = 'padding:50px' >< td >< img src = '{0}' width = '50px' style = 'margin:auto; display:block;border-radius:5px;border:1px solid #bbb9b9' /></ td >< td style = 'text-align:center' >{ 1 }</ td >< td style = 'text-align:center' >{ 2 }</ td >< td style = 'text-align:center' >{ 3 }</ td >< td style = 'text-align:center' >{ 4 }</ td >< td style = 'text-align:center' >{ 5 }</ td >< td style = 'text-align:center' >{ 6 }</ td >< td style = 'text-align:center' >{ 7 }</ td ></ tr >< tr style = 'background-color:#1f3933;color:white' >< th style = 'text-align:center; width:30%;' >{ 0 }</ th >< th style = 'text-align:center;' >< label ></ label ></ th >< th style = 'text-align:center;' >< label ></ label ></ th >< th style = 'text-align:center;' ></ th >< th style = 'text-align:center;' >< label >{ 1 }</ label ></ th >< th style = 'text-align:center;' >< label > Total </ label ></ th >< th style = 'text-align:center;' >< label >{ 1 }</ label ></ th >< th style = 'text-align:center;' >< label >{ 2 }</ label ></ th ></ tr ></ table ></ div >< br />< br />< br />< div >< a href = 'https://www.primoceramic.com' style = 'text-decoration:none;color: #1f3933' > PrimoCermaic </ a >< br />< br />< a href = 'mailto:info@primoceramic.com' style = 'text-decoration:none;color: #1f3933' > info@primoceramic.com </ a ></ div ></ body ></ html > ",
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };


            var file = _converter.Convert(pdf);

            return File(file, "application/pdf", "EmployeeReport.pdf");
        }

    }
}