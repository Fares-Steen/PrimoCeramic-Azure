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
    public class StaticsController : Controller
    {

        private readonly ApplicationDbContext _db;

        public StaticsController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Special tag view home page
        public IActionResult Index()
        {
            //pass a list of all special tags to the view page
            return View(_db.Statics.ToList());
        }


        //create new special tag view page
        public IActionResult Create()
        {
            //show the create page
            return View();
        }

        //Get Create action mode (resive the id nomber
        [HttpPost]
        
        [ValidateAntiForgeryToken]//to check all validation
        public async Task<IActionResult> Create(Statics Static)
        {
            //if all poramiters which are required is ok(no nulls)
            if (ModelState.IsValid)
            {
                //add a new Static to local database
                _db.Add(Static);

                //save the new Static to database
                await _db.SaveChangesAsync();

                //go to index page after creating the new Static
                return RedirectToAction(nameof(Index));

                
            }


            //Need to know what is this*********
            //i think its for in case of error
            return View(Static);
        
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

            //search the Static by id 
            var Static = await _db.Statics.FindAsync(id);

            //if Static not found
            if (Static == null)
            {
                //show not found page
                return NotFound();
            }

            //show the view and give it a Static
            return View(Static);
        }

        //POST edit  action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Statics Static)
        {
            //if the id which passed is not equal to target Static
            if (id != Static.Id)
            {
                //show not found page
                return NotFound();     
            }

            //if vaidation is ok
            if (ModelState.IsValid)
            {
                //update the local database
                _db.Update(Static);

                //save updated in database
                await _db.SaveChangesAsync();

                //redirect to home after saving
                return RedirectToAction(nameof(Index));


            }

            //show the view
            return View(Static);

        }


        //Get details  action method
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Static = await _db.Statics.FindAsync(id);
            if (Static == null)
            {
                return NotFound();
            }
            return View(Static);
        }

        //Get Delete  action method
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Static = await _db.Statics.FindAsync(id);
            if (Static == null)
            {
                return NotFound();
            }
            return View(Static);
        }


        //POST Delete  action method
        //action name if we want to change the method not (for example:"DeleteConfirmed")
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var Static = await _db.Statics.FindAsync(id);
            _db.Statics.Remove(Static);


            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}