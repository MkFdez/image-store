using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Store.Models;
using DataAccess;
using Models;
using System.IO;
using Services;
using CacheUtilities;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using Logger;
using Logger.Models;

namespace Store.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public IServicePack ServicePack;
        public ILogger _logger;
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger("AccountLogger");
       
        public AccountController(IServicePack servicePack, ILogger logger) 
        {
            ServicePack = servicePack;
            _logger = logger;
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, IServicePack servicePack, ILogger logger )
        {
            UserManager = userManager;
            SignInManager = signInManager;
            ServicePack = servicePack;
            _logger = logger;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try { 
            if (!ModelState.IsValid)
            {
                log.Info("Model State Not valid");
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account-Resend", model.Email);
                    ViewBag.errorMessage = "You must have a confirmed email to log on.";
                    return View("Error");
                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            if (user == null) 
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
            var result = await SignInManager.PasswordSignInAsync( user.UserName, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        using (var context = new Project1DBEntities())
                        {
                            try
                            {
                                HttpCookie cookie = new HttpCookie("vals");
                                var userTemp = context.Users.First(x => x.Email == model.Email);
                                int id = userTemp.Id;

                                cookie["picture"] = userTemp.ProfilePicture.Image != null ? userTemp.ProfilePicture.Image : @"\ProfilePictures\default.png";

                                if (model.RememberMe)
                                {
                                    cookie.Expires = DateTime.MaxValue;
                                    cookie["exp"] = DateTime.MaxValue.ToString();
                                }
                                else
                                {
                                    cookie["exp"] = DateTime.MinValue.ToString();
                                }
                                Response.Cookies.Add(cookie);
                            } catch (Exception ex)
                            {
                                log.Error("Error while creating profile picture cookie", ex);
                            }
                        }

                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }catch(Exception ex)
            {
                log.Error("Something happened while during the log in", ex);
                return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                log.Info("Model State Not valid");
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            try
            {
                var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);

                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(model.ReturnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid code.");
                        return View(model);
                }
            }catch(Exception ex)
            {
                log.Error("Error while verifying code", ex);
                return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    using(var context = new Project1DBEntities())
                    {
                        try
                        {
                            context.Users.First(x => x.Email == model.Email).ProfilePicture = new ProfilePicture() { Image = null };
                            context.SaveChanges();
                            HttpCookie cookie = new HttpCookie("vals");
                            cookie["picture"] = @"\ProfilePictures\default.png";
                            Response.Cookies.Add(cookie);
                        }catch(Exception ex)
                        {
                            log.Error("Error while creating profile picture cookie", ex);
                        }
                    }
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account");                  
                   
                    ViewBag.Message = "Check your email and confirm your account, you must be confirmed "
                         + "before you can log in.";

                    return View("Info");
                   // return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            else
            {
                log.Info("Model State Not valid");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(int userId, string code)
        {
            if (userId == default(int) || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                log.Info("Model State Not valid");
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + ">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                log.Info("Model State Not valid");
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == default(int))
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    using (var context = new Project1DBEntities())
                    {
                        try
                        {
                            context.Users.First(x => x.Email == loginInfo.Email).ProfilePicture = new ProfilePicture() { Image = null };
                            context.SaveChanges();
                            HttpCookie cookie = new HttpCookie("vals");
                            cookie["picture"] = @"\ProfilePictures\default.png";
                            cookie["exp"] = DateTime.MinValue.ToString();
                            Response.Cookies.Add(cookie);
                        }catch(Exception ex)
                        {
                            log.Error("Error while creating profile picture cookie", ex);
                        }
                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }
            else
            {
                log.Info("Model State Not valid");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        public async Task<ActionResult> Profile()
        {
            int userId = User.Identity.GetUserId<int>();
            return View(await ServicePack.GetProfile());
        }
        [HttpPost]
        public async Task<ActionResult> Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PostedPicture != null)
                {
                    string filename = Path.GetFileName(model.PostedPicture.FileName).Replace(" ", "");
                    string guid = Guid.NewGuid().ToString();
                    string folder = Path.Combine(Server.MapPath("~/ProfilePictures"), guid);
                    Directory.CreateDirectory(folder);
                    folder = Path.Combine(folder, filename);
                    model.PostedPicture.SaveAs(folder);
                    await ServicePack.UpdateProfilePicture(Path.Combine("/ProfilePictures", guid, filename));
                    var expire = HttpContext.Request.Cookies["vals"]["exp"];
                    HttpCookie cookie = new HttpCookie("vals");
                    cookie["picture"] = Path.Combine("/ProfilePictures", guid, filename);
                    cookie.Expires = DateTime.Parse(expire);
                    Response.Cookies.Add(cookie);
                }
                ServicePack.UpdateSocialMedia(new SocialMedia()
                {
                    Instagram = model.Instagram,
                    Facebook = model.Facebook,
                    Website = model.Website,
                    Twitter = model.Twitter,
                    Pinterest = model.Pinterest,
                });
                return RedirectToAction("Index", "Publication");
            }
            else
            {
                log.Info("Model State Not valid");
                return RedirectToAction("Profile", "Account");
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
        private async Task<string> SendEmailConfirmationTokenAsync(int userID, string subject, string email = "")
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            string body = CacheUtilities.ChacheUtilities.GetCached<string>("CMail", CacheUtilities.ChacheUtilities.getMail);


            body = body.Replace("{ConfirmationLink}", callbackUrl);
            body = body.Replace("{UserName}", email);
            await UserManager.SendEmailAsync(userID, subject,
              body);

            return callbackUrl;
        }

        public async Task<ActionResult> Index(string username)
        {
            var data = await ServicePack.GetCreator(username);
            return View(data);
        }

        public async Task<ActionResult> CreatorImages(string username, int count)
        {
            try
            {
                var data = await ServicePack.GetCreatorPubliactions(username, count);
                return Json(JsonSerializer.Serialize<List<SimplePublicationViewModel>>(data));
            }catch(Exception ex)
            {
                log.Error($"Error accessing to {username} images with count: {count}", ex);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}