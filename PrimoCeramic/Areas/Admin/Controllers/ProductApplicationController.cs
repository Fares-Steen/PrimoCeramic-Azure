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
    public class ProductApplicationController : Controller
    {

        private readonly ApplicationDbContext _db;

        public ProductApplicationController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {

            return View(_db.ProductApplication.ToList());

        }

        public IActionResult Create()
        {
            return View();
        }


        //POST create action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductApplication ProductApplication)
        {
            //if all poramiters which are required is ok(no nulls)
            if (ModelState.IsValid)
            {
                _db.Add(ProductApplication);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(ProductApplication);
        }


        //Get edit  action method
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ProductApplication = await _db.ProductApplication.FindAsync(id);
            if (ProductApplication == null)
            {
                return NotFound();
            }
            return View(ProductApplication);
        }


        //POST edit  action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductApplication ProductApplication)
        {

            if (id != ProductApplication.Id)
            {
                return NotFound();
            }

            //if all poramiters which are required is ok(no nulls)
            if (ModelState.IsValid)
            {
                _db.Update(ProductApplication);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(ProductApplication);
        }

        //Get details  action method
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ProductApplication = await _db.ProductApplication.FindAsync(id);
            if (ProductApplication == null)
            {
                return NotFound();
            }
            return View(ProductApplication);
        }

        //Get Delete  action method
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ProductApplication = await _db.ProductApplication.FindAsync(id);
            if (ProductApplication == null)
            {
                return NotFound();
            }
            return View(ProductApplication);
        }


        //POST Delete  action method
        //action name if we want to change the method not (for example:"DeleteConfirmed")
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var ProductApplication = await _db.ProductApplication.FindAsync(id);
            _db.ProductApplication.Remove(ProductApplication);
            
            
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}