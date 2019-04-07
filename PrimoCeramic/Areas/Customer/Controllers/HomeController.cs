using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrimoCeramic.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace PrimoCeramic.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
      
        public async Task<IActionResult> Index()
        {

          
            return View();
        }
    }
}