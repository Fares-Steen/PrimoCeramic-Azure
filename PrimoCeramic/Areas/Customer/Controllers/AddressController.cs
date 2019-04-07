using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PrimoCeramic.Data;
using PrimoCeramic.Models;
using PrimoCeramic.Models.ViewModel;
using PrimoCeramic.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PrimoCeramic.Areas.Customer.Controllers
{
    [Authorize(Roles = SD.CustomerUser)]
    [Area("Customer")]
    public class AddressController : Controller
    {

        private readonly ApplicationDbContext _db;

        [BindProperty]
        public List<CustomerAddress> CustomerAddress { get; set; }

        [BindProperty]
        [TempData]
        public string Message { get; set; }

        [BindProperty]
        public string UserId { get; set; }



        


        public AddressController(ApplicationDbContext db)
        {




            _db = db;

            //var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);




        }


        public IActionResult Index(string Message)
        {

            UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // will give the user's userId
            //var userName = User.FindFirst(ClaimTypes.Name).Value; // will give the user's userName
            //var userEmail = User.FindFirst(ClaimTypes.Email).Value; // will give the user's Email

            TempData["meesage"] = Message;
            CustomerAddress = _db.CustomerAddresses.Where(x => x.PersonID == UserId).ToList();

            return View(CustomerAddress);
        }


        public IActionResult Create()
        {
          
            return View(CustomerAddress);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST(CustomerAddress customerAdress, string returnUrl = null)
        {
           // returnUrl = returnUrl ?? Url.Content("~/Customer/Address");

            customerAdress.PersonID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!ModelState.IsValid)
            {
                return View(customerAdress);
            }
            CustomerAddress.Add(customerAdress);
            _db.Add(customerAdress);
            await _db.SaveChangesAsync();
            if (returnUrl == null)
            {
                return RedirectToAction("Index", new { Message="The Address has been added" });
            }
            return LocalRedirect(returnUrl);
           // return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            CustomerAddress address = _db.CustomerAddresses.Where(x => x.Id == id).FirstOrDefault();
            return View(address);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            

            if (id == null)
            {
                return NotFound();
            }
            var address = await _db.CustomerAddresses.FindAsync(id);
            _db.CustomerAddresses.Remove(address);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            CustomerAddress address = _db.CustomerAddresses.Where(x => x.Id == id).FirstOrDefault();
            return View(address);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(CustomerAddress customerAdress,int? id)
        {
            var UserId= User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (id == null)
            {
                return NotFound();
            }
            var addressDb = await _db.CustomerAddresses.FindAsync(id);

            addressDb.Country = customerAdress.Country;
            addressDb.City = customerAdress.City;
            addressDb.Address = customerAdress.Address;
            addressDb.Phone = customerAdress.Phone;

            if(addressDb.IsDefault != customerAdress.IsDefault && customerAdress.IsDefault==true)
            {
                var DefaultAdress = _db.CustomerAddresses.Where(a => a.PersonID == UserId).Where(a => a.IsDefault == true).FirstOrDefault();
                DefaultAdress.IsDefault = false;
                await _db.SaveChangesAsync();

            }
            addressDb.IsDefault = customerAdress.IsDefault;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}