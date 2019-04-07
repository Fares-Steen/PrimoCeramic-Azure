using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using PrimoCeramic.Models;
using PrimoCeramic.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PrimoCeramic.Areas.Identity.Pages.Account
{
    //To allow any one to access rigister page
    [AllowAnonymous]
    //[Authorize(Roles = SD.SuperAdminEndUser)]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        //created by fares 
        private readonly RoleManager<IdentityRole> _roleManager;


        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
           IEmailSender emailSender,
            //created by fares
            RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            //created by fares
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name="Phone Number")]
        public string PhoneNumber { get; set; }


        [Display(Name = "Super Admin")]
        public bool IsAdmin { get; set; }


        [Display(Name = "Super Admin")]
        public bool IsSuperAdmin { get; set; }

        }
        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Customer/Address/Create");
            if (ModelState.IsValid)
            {

                //before user
                //var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

                //afteruser

                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email,Name=Input.Name ,PhoneNumber=Input.PhoneNumber};


                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    if(!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
                    {

                        await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
                    }


                    if (!await _roleManager.RoleExistsAsync(SD.SuperAdminEndUser))
                    {

                        await _roleManager.CreateAsync(new IdentityRole(SD.SuperAdminEndUser));
                    }

                    if (!await _roleManager.RoleExistsAsync(SD.CustomerUser))
                    {

                        await _roleManager.CreateAsync(new IdentityRole(SD.CustomerUser));
                    }

                    if (Input.IsSuperAdmin)
                    {
                        await _userManager.AddToRoleAsync(user, SD.SuperAdminEndUser);
                    }
                    else if (Input.IsAdmin)
                    {
                        await _userManager.AddToRoleAsync(user, SD.AdminEndUser);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.CustomerUser);
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //To cancel auto login after creating a user
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);


                    //to send conformation email
                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    // return RedirectToAction("Index", "AdminUser",new { area="Admin"});


                    //To cancel auto login after creating a user
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
