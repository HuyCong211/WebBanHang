using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnChuyenNganh.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using DoAnChuyenNganh.Models.EF;



namespace DoAnChuyenNganh.Controllers
{
    [Authorize(Roles = "Customer")] // Bắt buộc đăng nhập mới vào được
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController()
        {
            _context = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
        }
        // GET: Profile
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return HttpNotFound();

            var model = new ProfileViewModel
            {
                Email = user.Email,
                Fullname = user.Fullname,
                Phone = user.Phone
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.Identity.GetUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return HttpNotFound();

            user.Fullname = model.Fullname;
            user.Phone = model.Phone;
            _context.SaveChanges();

            ViewBag.Success = "Cập nhật thông tin thành công!";
            return View(model);
        }

        // GET: Đổi mật khẩu
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(DoAnChuyenNganh.Models.EF.ChangePasswordViewModel model)

        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                ViewBag.Success = "Đổi mật khẩu thành công!";
                return RedirectToAction("Index");
            }
            AddErrors(result);
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}