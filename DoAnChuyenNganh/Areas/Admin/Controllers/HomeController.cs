using DoAnChuyenNganh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnChuyenNganh.Areas.Admin.Controllers
{
    public class HomeController  : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Home
        public ActionResult Index()
        {
            var fullName = User.Identity.IsAuthenticated
                ? ((System.Security.Claims.ClaimsIdentity)User.Identity)
                    .FindFirst("Fullname")?.Value
                : User.Identity.Name;

            ViewBag.FullName = fullName;

            // Lấy dữ liệu thật từ database
            var productCount = db.SanPhams.Count();
            var orderCount = db.DonHangs.Count();
            var userCount = db.Users.Count();
            var stockCount = db.TonKhoes.Count();

            // Truyền sang view
            ViewBag.ProductCount = productCount;
            ViewBag.OrderCount = orderCount;
            ViewBag.UserCount = userCount;
            ViewBag.StockCount = stockCount;
            return View();
        }
    }
}