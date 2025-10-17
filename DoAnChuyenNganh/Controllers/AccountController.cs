using DoAnChuyenNganh.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace DoAnChuyenNganh.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private IAuthenticationManager Auth => HttpContext.GetOwinContext().Authentication;

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

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await UserManager.FindAsync(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Đăng nhập không hợp lệ.");
                return View(model);
            }

            var identity = await UserManager.CreateIdentityAsync(user, "CustomerCookie");

            Auth.SignOut("CustomerCookie");
            Auth.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, identity);

            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Fullname = model.Fullname,
                Phone = model.Phone
            };

            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user.Id, "Customer");

                var identity = await UserManager.CreateIdentityAsync(user, "CustomerCookie");
                Auth.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                return RedirectToAction("Index", "Home");
            }

            AddErrors(result);
            return View(model);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("❌ ModelState invalid");
                return View(model);
            }

            System.Diagnostics.Debug.WriteLine("🔹 Nhận request ForgotPassword: " + model.Email);

            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Không tìm thấy user trong DB");
                return View("ForgotPasswordConfirmation");
            }

            System.Diagnostics.Debug.WriteLine("✅ Tìm thấy user: " + user.Email);

            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account",
                new { userId = user.Id, code = code },
                protocol: Request.Url.Scheme);

            System.Diagnostics.Debug.WriteLine("📨 Chuẩn bị gửi mail reset...");

            await UserManager.SendEmailAsync(
                user.Id,
                "Đặt lại mật khẩu",
                $"Vui lòng nhấp vào <a href=\"{callbackUrl}\">liên kết này</a> để đặt lại mật khẩu."
            );

            System.Diagnostics.Debug.WriteLine("✅ Đã gọi SendEmailAsync xong");

            return RedirectToAction("ForgotPasswordConfirmation", "Account");

        }

        [HttpGet]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation", "Account");

            AddErrors(result);
            return View();
        }
        [HttpGet]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Auth.SignOut("CustomerCookie");
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
        }
    }
}
