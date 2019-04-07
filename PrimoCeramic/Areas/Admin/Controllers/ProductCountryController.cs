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
    public class ProductCountryController : Controller
    {

        private readonly ApplicationDbContext _db;

        public ProductCountryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {

            return View(_db.ProductCountry.ToList());

        }

        public IActionResult Create()
        {
            return View();
        }


        //POST create action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCountry ProductCountry)
        {
            //if all poramiters which are required is ok(no nulls)
            if (ModelState.IsValid)
            {
                _db.Add(ProductCountry);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(ProductCountry);
        }


        //Get edit  action method
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ProductCountry = await _db.ProductCountry.FindAsync(id);
            if (ProductCountry == null)
            {
                return NotFound();
            }
            return View(ProductCountry);
        }


        //POST edit  action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCountry ProductCountry)
        {

            if (id != ProductCountry.Id)
            {
                return NotFound();
            }

            //if all poramiters which are required is ok(no nulls)
            if (ModelState.IsValid)
            {
                _db.Update(ProductCountry);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(ProductCountry);
        }

        //Get details  action method
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ProductCountry = await _db.ProductCountry.FindAsync(id);
            if (ProductCountry == null)
            {
                return NotFound();
            }
            return View(ProductCountry);
        }

        //Get Delete  action method
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ProductCountry = await _db.ProductCountry.FindAsync(id);
            if (ProductCountry == null)
            {
                return NotFound();
            }
            return View(ProductCountry);
        }


        //POST Delete  action method
        //action name if we want to change the method not (for example:"DeleteConfirmed")
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var ProductCountry = await _db.ProductCountry.FindAsync(id);
            _db.ProductCountry.Remove(ProductCountry);
            
            
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}