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
    public class SpecialTagsController : Controller
    {

        private readonly ApplicationDbContext _db;

        public SpecialTagsController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Special tag view home page
        public IActionResult Index()
        {
            //pass a list of all special tags to the view page
            return View(_db.SpecialTags.ToList());
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
        public async Task<IActionResult> Create(SpecialTags specialtag)
        {
            //if all poramiters which are required is ok(no nulls)
            if (ModelState.IsValid)
            {
                //add a new specialtag to local database
                _db.Add(specialtag);

                //save the new specialtag to database
                await _db.SaveChangesAsync();

                //go to index page after creating the new specialtag
                return RedirectToAction(nameof(Index));

                
            }


            //Need to know what is this*********
            //i think its for in case of error
            return View(specialtag);
        
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
            var specialTag = await _db.SpecialTags.FindAsync(id);

            //if specialtag not found
            if (specialTag == null)
            {
                //show not found page
                return NotFound();
            }

            //show the view and give it a specialtag
            return View(specialTag);
        }

        //POST edit  action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTags specialTag)
        {
            //if the id which passed is not equal to target specialtag
            if (id != specialTag.Id)
            {
                //show not found page
                return NotFound();     
            }

            //if vaidation is ok
            if (ModelState.IsValid)
            {
                //update the local database
                _db.Update(specialTag);

                //save updated in database
                await _db.SaveChangesAsync();

                //redirect to home after saving
                return RedirectToAction(nameof(Index));


            }

            //show the view
            return View(specialTag);

        }


        //Get details  action method
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _db.SpecialTags.FindAsync(id);
            if (specialTag == null)
            {
                return NotFound();
            }
            return View(specialTag);
        }

        //Get Delete  action method
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialtag = await _db.SpecialTags.FindAsync(id);
            if (specialtag == null)
            {
                return NotFound();
            }
            return View(specialtag);
        }


        //POST Delete  action method
        //action name if we want to change the method not (for example:"DeleteConfirmed")
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var specialtag = await _db.SpecialTags.FindAsync(id);
            _db.SpecialTags.Remove(specialtag);


            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}