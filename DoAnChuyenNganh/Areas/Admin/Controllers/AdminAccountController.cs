using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace DoAnChuyenNganh.Areas.Admin.Controllers
{
    [AllowAnonymous]
    public class AdminAccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private IAuthenticationManager Auth => HttpContext.GetOwinContext().Authentication;

        private ApplicationUserManager UserManager =>
            _userManager ?? (_userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Bạn phải nhập đầy đủ thông tin.");
                return View(model);
            }

            var user = await UserManager.FindAsync(model.Email, model.Password);
            if (user == null || !await UserManager.IsInRoleAsync(user.Id, "Admin"))
            {
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không hợp lệ, hoặc không có quyền Admin.");
                return View(model);
            }

            // Tạo identity cho ADMIN
            var identity = await UserManager.CreateIdentityAsync(user, "AdminCookie");
            identity.AddClaim(new System.Security.Claims.Claim("Fullname", user.Fullname ?? ""));


            // Xóa cookie admin cũ rồi đăng nhập mới
            Auth.SignOut("AdminCookie");
            Auth.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, identity);

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Auth.SignOut("AdminCookie");
            return RedirectToAction("Login", "AdminAccount");
        }
    }
}
