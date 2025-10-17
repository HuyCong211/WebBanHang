using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using DoAnChuyenNganh.Models;

namespace DoAnChuyenNganh.App_Start
{
    public class RoleConfig
    {
        public static void SeedRoles()
        {
            var context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // ✅ 1. Tạo role Admin nếu chưa có
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }

            // ✅ 2. Tạo role Customer nếu chưa có
            if (!roleManager.RoleExists("Customer"))
            {
                roleManager.Create(new IdentityRole("Customer"));
            }

            // ✅ 3. Tạo tài khoản admin mặc định
            var adminEmail = "admin@gmail.com";
            var adminUser = userManager.FindByEmail(adminEmail);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };
                var result = userManager.Create(user, "Admin@123"); // mật khẩu mặc định

                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }
        }
    }
}