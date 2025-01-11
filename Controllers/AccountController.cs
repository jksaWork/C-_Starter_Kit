using System.Net;
using System.Text;
using IdentiyFreamwork.Models;
using IdentiyFreamwork.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentiyFreamwork.Controllers
{
    [Route("[controller]")]
    
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
       private readonly  MailService _mailService;

       private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(ILogger<AccountController> logger,
         UserManager<IdentityUser> userManager, 
         SignInManager<IdentityUser> signInManager, 
         RoleManager<IdentityRole> roleManager, 
         MailService mailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _mailService = mailService;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            bool cond = await _roleManager.RoleExistsAsync("Admin");
             if(!cond){
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }
            List<SelectListItem> Items = new List<SelectListItem>();
            Items.Add(new SelectListItem(){
                Value = "Admin", Text = "Admin"
            });

            Items.Add(new SelectListItem(){
                Value = "User", Text = "User"
            }); 
            var model = new RegisterViewModel() {
                SelectRole = Items
            };

            return View(model);
        }

      

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };

               
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded) // 0561885934
                {
                    await _userManager.AddToRoleAsync(user , model.Role);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");   
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            List<SelectListItem> Items = new List<SelectListItem>();
            Items.Add(new SelectListItem(){
                Value = "Admin", Text = "Admin"
            });

            Items.Add(new SelectListItem(){
                Value = "User", Text = "User"
            }); 
             model.SelectRole = Items;

            return View(model);
            }  
        

        [HttpGet("/Account/Login")]
        public IActionResult Login()
        {
            var model = new LoginViewModel();

            return View(model);
        }


        [HttpPost("/Account/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email,model.Password, model.RememberMe , lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");   
                }else{
                    ModelState.AddModelError(string.Empty, "Error On Login Attepm ");
                }
            }
            return View(model);
            }  
        


        [HttpGet("/Account/ForgetPassword")]
        public IActionResult ForgetPassword()
        {
            var model = new ForgetPasswordViewModel();

            return View(model);
        }


        [HttpPost("/Account/ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return  RedirectToAction("Account", "ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", new { token, model.Email }, Request.Scheme);

            await _mailService.SendEmailAsync(model.Email, "Password Reset",
                $"<p>Click <a href='{resetLink}'>here</a> to reset your password.</p>");

            return RedirectToAction("ForgotPasswordConfirmation","Account");
        }
        
        
    [HttpGet("Account/ResetPassword")]
    public IActionResult ResetPassword(string token, string email)
    {
        return View(new ResetPasswordViewModel { Token = token, Email = email });
    }

    [HttpPost("/Account/Account/ResetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return RedirectToAction("ResetPasswordConfirmation", "Account");

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded) return RedirectToAction("Login", "Account");

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }

        [HttpGet("Account/ForgotPasswordConfirmation")]
        public IActionResult ForgotPasswordConfirmation(){
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        [HttpGet("/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); // Redirect to the Home page or desired location
        }

        [HttpGet("/EnableTowFactor")]
        [Authorize]
        public async Task<IActionResult> EnableTowFactor(){
            var user = await _userManager.GetUserAsync(User);
           await _userManager.ResetAuthenticatorKeyAsync(user);
           var token = await _userManager.GetAuthenticatorKeyAsync(user);
           var model = new TowFactorAuthuntication { Token = token };

            return View(model);
        }

        [HttpGet("/EnableAuthenticator")]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var model = new EnableAuthenticatorViewModel
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
            };

            return View(model);
        }

        [HttpPost("/VerifyAuthenticatorCode")]
        public async Task<IActionResult> VerifyAuthenticatorCode(string code)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var is2FaTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);

            if (!is2FaTokenValid)
            {
                ModelState.AddModelError("code", "Invalid code.");
                return View();
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            return RedirectToAction("Index", "Home");
        }
        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            var encodedKey = WebUtility.UrlEncode(unformattedKey);
            var encodedEmail = WebUtility.UrlEncode(email);
            return $"otpauth://totp/MyApp:{encodedEmail}?secret={encodedKey}&issuer=MyApp&digits=6";
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        
    }
}