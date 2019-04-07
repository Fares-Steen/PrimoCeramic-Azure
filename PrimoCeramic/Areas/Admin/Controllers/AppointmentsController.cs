using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PrimoCeramic.Data;
using PrimoCeramic.Models.ViewModel;
using PrimoCeramic.Models;
using PrimoCeramic.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace PrimoCeramic.Areas.Admin.Controllers
{

    [Authorize (Roles=SD.AdminEndUser+","+SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;

        private int PageSize = 30;

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult Index(int productPage = 1, string searchName = null, string searchEmail = null, string searchPhone = null, string searchDate = null)
        {

            //who is logedin
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            AppointmentViewmodel appointmentVM = new AppointmentViewmodel()
            {
                Appointments = new List<Models.Appointments>()
            };

            //for paging 
            StringBuilder param = new StringBuilder();

            param.Append("/Admin/Appointments?productPage=:");


            param.Append("&searchName=");

            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");

            if (searchName != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");

            if (searchName != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchDate=");

            if (searchName != null)
            {
                param.Append(searchDate);
            }

            //
            appointmentVM.Appointments = _db.Appointments.Include(a => a.SalesPerson).ToList();


            //if the user is superadmin
            if (User.IsInRole(SD.AdminEndUser))
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.SalesPersonID == claim.Value).ToList();
            }

            if (searchName != null)
            {


                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();

            }
            if (searchEmail != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            if (searchPhone != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerPhone.ToLower().Contains(searchPhone.ToLower())).ToList();
            }
            if (searchDate != null)
            {
                try
                {
                    var appDate = Convert.ToDateTime(searchDate);

                    appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();
                }
                catch (Exception ex)
                {

                }
            }


            //fpr paging numbers
            var count = appointmentVM.Appointments.Count;

            appointmentVM.Appointments = appointmentVM.Appointments.OrderBy(p => p.AppointmentDate)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize).ToList();


            appointmentVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemPerPage = PageSize,
                TotalItem = count,
                urlParam = param.ToString()
        };

            return View(appointmentVM);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _db.Products
                                                   join a in _db.ProductsSelectedForAppointment
                                                   on p.Id equals a.ProductId
                                                   where a.AppointmentID == id
                                                   
                                                   select p).Include("ProductTypes");



            AppointmentDetailsViewModel  objApplointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.ID == id).FirstOrDefault(),
                SalesPerosn = _db.ApplicationUsers.Where(a => a.LockoutEnd == null || a.LockoutEnd < DateTime.Now).ToList(),
                Products = productList.ToList()
            };

            return View(objApplointmentVM);

        }

        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUpdate(int? id, AppointmentDetailsViewModel objApplointmentVM)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var appointmentDb = await _db.Appointments.Where(a => a.ID == id).FirstOrDefaultAsync();

                appointmentDb.CustomerName = objApplointmentVM.Appointment.CustomerName;
                appointmentDb.CustomerEmail = objApplointmentVM.Appointment.CustomerEmail;
                appointmentDb.CustomerPhone = objApplointmentVM.Appointment.CustomerPhone;
                appointmentDb.AppointmentDate = objApplointmentVM.Appointment.AppointmentDate.AddHours(objApplointmentVM.Appointment.AppointmentTime.Hour)
                    .AddMinutes(objApplointmentVM.Appointment.AppointmentTime.Minute);
               
                appointmentDb.IsConfirmed = objApplointmentVM.Appointment.IsConfirmed;

                if (User.IsInRole(SD.SuperAdminEndUser))
                {
                    appointmentDb.SalesPersonID = objApplointmentVM.Appointment.SalesPersonID;
                }
                
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                return View(objApplointmentVM);
            }

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _db.Products
                                                      join a in _db.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentID == id

                                                      select p).Include("ProductTypes");



            AppointmentDetailsViewModel objApplointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.ID == id).FirstOrDefault(),
                SalesPerosn = _db.ApplicationUsers.Where(a => a.LockoutEnd == null || a.LockoutEnd < DateTime.Now).ToList(),
                Products = productList.ToList()
            };

            return View(objApplointmentVM);

        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _db.Products
                                                      join a in _db.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentID == id

                                                      select p).Include("ProductTypes");



            AppointmentDetailsViewModel objApplointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.ID == id).FirstOrDefault(),
                SalesPerosn = _db.ApplicationUsers.Where(a => a.LockoutEnd == null || a.LockoutEnd < DateTime.Now).ToList(),
                Products = productList.ToList()
            };

            return View(objApplointmentVM);

        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Debug.WriteLine("****************************************************************");

            var appointmentDb = await _db.Appointments.FindAsync(id);

            _db.Remove(appointmentDb);
           await _db.SaveChangesAsync();
           return RedirectToAction(nameof(Index));
        }

    }
}