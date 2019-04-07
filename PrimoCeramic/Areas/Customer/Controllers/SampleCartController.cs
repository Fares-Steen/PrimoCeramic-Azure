using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PrimoCeramic.Data;
using PrimoCeramic.Extensions;
using PrimoCeramic.Models;
using PrimoCeramic.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PrimoCeramic.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SampleCartController : Controller
    {
        private readonly ApplicationDbContext _db;


        [BindProperty]
        public SampleCartViewModel SampleCartVM { get; set; }

        public SampleCartController(ApplicationDbContext db)
        {
            _db = db;
            SampleCartVM = new SampleCartViewModel()
            {
                
               Products= new List<Models.Products>()
            };
        }

        //Get Index SampleCart
        public async Task<IActionResult> Index()
        {
            List<string> lstSampleCart = HttpContext.Session.Get<List<string>>("ssSampleCart");
            if (lstSampleCart!=null&&lstSampleCart.Count > 0)
            {
               


                foreach (string cartItem in lstSampleCart)
                {
                    Products prod = await _db.Products.Include(p=>p.ProductTypes).Include(p=>p.SpecialTags).Where(p => p.Id == Convert.ToInt32(cartItem)).FirstOrDefaultAsync();
                    SampleCartVM.Products.Add(prod);
                }
            }
            return View(SampleCartVM);
        }

        
        public IActionResult Remove(int id)
        {
            List<string> lstSampleCart = HttpContext.Session.Get<List<string>>("ssSampleCart");
            if (lstSampleCart.Count > 0 && lstSampleCart != null)
            {
                if (lstSampleCart.Contains(id.ToString()))
                {
                    lstSampleCart.Remove(id.ToString());
                    HttpContext.Session.Set("ssSampleCart", lstSampleCart);
                }
               
            }
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexPost()
        {
            List<string> lstSampleCart = HttpContext.Session.Get<List<string>>("ssSampleCart");

            Debug.WriteLine("****************************************"+ SampleCartVM.Appointments.CustomerEmail);

            SampleCartVM.Appointments.AppointmentDate = SampleCartVM.Appointments.AppointmentDate
                                                    .AddHours(SampleCartVM.Appointments.AppointmentTime.Hour)
                                                    .AddMinutes(SampleCartVM.Appointments.AppointmentTime.Minute);

            Appointments appointments = SampleCartVM.Appointments;

            _db.Appointments.Add(appointments);
            await _db.SaveChangesAsync();

            int appintmentId = appointments.ID;

            foreach(string productId in lstSampleCart)
            {
                ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment()
                {
                    AppointmentID = appintmentId,
                    ProductId =Convert.ToInt32(productId)
                };

                _db.ProductsSelectedForAppointment.Add(productsSelectedForAppointment);

            }

            await _db.SaveChangesAsync();
            lstSampleCart = new List<string>();
            HttpContext.Session.Set("ssSampleCart", lstSampleCart);

            return RedirectToAction("AppointmentConfirmation","SampleCart",new { Id=appintmentId});
        }


        //Get
        public async Task<IActionResult> AppointmentConfirmation(int id)
        {
             SampleCartVM.Appointments = await _db.Appointments.Where(a => a.ID == id).FirstOrDefaultAsync();

            List<ProductsSelectedForAppointment> objProdLis = await _db.ProductsSelectedForAppointment.Where(p => p.AppointmentID == id).ToListAsync();

            foreach(var prodAptObj in objProdLis)
            {
                SampleCartVM.Products.Add(await _db.Products.Include(p=>p.ProductTypes).Include(p=>p.SpecialTags).Where(p=>p.Id==prodAptObj.ProductId).FirstOrDefaultAsync());
            }

            return View(SampleCartVM);
        }
    }
}