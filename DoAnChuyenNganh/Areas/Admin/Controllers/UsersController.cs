using DoAnChuyenNganh.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnChuyenNganh.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : AdminBaseController
    {
        private readonly ApplicationDbContext context = new ApplicationDbContext(); // 🟢 thêm dòng này
        // GET: Admin/Users
        public ActionResult Index()
        {
            var users = context.Users.ToList();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var list = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                Fullname = u.Fullname,
                Phone = u.Phone,
                Roles = string.Join(", ", userManager.GetRoles(u.Id))
            }).ToList();

            return View(list);
        }
        public ActionResult Edit(string id)
        {
            var user = context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return HttpNotFound();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Fullname = user.Fullname,
                Phone = user.Phone,
                Roles = userManager.GetRoles(user.Id)
            };

            ViewBag.AllRoles = context.Roles.Select(r => r.Name).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserViewModel model, string[] selectedRoles)
        {
            var user = context.Users.FirstOrDefault(x => x.Id == model.Id);
            if (user == null)
                return HttpNotFound();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            user.Fullname = model.Fullname;
            user.Phone = model.Phone;
            context.SaveChanges();

            var userRoles = userManager.GetRoles(user.Id);
            userManager.RemoveFromRoles(user.Id, userRoles.ToArray());

            if (selectedRoles != null)
                userManager.AddToRoles(user.Id, selectedRoles);

            TempData["Message"] = "Cập nhật thành công!";
            return RedirectToAction("Index");
        }

        // =====================
        // 🟢 TẠO MỚI TÀI KHOẢN
        // =====================
        public ActionResult Create()
        {
            ViewBag.AllRoles = context.Roles.Select(r => r.Name).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterUserViewModel model, string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                // Kiểm tra email trùng
                if (context.Users.Any(x => x.Email == model.Email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại!");
                    ViewBag.AllRoles = context.Roles.Select(r => r.Name).ToList();
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Fullname = model.Fullname,
                    Phone = model.Phone
                };

                var result = userManager.Create(user, model.Password);
                if (result.Succeeded)
                {
                    if (selectedRoles != null)
                        userManager.AddToRoles(user.Id, selectedRoles);

                    TempData["Message"] = "Tạo tài khoản thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error);
                }
            }

            ViewBag.AllRoles = context.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        // =====================
        // 🔴 XOÁ TÀI KHOẢN
        // =====================
        [HttpPost]
        public ActionResult Delete(string id)
        {
            var user = context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return HttpNotFound();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Xoá tất cả quyền của user
            var roles = userManager.GetRoles(user.Id);
            if (roles.Any())
                userManager.RemoveFromRoles(user.Id, roles.ToArray());

            // Xoá user khỏi database
            context.Users.Remove(user);
            context.SaveChanges();

            TempData["Message"] = "Xoá tài khoản thành công!";
            return RedirectToAction("Index");
        }

    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Roles { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
    public class RegisterUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Họ tên")]
        public string Fullname { get; set; }

        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }

}
