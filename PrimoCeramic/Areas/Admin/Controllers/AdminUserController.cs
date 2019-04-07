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

    [Authorize(Roles =SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class AdminUserController : Controller
    {

        private readonly ApplicationDbContext _db;

        public AdminUserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(from a in _db.ApplicationUsers
                        join p in _db.UserRoles
                        on a.Id equals p.UserId
                        join b in _db.Roles
                        on p.RoleId equals b.Id
                        where b.Name == SD.AdminEndUser
                        || b.Name == SD.SuperAdminEndUser
                        select a
                   

                        );
           // return View(_db.ApplicationUsers.ToList());
        }

        //Get Edit
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var userFromDb = await _db.ApplicationUsers.FindAsync(id);

            if (userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }


        //Post Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                ApplicationUser userfromDb = _db.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();

                userfromDb.Name = applicationUser.Name;
                userfromDb.PhoneNumber = applicationUser.PhoneNumber;

                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(applicationUser);
        }



        //Get Delete
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var userFromDb = await _db.ApplicationUsers.FindAsync(id);

            if (userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }


        //Post Delete
        [HttpPost,ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(string id)
        {
           
            
                ApplicationUser userfromDb = _db.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();

                userfromDb.LockoutEnd = DateTime.Now.AddYears(1000);
               

                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            
            
        }
    }
}