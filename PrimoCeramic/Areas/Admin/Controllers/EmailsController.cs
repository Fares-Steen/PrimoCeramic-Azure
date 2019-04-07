using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrimoCeramic.Data;
using PrimoCeramic.Models;
using PrimoCeramic.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PrimoCeramic.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class EmailsController : Controller
    {

        private readonly ApplicationDbContext _db;

        
        

        public EmailsController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Special tag view home page
        public IActionResult Index()
        {
            //pass a list of all special tags to the view page
            return View(_db.Emails.ToList());
        }


        //create new special tag view page
        public IActionResult Create()
        {
            //show the create page
            return View();
        }

        //Get Create action mode (resive the id nomber
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]//to check all validation
        public async Task<IActionResult> CreateEmail(Emails Emails)
        {
            //if all poramiters which are required is ok(no nulls)
            if (ModelState.IsValid)
            {
                //add a new specialtag to local database
                _db.Add(Emails);

                //save the new specialtag to database
                await _db.SaveChangesAsync();

                //go to index page after creating the new specialtag
                return RedirectToAction(nameof(Index));

                
            }


            
            // in case of error
            return View(Emails);
        
        }


        //Get edit  action method
        public async Task<IActionResult> Edit(int? id)
        {
            //check if the id is not hloding a number
            if (id == null)
            {
                //show not found page
                return NotFound();
            }

            //search the specialtag by id 
            var Email = await _db.Emails.FindAsync(id);

            //if specialtag not found
            if (Email == null)
            {
                //show not found page
                return NotFound();
            }

            //show the view and give it a specialtag
            return View(Email);
        }

        //POST edit  action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Emails Emails)
        {
            //if the id which passed is not equal to target specialtag
            if (id != Emails.Id)
            {
                //show not found page
                return NotFound();     
            }

            //if vaidation is ok
            if (ModelState.IsValid)
            {
                //update the local database
                _db.Update(Emails);

                //save updated in database
                await _db.SaveChangesAsync();

                //redirect to home after saving
                return RedirectToAction(nameof(Index));


            }

            //show the view
            return View(Emails);

        }


        //Get details  action method
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Email = await _db.Emails.FindAsync(id);
            if (Email == null)
            {
                return NotFound();
            }
            return View(Email);
        }

        //Get Delete  action method
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Email = await _db.Emails.FindAsync(id);
            if (Email == null)
            {
                return NotFound();
            }
            return View(Email);
        }


        //POST Delete  action method
        //action name if we want to change the method not (for example:"DeleteConfirmed")
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var Email = await _db.Emails.FindAsync(id);
            _db.Emails.Remove(Email);


            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}