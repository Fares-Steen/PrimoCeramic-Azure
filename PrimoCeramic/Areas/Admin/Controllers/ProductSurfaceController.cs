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
    public class ProductSurfaceController : Controller
    {

        private readonly ApplicationDbContext _db;

        public ProductSurfaceController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {

            return View(_db.ProductSurface.ToList());

        }

        public IActionResult Create()
        {
            return View();
        }


        //POST create action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductSurface ProductSurface)
        {
            //if all poramiters which are required is ok(no nulls)
            if (ModelState.IsValid)
            {
                _db.Add(ProductSurface);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(ProductSurface);
        }


        //Get edit  action method
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ProductSurface = await _db.ProductSurface.FindAsync(id);
            if (ProductSurface == null)
            {
                return NotFound();
            }
            return View(ProductSurface);
        }


        //POST edit  action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductSurface ProductSurface)
        {

            if (id != ProductSurface.Id)
            {
                return NotFound();
            }

            //if all poramiters which are required is ok(no nulls)
            if (ModelState.IsValid)
            {
                _db.Update(ProductSurface);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(ProductSurface);
        }

        //Get details  action method
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ProductSurface = await _db.ProductSurface.FindAsync(id);
            if (ProductSurface == null)
            {
                return NotFound();
            }
            return View(ProductSurface);
        }

        //Get Delete  action method
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ProductSurface = await _db.ProductSurface.FindAsync(id);
            if (ProductSurface == null)
            {
                return NotFound();
            }
            return View(ProductSurface);
        }


        //POST Delete  action method
        //action name if we want to change the method not (for example:"DeleteConfirmed")
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var ProductSurface = await _db.ProductSurface.FindAsync(id);
            _db.ProductSurface.Remove(ProductSurface);
            
            
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}