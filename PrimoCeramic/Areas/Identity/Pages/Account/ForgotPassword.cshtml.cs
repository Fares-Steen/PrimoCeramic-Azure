using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using PrimoCeramic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using PrimoCeramic.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PrimoCeramic.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;
        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)//|| !(await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code },
                    protocol: Request.Scheme);

                var Emails = await _db.Emails.Where(a => a.EmailType == "resetpassword").FirstOrDefaultAsync();
                if (Emails != null)
                {
                    BodyBuilder bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = $@"To reset your password please <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a><br/><br/><a href='https://www.primoceramic.com' style='text-decoration:none;color: #1f3933'>PrimoCermaic</a><br/><br/><a href='mailto:info@primoceramic.com' style='text-decoration:none;color: #1f3933'>info@primoceramic.com</a>";
                    bodyBuilder.TextBody = "Please reset your password by ";


                    SendEmail sendEmail = new SendEmail(Input.Email, "Reset Password", bodyBuilder, Emails);
                    sendEmail.SendRestPassword();

                }
                
                
                //await _emailSender.SendEmailAsync(
                //    Input.Email,
                //    "Reset Password",
                //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
